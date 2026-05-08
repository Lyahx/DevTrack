import type { ReminderSeverity, ReminderType } from "./enums";

export type ReminderResponse = {
  id: number;
  userId: number;
  relatedProjectId: number | null;
  relatedLearningTrackId: number | null;
  type: ReminderType;
  title: string;
  message: string;
  severity: ReminderSeverity;
  isRead: boolean;
  isDismissed: boolean;
  generatedAt: string;
  createdAt: string;
  updatedAt: string | null;
  isDeleted: boolean;
  deletedAt: string | null;
};

export type ReminderListQuery = {
  isRead?: boolean;
  isDismissed?: boolean;
  page?: number;
  pageSize?: number;
  includeDeleted?: boolean;
};
