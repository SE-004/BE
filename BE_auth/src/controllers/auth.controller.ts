import bcrypt from "bcrypt";
import { z } from "zod";
import { User, RefreshToken } from "#models";
import {
  createAccessToken,
  verifyAccessToken,
  generateRefreshTokenString,
} from "#utils";
import { REFRESH_TOKEN_TTL, SALT_ROUNDS } from "#config";
import { loginSchema, registerSchema } from "#schemas";
import type { RequestHandler } from "express";

type RegisterBody = z.infer<typeof registerSchema>;
type LoginBody = z.infer<typeof loginSchema>;

const ACCESS_COOKIE_OPTIONS = {
  httpOnly: true,
  sameSite: "strict" as const,
  secure: process.env.NODE_ENV === "production",
  maxAge: 15 * 60 * 1000,
};

const REFRESH_COOKIE_OPTION = {
  httpOnly: true,
  sameSite: "strict" as const,
  secure: process.env.NODE_ENV === "production",
  maxAge: REFRESH_TOKEN_TTL * 1000,
};

export const register: RequestHandler = async (req, res, next) => {
  try {
    const { email, password, firstName, lastName } = req.body as Omit<
      RegisterBody,
      "confirmPassword"
    >;

    const existing = await User.findOne({ email }).lean();

    if (existing) {
      const err = new Error("email already in use");
      (err as any).statusCode = 409;
      throw err;
    }

    const passwordHash = await bcrypt.hash(password, SALT_ROUNDS);

    const newUser = await User.create({
      email,
      password: passwordHash,
      roles: ["user"],
      ...(firstName && { firstName }),
      ...(lastName && { lastName }),
    });

    const accessToken = createAccessToken({
      userId: newUser._id.toString(),
      roles: newUser.roles,
    });
    const refreshToken = generateRefreshTokenString();

    await RefreshToken.create({ token: refreshToken, userId: newUser._id });

    res.cookie("accessToken", accessToken, ACCESS_COOKIE_OPTIONS);
    res.cookie("refreshToken", refreshToken, REFRESH_COOKIE_OPTION);

    res.status(201).json({
      message: "user registered",
      user: {
        id: newUser._id,
        email: newUser.email,
        firstName: newUser.firstName,
        lastName: newUser.lastName,
        roles: newUser.roles,
      },
    });
  } catch (error) {
    next(error);
  }
};

export const login: RequestHandler = async (req, res, next) => {
  try {
    const { email, password } = req.body as LoginBody;

    const user = await User.findOne({ email });

    if (!user) {
      const err = new Error("Incorrect credentials");
      (err as any).statusCode = 401;
      throw err;
    }

    const ok = await bcrypt.compare(password, user.password);

    if (!ok) {
      const err = new Error("Incorrect credentials");
      (err as any).statusCode = 401;
      throw err;
    }

    const accessToken = createAccessToken({
      userId: user._id.toString(),
      roles: user.roles,
    });
    const refreshToken = generateRefreshTokenString();

    await RefreshToken.create({
      token: refreshToken,
      userId: user._id,
    });

    res.cookie("accessToken", accessToken, ACCESS_COOKIE_OPTIONS);
    res.cookie("refreshToken", refreshToken, REFRESH_COOKIE_OPTION);

    res.status(200).json({
      message: "Logged in",
      user: {
        id: user._id,
        email: user.email,
        firstName: user.firstName,
        lastName: user.lastName,
        roles: user.roles,
      },
    });
  } catch (error) {
    next(error);
  }
};

export const refresh: RequestHandler = async (req, res, next) => {
  try {
    const oldRefreshToken = req.cookies?.refreshToken;

    if (!oldRefreshToken) {
      const err = new Error("No refresh token found");
      (err as any).statusCode = 401;
      throw err;
    }

    const existing = await RefreshToken.findOne({ token: oldRefreshToken });
    if (!existing) {
      const err = new Error("Invalid refresh token");
      (err as any).statusCode = 401;
      throw err;
    }

    const user = await User.findById(existing.userId);

    if (!user) {
      const err = new Error("Invalid session");
      (err as any).statusCode = 401;
      throw err;
    }

    await RefreshToken.deleteOne({ _id: existing._id });

    const newAccessToken = createAccessToken({
      userId: user._id.toString(),
      roles: user.roles,
    });
    const newRefreshToken = generateRefreshTokenString();

    await RefreshToken.create({ token: newRefreshToken, userId: user._id });

    res.cookie("accessToken", newAccessToken, ACCESS_COOKIE_OPTIONS);
    res.cookie("refreshToken", newRefreshToken, REFRESH_COOKIE_OPTION);

    res.status(200).json({ message: "Tokens refreshed" });
  } catch (error) {
    next(error);
  }
};

export const logout: RequestHandler = async (req, res, next) => {
  try {
    const refreshTokenCookie = req.cookies?.refreshToken;

    if (refreshTokenCookie) {
      await RefreshToken.deleteOne({ token: refreshTokenCookie });
    }

    res.clearCookie("accessToken", ACCESS_COOKIE_OPTIONS);
    res.clearCookie("refreshToken", REFRESH_COOKIE_OPTION);

    res.status(200).json({ message: "Logged out" });
  } catch (error) {
    next(error);
  }
};

export const me: RequestHandler = async (req, res, next) => {
  try {
    const accessToken = req.cookies?.accessToken;
    if (!accessToken) {
      const err = new Error("Not authenticated");
      (err as any).statusCode = 401;
      throw err;
    }

    let decoded: { userId: string; roles: string[] };

    try {
      decoded = verifyAccessToken(accessToken);
    } catch (tokenErr: any) {
      if (tokenErr && tokenErr.name === "TokenExpiredError") {
        res.setHeader("WWW-Authenticate", "token_expired");
        res.status(401).json({ error: "Access token expired" });
        return;
      }

      const err = new Error("Invalid token");
      (err as any).statusCode = 401;
      throw err;
    }

    const user = await User.findById(decoded.userId).lean();
    if (!user) {
      const err = new Error("User not found");
      (err as any).statusCode = 404;
      throw err;
    }

    res.status(200).json({
      user: {
        id: user._id,
        email: user.email,
        firstName: user.firstName,
        lastName: user.lastName,
        roles: user.roles,
      },
    });
  } catch (error) {
    next(error);
  }
};
