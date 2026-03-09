import express, { type Application } from "express";
import AIRouter from "routers/openAI";
import localAIRouter from "routers/openAI";

const app: Application = express();
const PORT = process.env.PORT || 8000;

app.use(express.json());

app.get("/", (_req, res) => {
  res.send("Testing AI");
});

app.use("/ai", localAIRouter);
app.listen(PORT, () => {
  console.log(`Example app listening on port ${PORT}`);
});
