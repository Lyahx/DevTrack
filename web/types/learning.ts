import type { LearningTrackStatus, LearningModuleStatus } from "./enums";
import type { TagResponse } from "./tag";

export type LearningTrackResponse = {
  id: number;
  userId: number;
  name: string;
  description: string | null;
  source: string | null;
  status: LearningTrackStatus;
  lastActivityAt: string | null;
  completedAt: string | null;
  createdAt: string;
  updatedAt: string | null;
  isDeleted: boolean;
  deletedAt: string | null;
  tags: TagResponse[];
};

export type LearningTrackCreateRequest = {
  name: string;
  description?: string | null;
  source?: string | null;
  status?: LearningTrackStatus;
};

export type LearningTrackUpdateRequest = {
  name: string;
  description?: string | null;
  source?: string | null;
};

export type LearningTrackStatusUpdateRequest = { status: LearningTrackStatus };

export type LearningTrackListQuery = {
  status?: LearningTrackStatus;
  tagId?: number;
  page?: number;
  pageSize?: number;
  includeDeleted?: boolean;
};

export type LearningModuleResponse = {
  id: number;
  learningTrackId: number;
  name: string;
  order: number;
  status: LearningModuleStatus;
  startedAt: string | null;
  completedAt: string | null;
  lastActivityAt: string | null;
  createdAt: string;
  updatedAt: string | null;
  isDeleted: boolean;
  deletedAt: string | null;
};

export type LearningModuleCreateRequest = {
  name: string;
  order: number;
  status?: LearningModuleStatus;
};

export type LearningModuleUpdateRequest = {
  name: string;
  order: number;
};

export type LearningModuleStatusUpdateRequest = { status: LearningModuleStatus };
export type LearningModuleOrderUpdateRequest = { order: number };
