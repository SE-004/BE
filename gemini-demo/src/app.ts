import { geminiSdk } from "./gemini-sdk.ts";
import { geminiRest } from "./gemini-rest.ts";
import { input, select } from "@inquirer/prompts";

async function main() {
  console.log("✨ Welcome to the Gemini CLI ✨ \n");

  while (true) {
    try {
      const method = await select({
        message: "How would you like to connect to Gemini?",
        choices: [
          {
            name: "Google SDK (structured JSON)",
            value: "sdk",
            description: "Uses the official package to get a JSON object",
          },
          {
            name: "REST API (raw HTTP)",
            value: "rest",
            description: "Sends a raw HTTP POST request using the native fetch",
          },
          {
            name: " ❌ Exit CLI",
            value: "exit",
            description: "Close the process",
          },
        ],
      });

      if (method === "exit") {
        console.log("\n Exiting Gemini CLI. Good luck on your own.");
        break;
      }

      const prompt = await input({
        message: "What do you want to ask Gemini?",
      });

      console.log(
        `\n⏳ Generating response using [${method.toUpperCase()}]...`,
      );

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
        console.log(
          `❌ Unknown method: "${method}". Please use "rest" or "sdk"`,
        );
      }
    } catch (error) {
      if (error instanceof Error && error.name === "ExitPromptError") {
        return;
      }
      console.error("❌ An error occurred:");
      console.error(error instanceof Error ? error.message : error);
    }
  }
}

main();
