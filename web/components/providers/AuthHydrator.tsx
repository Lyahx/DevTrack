"use client";

import { useEffect } from "react";
import { authApi } from "@/lib/api/auth";
import { readToken } from "@/lib/token";
import { useAuthStore } from "@/store/auth";

export function AuthHydrator({ children }: { children: React.ReactNode }) {
  const setAuthenticated = useAuthStore((s) => s.setAuthenticated);
  const setUnauthenticated = useAuthStore((s) => s.setUnauthenticated);

  useEffect(() => {
    const token = readToken();
    if (!token) {
      setUnauthenticated();
      return;
    }
    authApi
      .me()
      .then((user) => setAuthenticated(user))
      .catch(() => setUnauthenticated());
  }, [setAuthenticated, setUnauthenticated]);

  return <>{children}</>;
}
