import type { NextStepResponse, WorklogResponse } from "./activity";
import type { LearningTrackResponse } from "./learning";
import type { ProjectResponse } from "./project";
import type { ReminderResponse } from "./reminder";

export type DashboardResponse = {
  activeProjects: ProjectResponse[];
  staleProjects: ProjectResponse[];
  activeLearningTracks: LearningTrackResponse[];
  staleLearningTracks: LearningTrackResponse[];
  openNextStepsCount: number;
  highPriorityOpenNextSteps: NextStepResponse[];
  recentWorklogs: WorklogResponse[];
  unreadReminders: ReminderResponse[];
  unreadRemindersCount: number;
};
