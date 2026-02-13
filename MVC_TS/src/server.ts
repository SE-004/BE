import express, { Request, Response, Application, NextFunction } from "express";
import connectDB from "./dbinit";
import cors from "cors";
import student from "./routes/student_route";
import course from "./routes/course_route";
import logger from "./middleware/logger";
import morgan from "morgan";
import maintenance from "./middleware/maintenance";
import "colors";

const app: Application = express();
const PORT = process.env.PORT || 8000;

// Application level middleware

// Route level middleware

// Third-party middleware

connectDB();

app.use(express.json());
app.use(cors());

app.use(morgan("dev"));
app.use(maintenance);

// app.use(logger);

app.get("/", (req: Request, res: Response) => {
  res.send("Hello World!");
});

app.use("/api/students", student);
app.use("/api/courses", course);

app.listen(PORT, () => {
  if (process.env.IS_MAINTENANCE_MODE === "true") {
    console.log(`!!! MAINTENANCE MODE ACTIVE !!!`.black.bgYellow.bold);
    console.log(`Server is running on port ${PORT}`.yellow);
    console.log("writes are BLOCKED. Reads are ALLOWED".yellow);
  } else {
    console.log(`Example app listening on port ${PORT}`.bgGreen.black);
  }
});
