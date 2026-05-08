import { apiDelete, apiGet, apiPost, apiPut } from "@/lib/api";
import type {
  ComponentCreateRequest,
  ComponentResponse,
  ComponentStatusNoteRequest,
  ComponentUpdateRequest,
} from "@/types/component";

export const componentsApi = {
  listForProject: (projectId: number, includeDeleted = false) =>
    apiGet<ComponentResponse[]>(`/projects/${projectId}/components`, { includeDeleted }),
  get: (id: number, includeDeleted = false) =>
    apiGet<ComponentResponse>(`/components/${id}`, { includeDeleted }),
  create: (projectId: number, req: ComponentCreateRequest) =>
    apiPost<ComponentResponse>(`/projects/${projectId}/components`, req),
  update: (id: number, req: ComponentUpdateRequest) => apiPut<ComponentResponse>(`/components/${id}`, req),
  remove: (id: number) => apiDelete<{ id: number }>(`/components/${id}`),
  setStatusNote: (id: number, req: ComponentStatusNoteRequest) =>
    apiPut<ComponentResponse>(`/components/${id}/status-note`, req),
  attachTag: (id: number, tagId: number) => apiPost(`/components/${id}/tags/${tagId}`),
  detachTag: (id: number, tagId: number) => apiDelete(`/components/${id}/tags/${tagId}`),
};
