import Student from "../schemas/Student";
import { Request, Response } from "express";
import mongoose from "mongoose";

// get all students
export const getAllStudents = async (req: Request, res: Response) => {
  try {
    const students = await Student.find();
    if (!students.length) {
      res.status(200).json({ msg: "No students in the DB" });
      return;
    }

    res.status(200).json({ students });
  } catch (error) {
    res.status(500).json(error);
  }
};

// get one student
export const getOneStudent = async (req: Request, res: Response) => {
  try {
    const id = req.params.id as string;

    if (!mongoose.Types.ObjectId.isValid(id)) {
      return res.status(400).json({ msg: "Invalid ID" });
    }

    const student = await Student.findById(id);
    if (student) {
      return res.status(200).json(student);
    }
    res.status(404).json({ msg: "Student not found" });
  } catch (error) {
    res.status(500).json(error);
  }
};

// create student
export const createStudent = async (req: Request, res: Response) => {
  try {
    const { firstName, lastName, email, age, courses } = req.body;
    if (!firstName || !lastName || !email || !age || !courses) {
      res.status(400).json({ msg: "All fields required" });
      return;
    }

    const student = await Student.create({
      firstName,
      lastName,
      email,
      age,
      courses,
    });
    res.status(201).json(student);
  } catch (error) {
    res.status(500).json(error);
  }
};

export const enrollStudentToCourse = async (req: Request, res: Response) => {
  // student ID ---- DONE
  // course ID ---- DONE
  // add course ID to student's 'courses' array
  // we can get the student ID from params and the course ID from body

  try {
    const studentID = req.params.id as string;
    const courseID = req.body.courseID;

    if (!courseID) {
      return res.status(400).json({ msg: "course ID missing" });
    }

    if (!studentID) {
      return res.status(400).json({ msg: "student ID missing" });
    }

    if (
      !mongoose.Types.ObjectId.isValid(courseID) ||
      !mongoose.Types.ObjectId.isValid(studentID)
    ) {
      return res.status(400).json({ msg: "Invalid ID" });
    }

    const existingStudent = await Student.findById(studentID);

    if (!existingStudent) {
      res.status(404).json({ msg: "student not found" });
      return;
    }

    // check if student is already in course
    if (existingStudent.courses.includes(courseID)) {
      res.status(409).json({ msg: "student already enrolled" });
      return;
    }

    await Student.findByIdAndUpdate(
      studentID,
      {
        $addToSet: { courses: courseID }, //keep any existing ID, adds it ONLY if its not included already
      },
      { new: true, runValidators: true }, // give me updated version back
    );
    res.status(200).json({ msg: "enrollment successful" });
  } catch (error) {
    res.status(500).json(error);
  }
};
