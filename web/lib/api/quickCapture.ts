import { apiPost } from "@/lib/api";
import type { IdeaResponse } from "@/types/activity";
import type { QuickCaptureRequest } from "@/types/quick-capture";

export const quickCaptureApi = {
  capture: (req: QuickCaptureRequest) => apiPost<IdeaResponse>("/quick-capture", req),
};
