import { apiGet } from "@/lib/api";
import type { CommitListResponse } from "@/types/commit";

export const commitsApi = {
  forProject: (projectId: number, limit = 10) =>
    apiGet<CommitListResponse>(`/projects/${projectId}/commits`, { limit }),
};
