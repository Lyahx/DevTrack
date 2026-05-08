import { ApiClientError } from "./api";

export function errorMessage(e: unknown): string {
  if (e instanceof ApiClientError) return e.message;
  if (e instanceof Error) return e.message;
  return "Bilinmeyen hata.";
}
