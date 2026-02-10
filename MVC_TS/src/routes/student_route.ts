import express from "express";

import {
  getAllStudents,
  getOneStudent,
  createStudent,
  enrollStudentToCourse,
} from "../controllers/student_controller";

const student = express.Router();

student.route("/").get(getAllStudents).post(createStudent);
student.route("/:id").get(getOneStudent);
student.route("/enroll/:id").post(enrollStudentToCourse);

export default student;
