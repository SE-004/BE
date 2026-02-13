import { Request, Response, NextFunction } from "express";

const isMaintenanceMode =
  process.env.IS_MAINTENANCE_MODE === "true" ? true : false;

const maintenance = (req: Request, res: Response, next: NextFunction) => {
  // check if we're in maintenance mode
  if (isMaintenanceMode) {
    // allow reads
    if (req.method === "GET") {
      next();
      return;
    }

    // block everything else
    res.status(503).json({
      msg: "Service Unavailable: We are currently updating the db. Read-only mode activated.",
    });
  } else {
    // business as usual
    next();
  }
};

export default maintenance;
