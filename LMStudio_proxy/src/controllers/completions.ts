import type { RequestHandler } from "express";
import { zodResponseFormat } from "openai/helpers/zod";
import { z } from "zod";
import { PromptBodySchema, Intent } from "../schemas/completionsSchema";
import OpenAI from "openai";
import type { ChatCompletionMessageParam } from "openai/resources";
import Pokedex from "pokedex-promise-v2";
import { FinalResponse } from "../schemas/completionsSchema";

type IncomingPrompt = z.infer<typeof PromptBodySchema>;
type FinalResponseDTO = z.infer<typeof FinalResponse> | { completion: string };

export const createCompletion: RequestHandler<
  unknown,
  FinalResponseDTO,
  IncomingPrompt
> = async (req, res) => {
  const { prompt, stream } = req.body;
  // OpenAI client setup
  const client = new OpenAI({
    apiKey: "lmstudio",
    baseURL: "http://localhost:1234/v1",
  });
  // Model, we define it here so we can use it in both steps
  const model = "meta-llama-3.1-8b-instruct";

  // Messages, we define it here so we can add more in the future
  const messages: ChatCompletionMessageParam[] = [
    {
      role: "system",
      content:
        "You determine if a question is about Pokémon. You can only answer questions about a single Pokémon and not open-ended questions.",
    },
    {
      role: "user",
      content: prompt,
    },
  ];

  // Step 1: Check if the prompt is about Pokémon
  const checkIntentCompletion = await client.chat.completions.parse({
    model,
    messages,
    temperature: 0,
    response_format: zodResponseFormat(Intent, "Intent"),
  });

  const intent = checkIntentCompletion.choices[0]?.message.parsed;
  if (!intent?.isPokemon) {
    res.status(400).json({
      completion:
        intent?.reason ||
        "I cannot answer this question, try asking about a Pokémon.",
    });
    return;
  }
  console.log(
    `Intent detected. Received a question about: ${intent.pokemonName}`,
  );
  messages.push({
    role: "assistant",
    content: JSON.stringify(intent, null, 2),
  });
  // Step 2: Fetch the Pokémon data from the PokeAPI
  const P = new Pokedex();
  const pokemonData = await P.getPokemonByName(
    intent.pokemonName.toLowerCase(),
  );
  if (!pokemonData) {
    res.status(404).json({
      completion: `Pokémon ${intent.pokemonName} not found.`,
    });
    return;
  }
  console.log(`Fetched data for Pokémon: ${pokemonData.name}`);

  // Step 3: Add the Pokémon data to the messages and generate a final response
  messages.push({
    role: "assistant",
    content: `This is all the relevant data about the Pokémon: ${
      intent.pokemonName
    }: ${JSON.stringify(pokemonData, null, 2)}
    Combine it with what you know about it to give the user a complete answer.`,
  });
  console.log(`Added Pokémon data to messages for further processing.`);
  const finalCompletion = await client.chat.completions.parse({
    model,
    messages,
    temperature: 0,
    response_format: zodResponseFormat(FinalResponse, "FinalResponse"),
  });
  const finalResponse = finalCompletion.choices[0]?.message.parsed;
  if (!finalResponse) {
    res.status(500).json({
      completion: "Failed to generate a final response.",
    });
    return;
  }
  res.json(finalResponse);
};
