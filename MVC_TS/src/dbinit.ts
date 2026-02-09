import mongoose from "mongoose";

const connectDB = async () => {
  const conn = await mongoose.connect(process.env.MONGO_URI as string);
  console.log(`MongoDB successfully connected: ${conn.connection.name}`);
};

export default connectDB;
