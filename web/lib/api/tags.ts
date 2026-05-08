import { apiDelete, apiGet, apiPost, apiPut } from "@/lib/api";
import type { TagCreateRequest, TagResponse, TagUpdateRequest } from "@/types/tag";

export const tagsApi = {
  list: () => apiGet<TagResponse[]>("/tags"),
  create: (req: TagCreateRequest) => apiPost<TagResponse>("/tags", req),
  update: (id: number, req: TagUpdateRequest) => apiPut<TagResponse>(`/tags/${id}`, req),
  remove: (id: number) => apiDelete<{ id: number }>(`/tags/${id}`),
};
