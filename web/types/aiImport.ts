import type { NextStepPriority, ResourceType } from "./enums";

export type AiWorklogItem = {
  whatIDid: string;
  whatsLeft?: string | null;
  reasoning?: string | null;
  alternatives?: string | null;
};

export type AiNextStepItem = {
  description: string;
  priority: NextStepPriority;
};

export type AiIdeaItem = {
  content: string;
};

export type AiResourceItem = {
  title: string;
  url: string;
  type: ResourceType;
  notes?: string | null;
};

export type AiExtractionResult = {
  worklogs: AiWorklogItem[];
  nextSteps: AiNextStepItem[];
  ideas: AiIdeaItem[];
  resources: AiResourceItem[];
};

export type AiImportApplyResult = {
  worklogsCreated: number;
  nextStepsCreated: number;
  ideasCreated: number;
  resourcesCreated: number;
};
