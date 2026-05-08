import { apiDelete, apiGet, apiPost, apiPut } from "@/lib/api";
import type { PagedResult } from "@/types/api";
import type {
  NextStepCreateRequest,
  NextStepListQuery,
  NextStepResponse,
  NextStepUpdateRequest,
} from "@/types/activity";

export const nextStepsApi = {
  list: (q: NextStepListQuery = {}) =>
    apiGet<PagedResult<NextStepResponse>>("/next-steps", q as Record<string, unknown>),
  open: () => apiGet<NextStepResponse[]>("/next-steps/open"),
  get: (id: number) => apiGet<NextStepResponse>(`/next-steps/${id}`),
  create: (req: NextStepCreateRequest) => apiPost<NextStepResponse>("/next-steps", req),
  update: (id: number, req: NextStepUpdateRequest) => apiPut<NextStepResponse>(`/next-steps/${id}`, req),
  remove: (id: number) => apiDelete<{ id: number }>(`/next-steps/${id}`),
  complete: (id: number) => apiPut<NextStepResponse>(`/next-steps/${id}/complete`),
  uncomplete: (id: number) => apiPut<NextStepResponse>(`/next-steps/${id}/uncomplete`),
  forProject: (id: number) => apiGet<NextStepResponse[]>(`/projects/${id}/next-steps`),
  forComponent: (id: number) => apiGet<NextStepResponse[]>(`/components/${id}/next-steps`),
  forTrack: (id: number) => apiGet<NextStepResponse[]>(`/learning-tracks/${id}/next-steps`),
  forModule: (id: number) => apiGet<NextStepResponse[]>(`/modules/${id}/next-steps`),
};
