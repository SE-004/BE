import { randomUUID } from "node:crypto";

export function generateRefreshTokenString(): string {
  return randomUUID();
}
