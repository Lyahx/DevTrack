import type { ComponentType } from "./enums";
import type { TagResponse } from "./tag";

export type ComponentResponse = {
  id: number;
  projectId: number;
  name: string;
  type: ComponentType;
  techStack: string | null;
  localUrl: string | null;
  repoPath: string | null;
  currentStatusNote: string | null;
  lastActivityAt: string | null;
  createdAt: string;
  updatedAt: string | null;
  isDeleted: boolean;
  deletedAt: string | null;
  tags: TagResponse[];
};

export type ComponentCreateRequest = {
  name: string;
  type: ComponentType;
  techStack?: string | null;
  localUrl?: string | null;
  repoPath?: string | null;
  currentStatusNote?: string | null;
};

export type ComponentUpdateRequest = ComponentCreateRequest;

export type ComponentStatusNoteRequest = {
  currentStatusNote: string | null;
};
