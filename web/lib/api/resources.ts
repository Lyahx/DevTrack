import { apiDelete, apiGet, apiPost, apiPut } from "@/lib/api";
import type { PagedResult } from "@/types/api";
import type {
  ResourceCreateRequest,
  ResourceListQuery,
  ResourceResponse,
  ResourceUpdateRequest,
} from "@/types/activity";

export const resourcesApi = {
  list: (q: ResourceListQuery = {}) =>
    apiGet<PagedResult<ResourceResponse>>("/resources", q as Record<string, unknown>),
  get: (id: number) => apiGet<ResourceResponse>(`/resources/${id}`),
  create: (req: ResourceCreateRequest) => apiPost<ResourceResponse>("/resources", req),
  update: (id: number, req: ResourceUpdateRequest) => apiPut<ResourceResponse>(`/resources/${id}`, req),
  remove: (id: number) => apiDelete<{ id: number }>(`/resources/${id}`),
  forProject: (id: number) => apiGet<ResourceResponse[]>(`/projects/${id}/resources`),
  forComponent: (id: number) => apiGet<ResourceResponse[]>(`/components/${id}/resources`),
  forTrack: (id: number) => apiGet<ResourceResponse[]>(`/learning-tracks/${id}/resources`),
  forModule: (id: number) => apiGet<ResourceResponse[]>(`/modules/${id}/resources`),
};
