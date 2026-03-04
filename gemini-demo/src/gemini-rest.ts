import { GenerateContentResponse } from "@google/genai";

export async function geminiRest(prompt: string) {
  // let's make sure we actually have an API key
  const apiKey = process.env.GEMINI_API_KEY;
  if (!apiKey) throw new Error("Looks like your GEMINI_API_KEY is missing");

  const model = "gemini-2.5-flash";
  const url = `https://generativelanguage.googleapis.com/v1beta/models/${model}:generateContent`;

  const res = await fetch(url, {
    method: "POST",
    headers: {
      "X-goog-api-key": apiKey,
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      contents: [{ role: "user", parts: [{ text: prompt }] }],
    }),
  });

  if (!res.ok) {
    const errorText = await res.text();
    throw new Error(`REST API Error (${res.status}): ${errorText}`);
  }

  return (await res.json()) as GenerateContentResponse;
}
