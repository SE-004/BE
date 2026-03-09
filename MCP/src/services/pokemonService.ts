import { zodResponseFormat } from "openai/helpers/zod";
import type { ChatCompletionMessageParam } from "openai/resources";
import Pokedex from "pokedex-promise-v2";
import { z } from "zod";
import { Intent, FinalResponse } from "../schemas/completionSchemas.ts";
import { lmsClient, LLM_MODEL } from "./llmService.ts";

export type PokemonResult = z.infer<typeof FinalResponse>;
export type NotPokemonResult = { completion: string };

export async function getPokemonInfo(
  prompt: string,
): Promise<PokemonResult | NotPokemonResult> {
  const messages: ChatCompletionMessageParam[] = [
    {
      role: "system",
      content:
        "You determine if a question is about Pokémon. You can only answer questions about a single Pokémon and not open-ended questions.",
    },
    { role: "user", content: prompt },
  ];

  // Step 1: Detect intent
  const intentCompletion = await lmsClient.chat.completions.parse({
    model: LLM_MODEL,
    messages,
    temperature: 0,
    response_format: zodResponseFormat(Intent, "Intent"),
  });
  const intent = intentCompletion.choices[0]?.message.parsed;
  if (!intent?.isPokemon) {
    return { completion: intent?.reason ?? "Not a Pokémon question." };
  }
  messages.push({ role: "assistant", content: JSON.stringify(intent) });

  // Step 2: Fetch PokéAPI data
  const P = new Pokedex();
  const pokemonData = await P.getPokemonByName(
    intent.pokemonName.toLowerCase(),
  );
  if (!pokemonData) return { completion: "Pokémon not found." };

  // Step 3: Generate structured final response
  messages.push({
    role: "assistant",
    content: `Here is all the data about ${intent.pokemonName}: ${JSON.stringify(pokemonData)}. Use it to answer the user's question.`,
  });

  const final = await lmsClient.chat.completions.parse({
    model: LLM_MODEL,
    messages,
    temperature: 0,
    response_format: zodResponseFormat(FinalResponse, "FinalResponse"),
  });
  return (
    final.choices[0]?.message.parsed ?? {
      completion: "Failed to generate a response.",
    }
  );
}
