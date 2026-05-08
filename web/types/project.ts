import type { ProjectStatus } from "./enums";
import type { TagResponse } from "./tag";

export type ProjectResponse = {
  id: number;
  userId: number;
  name: string;
  description: string | null;
  goal: string | null;
  status: ProjectStatus;
  repoUrl: string | null;
  lastActivityAt: string | null;
  completedAt: string | null;
  createdAt: string;
  updatedAt: string | null;
  isDeleted: boolean;
  deletedAt: string | null;
  tags: TagResponse[];
};

export type ProjectCreateRequest = {
  name: string;
  description?: string | null;
  goal?: string | null;
  repoUrl?: string | null;
  status?: ProjectStatus;
};

export type ProjectUpdateRequest = {
  name: string;
  description?: string | null;
  goal?: string | null;
  repoUrl?: string | null;
};

export type ProjectStatusUpdateRequest = {
  status: ProjectStatus;
};

export type ProjectListQuery = {
  status?: ProjectStatus;
  tagId?: number;
  page?: number;
  pageSize?: number;
  includeDeleted?: boolean;
};
