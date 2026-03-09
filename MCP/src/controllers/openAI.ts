import { type Request, type Response } from "express";
import { askLLM } from "../services/llmService.ts";

export async function lmsController(req: Request, res: Response) {
  const prompt = req.body?.prompt;
  if (!prompt) {
    res.status(400).json({ error: "No prompt was provided" });
    return;
  }
  const text = await askLLM(prompt);
  res.json({ content: text });
}
