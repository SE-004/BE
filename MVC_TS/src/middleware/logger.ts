import { Request, Response, NextFunction } from "express";

const logger = (req: Request, res: Response, next: NextFunction) => {
  console.log(
    `${new Date().toLocaleTimeString()} | request method: ${req.method} | path: ${req.url}`,
  );

  next();
};

export default logger;
