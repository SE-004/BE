import express from "express";
import cors from "cors";
import cookieParser from "cookie-parser";
import { connectDB } from "./db.js";
import { authRouter } from "#routes";
import { errorHandler, notFoundHandler } from "#middlewares";
import { CLIENT_BASE_URL, PORT } from "#config";

const app = express();

connectDB();

app.use(
  cors({
    origin: CLIENT_BASE_URL,
    credentials: true,
    exposedHeaders: ["WWW-Authenticate"],
  }),
);
app.use(express.json());
app.use(cookieParser());

app.get("/", (_req, res) => {
  res.send("Welcome to my Auth API");
});

app.use("/auth", authRouter);

app.use(notFoundHandler);
app.use(errorHandler);

app.listen(PORT, () => {
  console.log(`Auth server running on port ${PORT}`);
});
