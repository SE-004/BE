import { email, z } from "zod";

export const createStudentSchema = z.object({
  body: z.object({
    firstName: z
      .string()
      .min(2, "First name must be at least 2 chars")
      .max(50, "First name cannot be more than 50 chars"),
    lastName: z
      .string()
      .min(2, "Last name must be at least 2 chars")
      .max(50, "Last name cannot be more than 50 chars"),
    email: z.email("Invalid email format"),
    age: z.number(),
    courses: z.array(z.string().optional()), // we want to have the array, BUT it can be empty
  }),
});

export type CreateStudentInput = z.infer<typeof createStudentSchema>;
