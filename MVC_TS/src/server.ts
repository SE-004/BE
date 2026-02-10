import dotenv from "dotenv";
dotenv.config();
import express, { Request, Response, Application } from "express";
import connectDB from "./dbinit";
import cors from "cors";
import student from "./routes/student_route";
import course from "./routes/course_route";

const app: Application = express();
const PORT = process.env.PORT || 8000;

connectDB();

app.use(express.json());
app.use(cors());

app.get("/", (req: Request, res: Response) => {
  res.send("Hello World!");
});

app.use("/api/students", student);
app.use("/api/courses", course);

app.listen(PORT, () => {
  console.log(`Example app listening on port ${PORT}`);
});
