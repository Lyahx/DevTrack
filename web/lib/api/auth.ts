import { apiGet, apiPost } from "@/lib/api";
import type { AuthResponse, LoginRequest, RegisterRequest, UserResponse } from "@/types/auth";

export const authApi = {
  register: (req: RegisterRequest) => apiPost<UserResponse>("/auth/register", req),
  login: (req: LoginRequest) => apiPost<AuthResponse>("/auth/login", req),
  me: () => apiGet<UserResponse>("/auth/me"),
};
