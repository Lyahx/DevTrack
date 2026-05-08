import { apiGet } from "@/lib/api";
import type { DashboardResponse } from "@/types/dashboard";

export const dashboardApi = {
  get: () => apiGet<DashboardResponse>("/dashboard"),
};
