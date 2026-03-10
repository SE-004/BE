import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { z } from "zod";
import { askLLM } from "../services/llmService.ts";
import { getPokemonInfo } from "../services/pokemonService.ts";
import { getWeather } from "../services/weatherService.ts";

const server = new McpServer({
  name: "local-llm-mcp-server",
  version: "1.0.0",
});

server.registerTool(
  "ask_llm",
  {
    title: "Ask LLM",
    description:
      "Send a freeform prompt to the locally-hosted LLM and get a plain-text response.",
    inputSchema: {
      prompt: z.string().describe("The prompt to send to the model."),
    },
  },
  async ({ prompt }) => {
    const text = await askLLM(prompt);
    return { content: [{ type: "text", text }] };
  },
);

server.registerTool(
  "get_pokemon_info",
  {
    title: "Get Pokémon Info",
    description:
      "Ask a question about a specific Pokémon. Detects intent, fetches live PokéAPI data, and returns a structured answer.",
    inputSchema: {
      prompt: z.string().describe('e.g. "Tell me about Charizard"'),
    },
  },
  async ({ prompt }) => {
    const result = await getPokemonInfo(prompt);
    return {
      content: [{ type: "text", text: JSON.stringify(result, null, 2) }],
    };
  },
);

server.registerTool(
  "get_weather",
  {
    title: "Get Weather",
    description:
      "Get the current weather for a given city. Returns temperature, wind speed and conditions",
    inputSchema: {
      city: z.string().describe("The city name, e.g. 'Berlin'"),
    },
  },
  async ({ city }) => {
    const result = await getWeather(city);
    return {
      content: [{ type: "text", text: JSON.stringify(result, null, 2) }],
    };
  },
);

const transport = new StdioServerTransport();
await server.connect(transport);

console.error("MCP server running on stdio ✓");
