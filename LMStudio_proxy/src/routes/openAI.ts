import { createCompletion } from "controllers/completions";
import { lmsController } from "controllers/openAI";
import express from "express";

const localAIRouter = express.Router();

localAIRouter.post("/lms", lmsController);
localAIRouter.post("/chained-prompt", createCompletion);

export default localAIRouter;
