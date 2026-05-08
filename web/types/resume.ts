import type {
  IdeaResponse,
  NextStepResponse,
  ResourceResponse,
  WorklogResponse,
} from "./activity";
import type { ComponentResponse } from "./component";
import type { LearningModuleResponse, LearningTrackResponse } from "./learning";
import type { ProjectResponse } from "./project";

type ResumeBase = {
  recentWorklogs: WorklogResponse[];
  openNextSteps: NextStepResponse[];
  resources: ResourceResponse[];
  recentIdeas: IdeaResponse[];
  daysSinceLastActivity: number | null;
};

export type ResumeProjectResponse = ResumeBase & {
  project: ProjectResponse;
  components: ComponentResponse[];
};

export type ResumeComponentResponse = ResumeBase & {
  component: ComponentResponse;
};

export type ResumeLearningTrackResponse = ResumeBase & {
  track: LearningTrackResponse;
  modules: LearningModuleResponse[];
  progressPercent: number;
};

export type ResumeLearningModuleResponse = ResumeBase & {
  module: LearningModuleResponse;
};
