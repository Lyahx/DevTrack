import type { OwnerReference } from "./owner";

export type QuickCaptureRequest = {
  owner: OwnerReference;
  content: string;
};
