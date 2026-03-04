import { GoogleGenAI, Type } from "@google/genai";

export interface StructuredAIResponse {
  originalPrompt: string;
  generatedResponse: string;
}

export async function geminiSdk(prompt: string): Promise<StructuredAIResponse> {
  // let's make sure we actually have an API key
  const apiKey = process.env.GEMINI_API_KEY;
  if (!apiKey) throw new Error("Looks like your GEMINI_API_KEY is missing");

  const ai = new GoogleGenAI({ apiKey });

  const response = await ai.models.generateContent({
    model: "gemini-2.5-flash",
    contents: prompt,
    // this is where we structure the output
    config: {
      // force the AI to output pure JSON instead of markdown formatting
      responseMimeType: "application/json",
      //   define the strict blueprint that the AI must follow
      responseSchema: {
        type: Type.OBJECT, // the main output is a JSON object
        // inside the object, we want these two specific strings
        properties: {
          originalPrompt: { type: Type.STRING },
          generatedResponse: { type: Type.STRING },
        },
        // we make both fields strictly required so that the AI does not skip them
        required: ["originalPrompt", "generatedResponse"],
      },
    },
  });

  // The SDK returns a massive response object. We want to extract only the answer text
  if (response.text) {
    // Since we forced application/json, response.text is a JSON string that needs to be parsed into a usable JS object. We then cast it to our interface
    return JSON.parse(response.text) as StructuredAIResponse;
  }

  throw new Error("SDK did not return any text");
}
