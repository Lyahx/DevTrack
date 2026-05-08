import { apiDelete, apiGet, apiPost, apiPut } from "@/lib/api";
import type { PagedResult } from "@/types/api";
import type {
  LearningModuleCreateRequest,
  LearningModuleOrderUpdateRequest,
  LearningModuleResponse,
  LearningModuleStatusUpdateRequest,
  LearningModuleUpdateRequest,
  LearningTrackCreateRequest,
  LearningTrackListQuery,
  LearningTrackResponse,
  LearningTrackStatusUpdateRequest,
  LearningTrackUpdateRequest,
} from "@/types/learning";

export const learningTracksApi = {
  list: (q: LearningTrackListQuery = {}) =>
    apiGet<PagedResult<LearningTrackResponse>>("/learning-tracks", q as Record<string, unknown>),
  get: (id: number, includeDeleted = false) =>
    apiGet<LearningTrackResponse>(`/learning-tracks/${id}`, { includeDeleted }),
  create: (req: LearningTrackCreateRequest) => apiPost<LearningTrackResponse>("/learning-tracks", req),
  update: (id: number, req: LearningTrackUpdateRequest) =>
    apiPut<LearningTrackResponse>(`/learning-tracks/${id}`, req),
  remove: (id: number) => apiDelete<{ id: number }>(`/learning-tracks/${id}`),
  setStatus: (id: number, req: LearningTrackStatusUpdateRequest) =>
    apiPost<LearningTrackResponse>(`/learning-tracks/${id}/status`, req),
  attachTag: (id: number, tagId: number) => apiPost(`/learning-tracks/${id}/tags/${tagId}`),
  detachTag: (id: number, tagId: number) => apiDelete(`/learning-tracks/${id}/tags/${tagId}`),
};

export const learningModulesApi = {
  listForTrack: (trackId: number, includeDeleted = false) =>
    apiGet<LearningModuleResponse[]>(`/learning-tracks/${trackId}/modules`, { includeDeleted }),
  get: (id: number, includeDeleted = false) =>
    apiGet<LearningModuleResponse>(`/modules/${id}`, { includeDeleted }),
  create: (trackId: number, req: LearningModuleCreateRequest) =>
    apiPost<LearningModuleResponse>(`/learning-tracks/${trackId}/modules`, req),
  update: (id: number, req: LearningModuleUpdateRequest) =>
    apiPut<LearningModuleResponse>(`/modules/${id}`, req),
  remove: (id: number) => apiDelete<{ id: number }>(`/modules/${id}`),
  setStatus: (id: number, req: LearningModuleStatusUpdateRequest) =>
    apiPut<LearningModuleResponse>(`/modules/${id}/status`, req),
  setOrder: (id: number, req: LearningModuleOrderUpdateRequest) =>
    apiPut<LearningModuleResponse>(`/modules/${id}/order`, req),
};
