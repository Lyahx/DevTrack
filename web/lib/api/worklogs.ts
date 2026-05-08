import { apiDelete, apiGet, apiPost, apiPut } from "@/lib/api";
import type { PagedResult } from "@/types/api";
import type {
  WorklogCreateRequest,
  WorklogListQuery,
  WorklogResponse,
  WorklogUpdateRequest,
} from "@/types/activity";

export const worklogsApi = {
  list: (q: WorklogListQuery = {}) =>
    apiGet<PagedResult<WorklogResponse>>("/worklogs", q as Record<string, unknown>),
  recent: (days = 7) => apiGet<WorklogResponse[]>("/worklogs/recent", { days }),
  get: (id: number, includeDeleted = false) =>
    apiGet<WorklogResponse>(`/worklogs/${id}`, { includeDeleted }),
  create: (req: WorklogCreateRequest) => apiPost<WorklogResponse>("/worklogs", req),
  update: (id: number, req: WorklogUpdateRequest) => apiPut<WorklogResponse>(`/worklogs/${id}`, req),
  remove: (id: number) => apiDelete<{ id: number }>(`/worklogs/${id}`),
  forProject: (id: number) => apiGet<WorklogResponse[]>(`/projects/${id}/worklogs`),
  forComponent: (id: number) => apiGet<WorklogResponse[]>(`/components/${id}/worklogs`),
  forTrack: (id: number) => apiGet<WorklogResponse[]>(`/learning-tracks/${id}/worklogs`),
  forModule: (id: number) => apiGet<WorklogResponse[]>(`/modules/${id}/worklogs`),
};
