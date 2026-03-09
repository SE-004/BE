import { OpenAI } from "openai";

const lmsClient = new OpenAI({
  apiKey: "lmstudio",
  baseURL: process.env.LOCAL_LLM_URL ?? "http://127.0.0.1:1234/v1",
});

export const LLM_MODEL = "meta-llama-3.1-8b-instruct";

export async function askLLM(prompt: string): Promise<string> {
  const response = await lmsClient.chat.completions.create({
    model: LLM_MODEL,
    messages: [{ role: "user", content: prompt }],
  });
  return response.choices[0]?.message.content ?? "No response from model.";
}

export { lmsClient };
