import { lmsController } from "controllers/openAI";
import { createCompletion } from "controllers/completions";
import express from "express";
import { agentController } from "controllers/agent";

const localAIRouter = express.Router();

localAIRouter.post("/lms", lmsController);
localAIRouter.post("/chained-prompt", createCompletion);
localAIRouter.post("/agent", agentController);

export default localAIRouter;
