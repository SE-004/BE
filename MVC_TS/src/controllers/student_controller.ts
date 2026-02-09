import Student from "../schemas/Student";
import { Request, Response } from "express";

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
    const { id } = req.params;
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
