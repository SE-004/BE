import { OpenAI } from "openai";
import { type Request, type Response } from "express";

const lmsClient = new OpenAI({
  apiKey: "lmstudio",
  baseURL: "http://127.0.0.1:1234/v1",
});

export async function lmsController(req: Request, res: Response) {
  const prompt = req.body?.prompt;

  if (!prompt) {
    res.status(400).json({ error: "No prompt was provided" });
    return;
  }

  const response = await lmsClient.chat.completions.create({
    model: "meta-llama-3.1-8b-instruct",
    messages: [
      {
        role: "user",
        content: prompt,
      },
    ],
  });

  res.json(response.choices[0]?.message);
}
