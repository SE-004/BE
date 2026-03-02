import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";

type User = {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  roles: string[];
};

const AUTH_URL = import.meta.env.VITE_APP_TRAVEL_JOURNAL_AUTH_URL as
  | string
  | undefined;

if (!AUTH_URL)
  throw new Error(
    "AUTH_URL is required. Did you maybe forget to add it to the .env.development.local? :(",
  );

type AuthContextValue = {
  user: User | null;
  loading: boolean;
  setUser: (user: User | null) => void;
  logout: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let alive = true;
    (async () => {
      try {
        const res = await fetch(`${AUTH_URL}/auth/me`, {
          method: "GET",
          credentials: "include",
        });

        if (!res.ok) {
          if (alive) setUser(null);
          return;
        }
        const data = await res.json();
        if (alive) setUser(data.user);
      } catch {
        if (alive) setUser(null);
      } finally {
        if (alive) setLoading(false);
      }
    })();

    return () => {
      alive = false;
    };
  }, []);

  const logout = async () => {
    try {
      await fetch(`${AUTH_URL}/auth/logout`, {
        method: "DELETE",
        credentials: "include",
      });
    } catch {
      // ignore network error
    } finally {
      setUser(null);
    }
  };

  return (
    <AuthContext.Provider value={{ user, setUser, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within the <AuthProvider>");
  return ctx;
}
