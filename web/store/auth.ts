import { create } from "zustand";
import type { UserResponse } from "@/types/auth";
import { clearToken, writeToken } from "@/lib/token";

type Status = "loading" | "authenticated" | "unauthenticated";

type AuthState = {
  status: Status;
  user: UserResponse | null;
  setAuthenticated: (user: UserResponse, token?: string) => void;
  setUnauthenticated: () => void;
  setLoading: () => void;
  logout: () => void;
};

export const useAuthStore = create<AuthState>((set) => ({
  status: "loading",
  user: null,
  setAuthenticated: (user, token) => {
    if (token) writeToken(token);
    set({ status: "authenticated", user });
  },
  setUnauthenticated: () => set({ status: "unauthenticated", user: null }),
  setLoading: () => set({ status: "loading" }),
  logout: () => {
    clearToken();
    set({ status: "unauthenticated", user: null });
  },
}));
