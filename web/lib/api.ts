import axios, { AxiosError } from "axios";
import type { ApiResponse } from "@/types/api";
import { clearToken, readToken } from "./token";

const baseURL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:8080/api/v1";

export const api = axios.create({
  baseURL,
  headers: { "Content-Type": "application/json" },
});

api.interceptors.request.use((config) => {
  const token = readToken();
  if (token) {
    config.headers = config.headers ?? {};
    (config.headers as Record<string, string>)["Authorization"] = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (resp) => resp,
  (error: AxiosError<ApiResponse<unknown>>) => {
    if (error.response?.status === 401) {
      clearToken();
      if (typeof window !== "undefined" && !window.location.pathname.startsWith("/login")) {
        window.location.assign("/login");
      }
    }
    return Promise.reject(error);
  },
);

export class ApiClientError extends Error {
  code: string;
  details?: Record<string, string[]>;
  constructor(code: string, message: string, details?: Record<string, string[]>) {
    super(message);
    this.code = code;
    this.details = details;
  }
}

export async function request<T>(method: "get" | "post" | "put" | "delete", url: string, body?: unknown, params?: Record<string, unknown>): Promise<T> {
  try {
    const resp = await api.request<ApiResponse<T>>({ method, url, data: body, params });
    if (!resp.data?.success) {
      const err = resp.data?.error;
      throw new ApiClientError(err?.code ?? "UNKNOWN", err?.message ?? "Bir hata oluştu.", err?.details);
    }
    return resp.data.data as T;
  } catch (e) {
    if (e instanceof ApiClientError) throw e;
    if (axios.isAxiosError(e)) {
      const env = e.response?.data as ApiResponse<unknown> | undefined;
      if (env?.error) throw new ApiClientError(env.error.code, env.error.message, env.error.details);
      throw new ApiClientError("NETWORK", e.message);
    }
    throw e;
  }
}

export const apiGet = <T>(url: string, params?: Record<string, unknown>) => request<T>("get", url, undefined, params);
export const apiPost = <T>(url: string, body?: unknown) => request<T>("post", url, body);
export const apiPut = <T>(url: string, body?: unknown) => request<T>("put", url, body);
export const apiDelete = <T>(url: string) => request<T>("delete", url);
