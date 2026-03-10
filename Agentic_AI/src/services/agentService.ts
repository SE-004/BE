import type {
  ChatCompletionMessageParam,
  ChatCompletionTool,
} from "openai/resources";

import { lmsClient, LLM_MODEL } from "./llmService";
import { getPokemonInfo } from "./pokemonService";
import { getWeather } from "./weatherService";

// Safety valve
const MAX_ITERATIONS = 5; // Think -> Act -> Observe

// Tool definition
const tools: ChatCompletionTool[] = [
  {
    type: "function",
    function: {
      name: "get_weather",
      description: "Get the current weather in a given city",
      parameters: {
        type: "object",
        properties: {
          city: {
            type: "string",
            description: "The city name, e.g. 'Berlin'",
          },
        },
        required: ["city"], // force the LLM to provide a city when calling the get_weather tool
      },
    },
  },
  {
    type: "function",
    function: {
      name: "get_pokemon_info",
      description: "get detailed info about a specific Pokemon",
      parameters: {
        type: "object",
        properties: {
          pokemon: {
            type: "string",
            description: "The pokemon name, e.g. 'Pikachu'",
          },
        },
        required: ["pokemon"], // force the LLM to provide a pokemon when calling the get_pokemon_info tool
      },
    },
  },
];

// Tool executor
async function executeTool(
  name: string,
  args: Record<string, string>,
): Promise<string> {
  switch (name) {
    case "get_weather": {
      // extract "city" argument and pass it to the service
      const result = await getWeather(args.city!);
      return JSON.stringify(result);
    }
    case "get_pokemon_info": {
      const result = await getPokemonInfo(`Tell me about ${args.pokemon}`);
      return JSON.stringify(result);
    }
    default:
      return JSON.stringify({ error: `Unknown tool: ${name}` });
  }
}

export interface AgentSteps {
  type: "tool_call" | "tool_result" | "final_answer";
  tool?: string;
  input?: Record<string, string>;
  output?: string;
  content?: string;
}

// The agent loop
// 1. Send the conversation to the LLM (including tool description)
// 2. If the LLM responds with a tool call -> execute that tool call, append the result, go back to step 1
// 3. if the LLM responds with plain text, that means we have the final answer

export async function runAgent(
  userPrompt: string,
): Promise<{ steps: AgentSteps[]; answer: string }> {
  const steps: AgentSteps[] = [];

  // initialize the conversation with a system prompt, aka the rules for the AI's tool usage
  const messages: ChatCompletionMessageParam[] = [
    {
      role: "system",
      content: `content: You are a helpful assistant with access to tools.

Available tools:
- get_weather: get current weather for a city
- get_pokemon_info: get details about a specific Pokémon

Rules:
- You have NO knowledge about Pokémon. You MUST call get_pokemon_info to get any Pokémon data.
- If a question involves both weather and Pokémon, call get_weather first, then call get_pokemon_info.
- Do not generate a final answer until you have called every tool you need.
- Never say "let me look up" or "I would suggest" — call the tool instead.`,
    },
    {
      role: "user",
      content: userPrompt,
    },
  ];

  // The main agent loop
  for (let i = 0; i < MAX_ITERATIONS; i++) {
    console.log(`\n--- Agent Iteration ${i + 1} ---`);

    // 1. Send entire conversation history and the tool menu to the LLM
    const response = await lmsClient.chat.completions.create({
      model: LLM_MODEL,
      messages,
      tools,
    });

    const choices = response.choices[0]!;
    const message = choices.message;

    // Does the LLM want to call tools?
    if (message.tool_calls?.length) {
      messages.push(message);

      for (const tool_call of message.tool_calls) {
        if (tool_call.type !== "function") continue;

        const args = JSON.parse(tool_call.function.arguments) as Record<
          string,
          string
        >;

        console.log(
          `Tool call: ${tool_call.function.name}(${JSON.stringify(args)})`,
        );

        steps.push({
          type: "tool_call",
          tool: tool_call.function.name,
          input: args,
        });

        // 2. Execute the actual function with the LLM's arguments
        const result = await executeTool(tool_call.function.name, args);
        console.log(`Tool result: ${result.slice(0, 200)}...`);

        steps.push({
          type: "tool_call",
          tool: tool_call.function.name,
          output: result,
        });

        // 3. feed result we got from tool back to LLM
        messages.push({
          role: "tool",
          tool_call_id: tool_call.id,
          content: result,
        });
      }
      continue;
    }

    // No tool call -> final answer
    const answer = message.content ?? "No response from the model";
    console.log("Final answer received");

    // log the final steps and exit the function
    steps.push({ type: "final_answer", content: answer });
    return { steps, answer };
  }

  // if max iterations are reached without returning, then bail out
  return {
    steps,
    answer: "Agent reached max allowed iterations without producing an answer",
  };
}
