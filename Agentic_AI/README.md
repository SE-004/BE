# The agent loop pattern

## What makes this "agentic"?

Compare the three endpoints in this project — they represent an escalating scale of AI autonomy:

### `POST /ai/lms` — basic completion

One prompt in, one response out. No tools, no decisions. This is a stateless function call.

### `POST /ai/chained-prompt` — chained prompts

Multiple LLM calls in sequence (intent detection → data fetch → structured response), but the _code_ decides the control flow. The LLM just fills in blanks at each step.

### `POST /ai/agent` — agent loop

The LLM itself decides what to do next. It sees a set of available tools, chooses which to call (and in what order), inspects the results, and loops until it has enough information to produce a final answer. The code just executes whatever the LLM asks for.

That shift — from _code controls the flow_ to _the LLM controls the flow_ — is what makes it agentic.

## How the loop works

```
User: "What Pokémon should I bring to Hamburg today?"
                    │
                    ▼
        ┌───────────────────────┐
        │   LLM sees the prompt │
        │   + tool descriptions │
        └──────────┬────────────┘
                   │
                   ▼
        ┌───────────────────────┐
        │  LLM decides:         │
        │  "I need the weather" │
        │                       │
        │  → tool_call:         │
        │ get_weather("Hamburg")│
        └──────────┬────────────┘
                   │
                   ▼
        ┌───────────────────────┐
        │  Code executes tool   │
        │  Result: 12°C, rainy  │
        │  → fed back to LLM    │
        └──────────┬────────────┘
                   │
                   ▼
        ┌───────────────────────┐
        │  LLM decides:         │
        │  "Rainy → Water type" │
        │  "Let me look up      │
        │   Vaporeon"           │
        │                       │
        │  → tool_call:         │
        │    get_pokemon_info(  │
        │      "Vaporeon")      │
        └──────────┬────────────┘
                   │
                   ▼
        ┌───────────────────────┐
        │  Code executes tool   │
        │  Result: Vaporeon     │
        │  stats, types, etc.   │
        │  → fed back to LLM    │
        └──────────┬────────────┘
                   │
                   ▼
        ┌───────────────────────┐
        │  LLM decides:         │
        │  "I have everything   │
        │   I need."            │
        │                       │
        │  → final text answer  │
        │    with reasoning     │
        └───────────────────────┘
```

The critical thing to notice: the code never says "call weather first, then call Pokémon." The LLM figures that out on its own. If you asked a different question — say "tell me about Pikachu" — it would skip the weather tool entirely and only call `get_pokemon_info`.

## Key code concepts

### Tool descriptions tell the LLM what's available

```typescript
const tools: ChatCompletionTool[] = [
  {
    type: "function",
    function: {
      name: "get_weather",
      description: "Get the current weather for a given city.",
      parameters: {
        /* JSON Schema */
      },
    },
  },
  // ...
];
```

The LLM never calls these functions directly. It returns a structured `tool_calls` array saying _"I want to call get_weather with city='Hamburg'"_, and our code actually executes it.

### The loop checks `finish_reason` / `tool_calls`

```typescript
if (message.tool_calls?.length) {
  // LLM wants tools → execute them, feed results back, continue loop
} else {
  // No tool calls → this is the final answer, break out
}
```

### Tool results go back as `role: "tool"` messages

```typescript
messages.push({
  role: "tool",
  tool_call_id: toolCall.id,
  content: result,
});
```

This is how the LLM "sees" what the tool returned, so it can reason about it in the next iteration.

## Testing it

For simplicity's sake, use Postman for testing, but here's the curl command as well:

```bash
curl -X POST http://localhost:8000/ai/agent
  -H "Content-Type: application/json"
  -d '{"prompt": "What Pokémon should I bring to Hamburg today?"}'
```

The response includes both the final `answer` and a `steps` array showing every tool call and result, so you can trace the agent's reasoning.

Example prompts to try:

- "What Pokémon would thrive in the current weather in Tokyo?"
- "I'm visiting Reykjavik, which Pokémon should I take?"
- "Tell me about Pikachu" (should skip the weather tool entirely)

## What this is missing (and why that's okay for a demo)

A production agent would also have: persistent memory across requests, more sophisticated planning (breaking complex goals into sub-goals), error recovery and retry logic, and guardrails to prevent hallucinated tool calls. But this demo captures the fundamental pattern: **LLM in a loop, choosing tools, until it's done.**
