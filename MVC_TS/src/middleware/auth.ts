import { Request, Response, NextFunction } from "express";

const checkApiKey = (req: Request, res: Response, next: NextFunction) => {
  const apiKey = req.headers["x-api-key"];

  if (apiKey === "my-super-secret-password") {
    next();
  } else {
    res.status(403).json({ msg: "Access denied: Invalid API key. Shoo." });
  }
};

export default checkApiKey;
