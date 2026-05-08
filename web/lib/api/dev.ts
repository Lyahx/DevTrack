import { apiPost } from "@/lib/api";
import type { AuthResponse } from "@/types/auth";

export type SeedResult = {
  projects: number;
  components: number;
  learningTracks: number;
  learningModules: number;
  tags: number;
  worklogs: number;
  decisions: number;
  nextSteps: number;
  ideas: number;
  resources: number;
  reminders: number;
};

export const devApi = {
  quickLogin: () => apiPost<AuthResponse>("/dev/quick-login"),
  seed: () => apiPost<SeedResult>("/dev/seed"),
  wipe: () => apiPost<{ rowsDeleted: number }>("/dev/wipe"),
};
