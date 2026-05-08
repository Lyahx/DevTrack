import { apiDelete, apiGet, apiPost, apiPut } from "@/lib/api";
import type { PagedResult } from "@/types/api";
import type {
  ProjectCreateRequest,
  ProjectListQuery,
  ProjectResponse,
  ProjectStatusUpdateRequest,
  ProjectUpdateRequest,
} from "@/types/project";

export const projectsApi = {
  list: (q: ProjectListQuery = {}) => apiGet<PagedResult<ProjectResponse>>("/projects", q as Record<string, unknown>),
  get: (id: number, includeDeleted = false) =>
    apiGet<ProjectResponse>(`/projects/${id}`, { includeDeleted }),
  create: (req: ProjectCreateRequest) => apiPost<ProjectResponse>("/projects", req),
  update: (id: number, req: ProjectUpdateRequest) => apiPut<ProjectResponse>(`/projects/${id}`, req),
  remove: (id: number) => apiDelete<{ id: number }>(`/projects/${id}`),
  setStatus: (id: number, req: ProjectStatusUpdateRequest) =>
    apiPost<ProjectResponse>(`/projects/${id}/status`, req),
  attachTag: (id: number, tagId: number) => apiPost(`/projects/${id}/tags/${tagId}`),
  detachTag: (id: number, tagId: number) => apiDelete(`/projects/${id}/tags/${tagId}`),
};
