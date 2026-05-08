import { apiGet } from "@/lib/api";
import type {
  ResumeComponentResponse,
  ResumeLearningModuleResponse,
  ResumeLearningTrackResponse,
  ResumeProjectResponse,
} from "@/types/resume";

export const resumeApi = {
  forProject: (id: number) => apiGet<ResumeProjectResponse>(`/projects/${id}/resume`),
  forComponent: (id: number) => apiGet<ResumeComponentResponse>(`/components/${id}/resume`),
  forTrack: (id: number) => apiGet<ResumeLearningTrackResponse>(`/learning-tracks/${id}/resume`),
  forModule: (id: number) => apiGet<ResumeLearningModuleResponse>(`/modules/${id}/resume`),
};
