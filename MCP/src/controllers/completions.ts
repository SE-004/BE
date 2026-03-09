import type { RequestHandler } from "express";
import { z } from "zod";
import {
  PromptBodySchema,
  FinalResponse,
} from "../schemas/completionSchemas.ts";
import { getPokemonInfo } from "../services/pokemonService.ts";

type IncomingPrompt = z.infer<typeof PromptBodySchema>;
type FinalResponseDTO = z.infer<typeof FinalResponse> | { completion: string };

export const createCompletion: RequestHandler<
  unknown,
  FinalResponseDTO,
  IncomingPrompt
> = async (req, res) => {
  const { prompt } = req.body;
  const result = await getPokemonInfo(prompt);
  const isError = "completion" in result;
  res.status(isError ? 400 : 200).json(result);
};
