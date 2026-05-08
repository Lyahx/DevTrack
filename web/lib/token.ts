const KEY = "devtrack_token";

export function readToken(): string | null {
  if (typeof window === "undefined") return null;
  return window.localStorage.getItem(KEY);
}

export function writeToken(token: string) {
  if (typeof window === "undefined") return;
  window.localStorage.setItem(KEY, token);
}

export function clearToken() {
  if (typeof window === "undefined") return;
  window.localStorage.removeItem(KEY);
}
