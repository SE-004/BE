# Connecting your MCP server to AI clients

This guide walks you through connecting the MCP server from this project to Claude, ChatGPT, and Gemini.

## Prerequisites

### The `.env` file must exist

Our MCP config uses `tsx --env-file=.env` to launch the server. The `--env-file` flag in Node/tsx will **fail if the file doesn't exist**, even if you don't have any environment variables to set yet. Make sure you have a `.env` file in the project root — it can be completely empty:

```bash
touch .env
```

If you skip this step, you'll see an error like:

```
node: .env: not found
```

---

## Claude Desktop App (local, stdio)

This requires the **Claude Desktop app** (downloadable from [claude.ai/download](https://claude.ai/download)) — not the browser version at claude.ai. The browser version only supports remote MCP servers and requires a paid plan. The Desktop app, however, supports local MCP servers on **all plans, including the free tier**.

Claude Desktop can launch your MCP server as a local subprocess and communicate over stdio. This is the simplest setup.

### 1. Find the config file

The config file is located at:

| OS      | Path                                                              |
| ------- | ----------------------------------------------------------------- |
| macOS   | `~/Library/Application Support/Claude/claude_desktop_config.json` |
| Windows | `%APPDATA%\Claude\claude_desktop_config.json`                     |
| Linux   | `~/.config/Claude/claude_desktop_config.json`                     |

If the file doesn't exist yet, create it.

### 2. Add the MCP server

Open the file and add (or merge into) the `mcpServers` key:

```json
{
  "mcpServers": {
    "local-llm": {
      "command": "/absolute/path/to/your/project/node_modules/.bin/tsx",
      "args": [
        "--env-file=/absolute/path/to/your/project/.env",
        "/absolute/path/to/your/project/src/mcp/app.ts"
      ]
    }
  }
}
```

**Important:** Use **absolute paths** everywhere. Relative paths won't work because Claude Desktop doesn't launch from your project directory.

For example, on macOS this might look like:

```json
{
  "mcpServers": {
    "local-llm": {
      "command": "npx",
      "args": [
        "tsx",
        "--env-file=/Users/your-name-here/Desktop/projects/MCP/.env",
        "/Users/your-name-here/Desktop/projects/MCP/src/mcp/server.ts"
      ]
    }
  },
  "preferences": {
    "coworkScheduledTasksEnabled": true,
    "ccdScheduledTasksEnabled": true,
    "sidebarMode": "chat",
    "coworkWebSearchEnabled": true
  }
}
```

### 3. Restart Claude Desktop

After saving the config, **fully quit and reopen** Claude Desktop (not just close the window). You should see your tools appear in a new conversation.

---

## ChatGPT (remote only)

ChatGPT **does not support local stdio MCP servers**. It can only connect to MCP servers over the network (HTTPS). This means you'd need to:

1. Deploy your MCP server somewhere publicly accessible (e.g., a cloud VM, Railway, Fly.io), or expose it locally using a tunnel like [ngrok](https://ngrok.com) or Cloudflare Tunnel.
2. Modify your server to use an **HTTP/SSE transport** instead of stdio (the current `StdioServerTransport`).
3. In ChatGPT, go to **Settings → Connectors → Create**, enter your server's public HTTPS URL, and follow the prompts.

There is **no local config file** to edit — it's all done through the ChatGPT web UI.

### Requirements

- A **ChatGPT Pro, Plus, Business, Enterprise, or Education** account.
- **Developer Mode** enabled (Settings → Connectors → Advanced → Developer Mode).

### What would need to change in our code

Our server currently uses `StdioServerTransport`, which reads/writes over stdin/stdout. To work with ChatGPT, you'd swap that out for an HTTP-based transport, something like:

```typescript
// Instead of StdioServerTransport, you'd use an HTTP transport
// and run the server on a port that's publicly accessible
import { StreamableHTTPServerTransport } from "@modelcontextprotocol/sdk/server/streamableHttp.js";
```

This is a non-trivial change, so for now **Claude Desktop is the easiest way to test locally**.

---

## Gemini

Google's Gemini support for MCP depends on which Gemini product you're using:

### Gemini CLI (local, stdio — similar to Claude Desktop)

The [Gemini CLI](https://github.com/google-gemini/gemini-cli) is a terminal-based agent that supports local MCP servers, very similar to Claude Desktop. Configure it in `~/.gemini/settings.json`:

```json
{
  "mcpServers": {
    "local-llm": {
      "command": "/absolute/path/to/your/project/node_modules/.bin/tsx",
      "args": [
        "--env-file=/absolute/path/to/your/project/.env",
        "/absolute/path/to/your/project/src/mcp/app.ts"
      ]
    }
  }
}
```

Then launch `gemini` in your terminal and your tools will be available.

### Gemini Web App (gemini.google.com)

The Gemini web app **does not currently support connecting custom MCP servers**. MCP integration is only available through the Gemini CLI, the Gemini API/SDK (programmatically), or enterprise products like Gemini Enterprise with admin-managed connectors.

---

## Quick comparison

| Client         | Local Stdio? | Config Method                     | Plan Required                   | Extra Setup Needed?              |
| -------------- | ------------ | --------------------------------- | ------------------------------- | -------------------------------- |
| Claude Desktop | Yes          | Edit `claude_desktop_config.json` | Any (free works)                | None — works as-is               |
| Gemini CLI     | Yes          | Edit `~/.gemini/settings.json`    | Free (uses API key)             | Install Gemini CLI               |
| ChatGPT        | No           | Web UI (Settings → Connectors)    | Pro, Plus, Business, Enterprise | Deploy server over HTTPS         |
| Gemini Web App | No           | N/A                               | N/A                             | Not supported for custom servers |

---

## How it all works

Let's see the big picture of what's actually happening when you use this setup.

### Your local LLM must be running

Your MCP server is a middleman — it forwards prompts to a locally hosted LLM running in **LM Studio** (or Ollama). If LM Studio isn't running with a model loaded, the MCP tools will fail. Make sure your local LLM server is up and accepting requests before you start chatting.

### What MCP actually does

When you connect this MCP server to Claude (or Gemini CLI), you're not replacing Claude with your local LLM. You're giving Claude **new tools** it can call when it needs them. Think of it like giving Claude a phone it can use to call your local model for specific tasks.

Claude still handles the conversation. But when a task matches one of the tools you registered (like `ask_llm` or `get_pokemon_info`), Claude can choose to call that tool, get the result from your local LLM, and incorporate it into its response.

### When do the tools get used?

The AI client decides when to use your tools based on the conversation context. This can happen in two ways:

**Implicitly** — you ask something that naturally fits a tool, and the AI decides to use it on its own:

> **You:** "What type is Bulbasaur?"
>
> **Claude:** _(automatically calls `get_pokemon_info` behind the scenes, then responds with the result)_
> "Bulbasaur is a Grass/Poison type Pokémon..."

**Explicitly** — you directly ask the AI to use a specific tool:

> **You:** "Use the ask_llm tool to explain what a closure is in JavaScript."
>
> **Claude:** _(calls `ask_llm` with your prompt, gets the response from your local LLM, and relays it back)_

In both cases, Claude will typically show you which tool it called and what it returned, so you can see exactly what's happening under the hood.

---

## Troubleshooting

**"node: .env: not found"** — Create an empty `.env` file in the project root.

**"ERR_MODULE_NOT_FOUND"** — Double-check that the file path in your config matches the actual file. For example, `src/mcp/app.ts` vs `src/mcp/server.ts`. Also avoid symlinks to other project directories — they can cause stale module resolution paths.

**Server starts then immediately disconnects** — Make sure your server only writes MCP protocol messages to `stdout`. Any `console.log()` calls will corrupt the stdio stream. Use `console.error()` for debug logging instead.

**Tools don't appear after config change** — Fully quit and restart the client app (Claude Desktop, Gemini CLI). Just closing the window may not be enough.
