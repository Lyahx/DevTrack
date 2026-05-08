import { create } from "zustand";
import type { OwnerReference } from "@/types/owner";

type State = {
  open: boolean;
  defaultOwner: OwnerReference | null;
  openCapture: (defaultOwner?: OwnerReference) => void;
  close: () => void;
};

export const useQuickCaptureStore = create<State>((set) => ({
  open: false,
  defaultOwner: null,
  openCapture: (defaultOwner) => set({ open: true, defaultOwner: defaultOwner ?? null }),
  close: () => set({ open: false, defaultOwner: null }),
}));
