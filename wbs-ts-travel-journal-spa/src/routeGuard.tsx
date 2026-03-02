import type { ReactNode } from "react";
import { useAuth } from "./context/AuthContext";
import { Navigate } from "react-router";

export function RequireAuth({ children }: { children: ReactNode }) {
  const { user, loading } = useAuth();

  if (loading) return null;

  if (!user) return <Navigate to="/login" replace />;

  return <>{children}</>;
}

export function RequireGuest({ children }: { children: ReactNode }) {
  const { user, loading } = useAuth();
  if (loading) return null;

  if (user) return <Navigate to="/" replace />;

  return <>{children}</>;
}
