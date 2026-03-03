import { Schema, model, Types } from "mongoose";
import { REFRESH_TOKEN_TTL } from "#config";

const refreshTokenSchema = new Schema(
  {
    token: {
      type: String,
      required: true,
      unique: true,
    },
    userId: {
      type: Types.ObjectId,
      ref: "User",
      required: true,
    },
    expireAt: {
      type: Date,
      default: () => new Date(Date.now() + REFRESH_TOKEN_TTL * 1000),
      expires: REFRESH_TOKEN_TTL,
    },
  },
  {
    timestamps: { createdAt: true, updatedAt: false },
    versionKey: false,
  },
);

const RefreshToken = model("RefreshToken", refreshTokenSchema);

export default RefreshToken;
