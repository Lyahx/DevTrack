import type { NextStepPriority, ResourceType } from "./enums";

export type AiWorklogItem = {
  whatIDid: string;
  whatsLeft?: string | null;
};

export type AiDecisionItem = {
  title: string;
  reasoning: string;
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
  decisions: AiDecisionItem[];
  nextSteps: AiNextStepItem[];
  ideas: AiIdeaItem[];
  resources: AiResourceItem[];
};

export type AiImportApplyResult = {
  worklogsCreated: number;
  decisionsCreated: number;
  nextStepsCreated: number;
  ideasCreated: number;
  resourcesCreated: number;
};
