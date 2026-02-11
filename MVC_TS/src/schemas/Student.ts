import mongoose, { Schema, Document } from "mongoose";

export interface IStudent extends Document {
  firstName: string;
  lastName: string;
  email: string;
  age: number;
  courses: mongoose.Types.ObjectId[];
  createdAt: Date;
  updatedAt: Date;
}

const StudentSchema: Schema = new Schema(
  {
    firstName: {
      type: String,
      required: true,
      minLength: [2, "min length is 2 chars"],
      maxLength: 50,
    },
    lastName: {
      type: String,
      required: true,
      minLength: [2, "min length is 2 chars"],
      maxLength: 50,
    },
    email: {
      type: String,
      required: true,
      match: [
        /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/,
        "Please use a valid email",
      ],
    },
    age: {
      type: Number,
      required: true,
    },
    courses: [{ type: mongoose.Schema.Types.ObjectId, ref: "Course" }],
  },
  { timestamps: true },
);

export default mongoose.model<IStudent>("Student", StudentSchema);
