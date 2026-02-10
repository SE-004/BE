import Course from "../schemas/Course";
import { Request, Response } from "express";
import mongoose from "mongoose";
import Student from "../schemas/Student";

// get all courses
export const getAllCourses = async (req: Request, res: Response) => {
  try {
    const courses = await Course.find();
    if (!courses.length) {
      res.status(200).json({ msg: "No courses in the DB" });
      return;
    }

    res.status(200).json({ courses });
  } catch (error) {
    res.status(500).json(error);
  }
};

// get one course
export const getOneCourse = async (req: Request, res: Response) => {
  try {
    // retrieving the id from the params
    const id = req.params.id as string;

    // making sure that the id is a valid mongoDB ID by using mongoose's inbuilt 'isValid' method
    if (!mongoose.Types.ObjectId.isValid(id)) {
      return res.status(400).json({ msg: "Invalid ID" });
    }

    const course = await Course.findById(id);
    if (course) {
      return res.status(200).json(course);
    }
    res.status(404).json({ msg: "Course not found" });
  } catch (error) {
    res.status(500).json(error);
  }
};

// create course
export const createCourse = async (req: Request, res: Response) => {
  try {
    const { title, description, credits } = req.body;
    if (!title || !description || !credits) {
      res.status(400).json({ msg: "All fields required" });
      return;
    }

    const course = await Course.create({ title, description, credits });
    res.status(201).json(course);
  } catch (error) {
    res.status(500).json(error);
  }
};

export const getCourseEnrollments = async (req: Request, res: Response) => {
  try {
    // get course ID
    // find all students where the 'courses' array includes courseID

    const courseID = req.params.id as string;
    const count = await Student.countDocuments({ courses: courseID });

    res.status(200).json({ courseID, enrolled_students: count });
  } catch (error) {
    res.status(500).json(error);
  }
};
