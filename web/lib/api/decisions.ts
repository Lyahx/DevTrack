import { apiDelete, apiGet, apiPost, apiPut } from "@/lib/api";
import type { PagedResult } from "@/types/api";
import type {
  DecisionCreateRequest,
  DecisionListQuery,
  DecisionResponse,
  DecisionUpdateRequest,
} from "@/types/activity";

export const decisionsApi = {
  list: (q: DecisionListQuery = {}) =>
    apiGet<PagedResult<DecisionResponse>>("/decisions", q as Record<string, unknown>),
  get: (id: number) => apiGet<DecisionResponse>(`/decisions/${id}`),
  create: (req: DecisionCreateRequest) => apiPost<DecisionResponse>("/decisions", req),
  update: (id: number, req: DecisionUpdateRequest) => apiPut<DecisionResponse>(`/decisions/${id}`, req),
  remove: (id: number) => apiDelete<{ id: number }>(`/decisions/${id}`),
  forProject: (id: number) => apiGet<DecisionResponse[]>(`/projects/${id}/decisions`),
  forComponent: (id: number) => apiGet<DecisionResponse[]>(`/components/${id}/decisions`),
  forTrack: (id: number) => apiGet<DecisionResponse[]>(`/learning-tracks/${id}/decisions`),
  forModule: (id: number) => apiGet<DecisionResponse[]>(`/modules/${id}/decisions`),
};
