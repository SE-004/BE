import { z } from "zod";

const envSchema = z.object({
  MONGO_URI: z.string(),
  DB_NAME: z.string(),
  SALT_ROUNDS: z.coerce.number().default(10),
  ACCESS_JWT_SECRET: z.string().min(64),
  REFRESH_TOKEN_TTL: z.coerce.number().default(30 * 24 * 60 * 60),
  CLIENT_BASE_URL: z.string().default("http://localhost:5173"),
  PORT: z.coerce.number().default(8080),
});

const parsedEnv = envSchema.safeParse(process.env);

if (!parsedEnv.success) {
  console.error("your env file is messed up.", parsedEnv.error);
  process.exit(1);
}

export const {
  MONGO_URI,
  DB_NAME,
  SALT_ROUNDS,
  ACCESS_JWT_SECRET,
  REFRESH_TOKEN_TTL,
  CLIENT_BASE_URL,
  PORT,
} = parsedEnv.data;
