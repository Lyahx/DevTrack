import { create } from "zustand";

type State = {
  open: boolean;
  setOpen: (v: boolean) => void;
};

export const useCommandPaletteStore = create<State>((set) => ({
  open: false,
  setOpen: (v) => set({ open: v }),
}));
