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
import { cn } from "@/lib/utils";

const projectClass: Record<ProjectStatus, { dot: string; bg: string }> = {
  Active: { dot: "bg-success", bg: "bg-success-soft text-success" },
  Paused: { dot: "bg-text-muted", bg: "bg-surface-3 text-text-muted" },
  Completed: { dot: "bg-info", bg: "bg-info-soft text-info" },
  Abandoned: { dot: "bg-warning", bg: "bg-surface-3 text-text-faint" },
};

export function ProjectStatusBadge({ status }: { status: ProjectStatus }) {
  const c = projectClass[status];
  return (
    <span className={cn("inline-flex w-fit items-center gap-1.5 self-start rounded-md px-1.5 py-0.5 text-[11px] font-medium", c.bg)}>
      <span className={cn("status-dot", c.dot)} aria-hidden />
      {PROJECT_STATUS_LABELS[status]}
    </span>
  );
}

export function ProjectStatusDot({ status }: { status: ProjectStatus }) {
  return <span className={cn("status-dot", projectClass[status].dot)} aria-hidden />;
}

// Priority — High is intentionally filled to dominate.
const priorityClass: Record<NextStepPriority, string> = {
  Low: "bg-surface-3 text-text-muted",
  Medium: "bg-info-soft text-info ring-1 ring-info/20",
  High: "bg-warning text-white shadow-soft",
};

export function PriorityBadge({ priority }: { priority: NextStepPriority }) {
  return (
    <span className={cn("inline-flex w-fit items-center self-start rounded-md px-2 py-0.5 text-[11px] font-medium tracking-tight", priorityClass[priority])}>
      {PRIORITY_LABELS[priority]}
    </span>
  );
}

const moduleClass: Record<LearningModuleStatus, { dot: string; bg: string }> = {
  NotStarted: { dot: "bg-text-muted", bg: "bg-surface-3 text-text-muted" },
  InProgress: { dot: "bg-info", bg: "bg-info-soft text-info" },
  Completed: { dot: "bg-success", bg: "bg-success-soft text-success" },
};

export function ModuleStatusBadge({ status }: { status: LearningModuleStatus }) {
  const c = moduleClass[status];
  return (
    <span className={cn("inline-flex w-fit items-center gap-1.5 self-start rounded-md px-1.5 py-0.5 text-[11px] font-medium", c.bg)}>
      <span className={cn("status-dot", c.dot)} aria-hidden />
      {MODULE_STATUS_LABELS[status]}
    </span>
  );
}

export function ResourceTypeBadge({ type }: { type: ResourceType }) {
  return <Badge variant="default">{RESOURCE_TYPE_LABELS[type]}</Badge>;
}
