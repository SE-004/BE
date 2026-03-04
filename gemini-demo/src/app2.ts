import { geminiSdk } from "./gemini-sdk.ts";
import { geminiRest } from "./gemini-rest.ts";

async function main() {
  const args = process.argv.slice(2);

  const method = args[0]?.toLowerCase();

  const prompt = args.slice(1).join(" ");

  if (!method || !prompt) {
    console.log("⚠️ Usage: npm run dev -- <rest|sdk> <your prompt>");
    console.log(
      "💡 Example: npm run dev -- sdk What is the capital of Germany?",
    );
    return;
  }

  console.log(`\n⏳ Generating response using [${method.toUpperCase()}] `);

  try {
    if (method === "rest") {
      const result = await geminiRest(prompt);
      const text = result.candidates?.[0]?.content?.parts?.[0]?.text;

      console.log("Gemini (REST API):");
      console.log("--------------------------------");
      console.log(text || "No text returned");
      console.log("--------------------------------");
    }

    if (method === "sdk") {
      const result = await geminiSdk(prompt);

      console.log("Gemini (SDK structure):");
      console.log("--------------------------------");
      console.log(`Original prompt: ${result.originalPrompt}\n`);
      console.log(`${result.generatedResponse}`);
      console.log("--------------------------------");
    } else {
      console.log(`❌ Unknown method: "${method}". Please use "rest" or "sdk"`);
    }
  } catch (error) {
    console.error("❌ An error occurred:");
    console.error("❌ An error occurred:");
    console.error(error instanceof Error ? error.message : error);
  }
}

main();
