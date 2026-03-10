import { runAgent } from "services/agentService";
import type { Request, Response } from "express";

export async function agentController(req: Request, res: Response) {
  const prompt = req.body.prompt;

  if (!prompt) {
    res.status(400).json({ error: "No prompt received" });
    return;
  }

  try {
    const result = await runAgent(prompt);
    res.json(result);
  } catch (error) {
    console.error("Agent error: ", error);
    res.status(500).json({
      error: "Agent failed",
      details: error instanceof Error ? error.message : String(error),
    });
  }
}
