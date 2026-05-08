import type { NextStepPriority, ResourceType } from "./enums";
import type { OwnerReference } from "./owner";

type Audited = {
  id: number;
  userId: number;
  owner: OwnerReference;
  createdAt: string;
  updatedAt: string | null;
  isDeleted: boolean;
  deletedAt: string | null;
};

export type WorklogResponse = Audited & {
  whatIDid: string;
  whatsLeft: string | null;
  loggedAt: string;
};

export type WorklogCreateRequest = {
  owner: OwnerReference;
  whatIDid: string;
  whatsLeft?: string | null;
  loggedAt?: string | null;
};

export type WorklogUpdateRequest = {
  whatIDid: string;
  whatsLeft?: string | null;
  loggedAt?: string | null;
};

export type WorklogListQuery = {
  projectId?: number;
  componentId?: number;
  learningTrackId?: number;
  learningModuleId?: number;
  fromDate?: string;
  toDate?: string;
  page?: number;
  pageSize?: number;
  includeDeleted?: boolean;
};

export type DecisionResponse = Audited & {
  title: string;
  reasoning: string;
  alternatives: string | null;
  decidedAt: string;
};

export type DecisionCreateRequest = {
  owner: OwnerReference;
  title: string;
  reasoning: string;
  alternatives?: string | null;
  decidedAt?: string | null;
};

export type DecisionUpdateRequest = {
  title: string;
  reasoning: string;
  alternatives?: string | null;
  decidedAt?: string | null;
};

export type DecisionListQuery = Omit<WorklogListQuery, "fromDate" | "toDate">;

export type NextStepResponse = Audited & {
  description: string;
  priority: NextStepPriority;
  isCompleted: boolean;
  completedAt: string | null;
};

export type NextStepCreateRequest = {
  owner: OwnerReference;
  description: string;
  priority?: NextStepPriority;
};

export type NextStepUpdateRequest = {
  description: string;
  priority: NextStepPriority;
};

export type NextStepListQuery = DecisionListQuery & {
  isCompleted?: boolean;
  priority?: NextStepPriority;
};

export type IdeaResponse = Audited & {
  content: string;
  isConvertedToNextStep: boolean;
  convertedNextStepId: number | null;
  capturedAt: string;
};

export type IdeaCreateRequest = {
  owner: OwnerReference;
  content: string;
};

export type IdeaUpdateRequest = {
  content: string;
};

export type IdeaListQuery = DecisionListQuery & {
  isConvertedToNextStep?: boolean;
};

export type IdeaConvertRequest = {
  priority: NextStepPriority;
};

export type ResourceResponse = Audited & {
  title: string;
  url: string;
  type: ResourceType;
  notes: string | null;
  addedAt: string;
};

export type ResourceCreateRequest = {
  owner: OwnerReference;
  title: string;
  url: string;
  type: ResourceType;
  notes?: string | null;
};

export type ResourceUpdateRequest = {
  title: string;
  url: string;
  type: ResourceType;
  notes?: string | null;
};

export type ResourceListQuery = DecisionListQuery & {
  type?: ResourceType;
};
