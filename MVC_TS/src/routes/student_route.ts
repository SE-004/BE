import express from "express";

import {
  getAllStudents,
  getOneStudent,
  createStudent,
  enrollStudentToCourse,
} from "../controllers/student_controller";
import checkApiKey from "../middleware/auth";

const student = express.Router();

student.route("/").get(getAllStudents).post(checkApiKey, createStudent);
student.route("/:id").get(getOneStudent);
student.route("/enroll/:id").post(enrollStudentToCourse);

export default student;
