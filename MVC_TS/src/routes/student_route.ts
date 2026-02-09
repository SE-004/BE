import express from "express";

import {
  getAllStudents,
  getOneStudent,
  createStudent,
} from "../controllers/student_controller";

const student = express.Router();

student.route("/").get(getAllStudents).post(createStudent);
student.route("/:id").get(getOneStudent);

export default student;
