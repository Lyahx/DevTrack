export type ProjectStatus = "Active" | "Paused" | "Completed" | "Abandoned";
export const PROJECT_STATUSES: ProjectStatus[] = ["Active", "Paused", "Completed", "Abandoned"];

export type ComponentType = "Api" | "WebApp" | "Mobile" | "Game" | "Library" | "Other";
export const COMPONENT_TYPES: ComponentType[] = ["Api", "WebApp", "Mobile", "Game", "Library", "Other"];

export type LearningTrackStatus = ProjectStatus;
export const LEARNING_TRACK_STATUSES: LearningTrackStatus[] = ["Active", "Paused", "Completed", "Abandoned"];

export type LearningModuleStatus = "NotStarted" | "InProgress" | "Completed";
export const LEARNING_MODULE_STATUSES: LearningModuleStatus[] = ["NotStarted", "InProgress", "Completed"];

export type NextStepPriority = "Low" | "Medium" | "High";
export const NEXT_STEP_PRIORITIES: NextStepPriority[] = ["Low", "Medium", "High"];

export type ResourceType =
  | "ClaudeChat"
  | "Documentation"
  | "Article"
  | "Video"
  | "StackOverflow"
  | "GitHub"
  | "Other";
export const RESOURCE_TYPES: ResourceType[] = [
  "ClaudeChat",
  "Documentation",
  "Article",
  "Video",
  "StackOverflow",
  "GitHub",
  "Other",
];

export type ReminderType =
  | "ProjectInactive"
  | "LearningTrackInactive"
  | "NextStepOverdue"
  | "Other";

export type ReminderSeverity = "Info" | "Warning";

export type OwnerType = "Project" | "Component" | "LearningTrack" | "LearningModule";
export const OWNER_TYPES: OwnerType[] = ["Project", "Component", "LearningTrack", "LearningModule"];

export const PROJECT_STATUS_LABELS: Record<ProjectStatus, string> = {
  Active: "Aktif",
  Paused: "Beklemede",
  Completed: "Tamamlandı",
  Abandoned: "Bırakıldı",
};

export const MODULE_STATUS_LABELS: Record<LearningModuleStatus, string> = {
  NotStarted: "Başlamadı",
  InProgress: "Devam ediyor",
  Completed: "Tamamlandı",
};

export const PRIORITY_LABELS: Record<NextStepPriority, string> = {
  Low: "Düşük",
  Medium: "Orta",
  High: "Yüksek",
};

export const COMPONENT_TYPE_LABELS: Record<ComponentType, string> = {
  Api: "API",
  WebApp: "Web",
  Mobile: "Mobil",
  Game: "Oyun",
  Library: "Kütüphane",
  Other: "Diğer",
};

export const RESOURCE_TYPE_LABELS: Record<ResourceType, string> = {
  ClaudeChat: "Claude Chat",
  Documentation: "Dokümantasyon",
  Article: "Makale",
  Video: "Video",
  StackOverflow: "StackOverflow",
  GitHub: "GitHub",
  Other: "Diğer",
};

export const OWNER_TYPE_LABELS: Record<OwnerType, string> = {
  Project: "Proje",
  Component: "Bileşen",
  LearningTrack: "Eğitim",
  LearningModule: "Modül",
};
