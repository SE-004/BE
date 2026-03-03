import { error } from "node:console";
import { email, z } from "zod";

const emailSchema = z.email({ error: "Please provide a valid email." });
const passwordSchema = z
  .string({ error: "Password must be a string" })
  .min(12, { error: "Password must be at least 12 chars" })
  .max(512, { error: "Chill. that's too long" });

export const registerSchema = z
  .object(
    {
      email: emailSchema,
      password: passwordSchema
        .regex(/[a-z]/, {
          error: "Password must include at least one lower case char",
        })
        .regex(/[A-Z]/, {
          error: "Password must include at least one upper case char",
        })
        .regex(/[0-9]/, { error: "Password must include at least one number" })
        .regex(/[^a-zA-Z0-9]/, {
          error: "Password must include at least one special char",
        }),
      confirmPassword: z.string(),
      firstName: z.string().min(2).max(50).optional(),
      lastName: z.string().min(2).max(50).optional(),
    },
    { error: "Please provide a valid email and a secure password" },
  )
  .strict()
  .refine((data) => data.password === data.confirmPassword, {
    error: "Passwords do not match",
  });

export const loginSchema = z.object({
  email: emailSchema,
  password: passwordSchema,
});
