import { Badge } from "@/components/ui/badge";
import {
  MODULE_STATUS_LABELS,
  PROJECT_STATUS_LABELS,
  PRIORITY_LABELS,
  RESOURCE_TYPE_LABELS,
} from "@/types/enums";
import type {
  LearningModuleStatus,
  NextStepPriority,
  ProjectStatus,
  ResourceType,
} from "@/types/enums";

const projectStatusClass: Record<ProjectStatus, string> = {
  Active: "bg-emerald-100 text-emerald-800 hover:bg-emerald-100 dark:bg-emerald-950 dark:text-emerald-300",
  Paused: "bg-amber-100 text-amber-900 hover:bg-amber-100 dark:bg-amber-950 dark:text-amber-300",
  Completed: "bg-sky-100 text-sky-900 hover:bg-sky-100 dark:bg-sky-950 dark:text-sky-300",
  Abandoned: "bg-zinc-200 text-zinc-700 hover:bg-zinc-200 dark:bg-zinc-800 dark:text-zinc-300",
};

export function ProjectStatusBadge({ status }: { status: ProjectStatus }) {
  return <Badge className={projectStatusClass[status]} variant="secondary">{PROJECT_STATUS_LABELS[status]}</Badge>;
}

export function ProjectStatusDot({ status }: { status: ProjectStatus }) {
  const cls: Record<ProjectStatus, string> = {
    Active: "bg-emerald-500",
    Paused: "bg-amber-500",
    Completed: "bg-sky-500",
    Abandoned: "bg-zinc-400",
  };
  return <span className={`inline-block h-2 w-2 rounded-full ${cls[status]}`} aria-hidden />;
}

const priorityClass: Record<NextStepPriority, string> = {
  Low: "bg-zinc-100 text-zinc-700 hover:bg-zinc-100 dark:bg-zinc-800 dark:text-zinc-300",
  Medium: "bg-amber-100 text-amber-900 hover:bg-amber-100 dark:bg-amber-950 dark:text-amber-300",
  High: "bg-rose-100 text-rose-900 hover:bg-rose-100 dark:bg-rose-950 dark:text-rose-300",
};

export function PriorityBadge({ priority }: { priority: NextStepPriority }) {
  return <Badge className={priorityClass[priority]} variant="secondary">{PRIORITY_LABELS[priority]}</Badge>;
}

const moduleStatusClass: Record<LearningModuleStatus, string> = {
  NotStarted: "bg-zinc-100 text-zinc-700 hover:bg-zinc-100 dark:bg-zinc-800 dark:text-zinc-300",
  InProgress: "bg-blue-100 text-blue-900 hover:bg-blue-100 dark:bg-blue-950 dark:text-blue-300",
  Completed: "bg-emerald-100 text-emerald-800 hover:bg-emerald-100 dark:bg-emerald-950 dark:text-emerald-300",
};

export function ModuleStatusBadge({ status }: { status: LearningModuleStatus }) {
  return <Badge className={moduleStatusClass[status]} variant="secondary">{MODULE_STATUS_LABELS[status]}</Badge>;
}

const resourceTypeClass: Record<ResourceType, string> = {
  ClaudeChat: "bg-orange-100 text-orange-900 hover:bg-orange-100 dark:bg-orange-950 dark:text-orange-300",
  Documentation: "bg-blue-100 text-blue-900 hover:bg-blue-100 dark:bg-blue-950 dark:text-blue-300",
  Article: "bg-violet-100 text-violet-900 hover:bg-violet-100 dark:bg-violet-950 dark:text-violet-300",
  Video: "bg-rose-100 text-rose-900 hover:bg-rose-100 dark:bg-rose-950 dark:text-rose-300",
  StackOverflow: "bg-amber-100 text-amber-900 hover:bg-amber-100 dark:bg-amber-950 dark:text-amber-300",
  GitHub: "bg-zinc-200 text-zinc-800 hover:bg-zinc-200 dark:bg-zinc-800 dark:text-zinc-200",
  Other: "bg-zinc-100 text-zinc-700 hover:bg-zinc-100 dark:bg-zinc-800 dark:text-zinc-300",
};

export function ResourceTypeBadge({ type }: { type: ResourceType }) {
  return <Badge className={resourceTypeClass[type]} variant="secondary">{RESOURCE_TYPE_LABELS[type]}</Badge>;
}
