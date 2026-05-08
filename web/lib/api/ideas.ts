import { apiDelete, apiGet, apiPost, apiPut } from "@/lib/api";
import type { PagedResult } from "@/types/api";
import type {
  IdeaConvertRequest,
  IdeaCreateRequest,
  IdeaListQuery,
  IdeaResponse,
  IdeaUpdateRequest,
  NextStepResponse,
} from "@/types/activity";

export const ideasApi = {
  list: (q: IdeaListQuery = {}) =>
    apiGet<PagedResult<IdeaResponse>>("/ideas", q as Record<string, unknown>),
  get: (id: number) => apiGet<IdeaResponse>(`/ideas/${id}`),
  create: (req: IdeaCreateRequest) => apiPost<IdeaResponse>("/ideas", req),
  update: (id: number, req: IdeaUpdateRequest) => apiPut<IdeaResponse>(`/ideas/${id}`, req),
  remove: (id: number) => apiDelete<{ id: number }>(`/ideas/${id}`),
  convert: (id: number, req: IdeaConvertRequest) =>
    apiPost<NextStepResponse>(`/ideas/${id}/convert-to-next-step`, req),
  forProject: (id: number) => apiGet<IdeaResponse[]>(`/projects/${id}/ideas`),
  forComponent: (id: number) => apiGet<IdeaResponse[]>(`/components/${id}/ideas`),
  forTrack: (id: number) => apiGet<IdeaResponse[]>(`/learning-tracks/${id}/ideas`),
  forModule: (id: number) => apiGet<IdeaResponse[]>(`/modules/${id}/ideas`),
};
