import express from "express";

import {
  getAllCourses,
  getOneCourse,
  createCourse,
  getCourseEnrollments,
} from "../controllers/course_controllers";

const course = express.Router();

course.route("/").get(getAllCourses).post(createCourse);
course.route("/:id").get(getOneCourse);
course.route("/count/:id").get(getCourseEnrollments);

export default course;
