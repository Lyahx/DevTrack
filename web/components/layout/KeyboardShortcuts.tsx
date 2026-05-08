"use client";

import { useEffect } from "react";
import { useCommandPaletteStore } from "@/store/commandPalette";
import { useQuickCaptureStore } from "@/store/quickCapture";

export function KeyboardShortcuts() {
  const setCommandOpen = useCommandPaletteStore((s) => s.setOpen);
  const openCapture = useQuickCaptureStore((s) => s.openCapture);
  const closeCapture = useQuickCaptureStore((s) => s.close);

  useEffect(() => {
    function onKey(e: KeyboardEvent) {
      const meta = e.metaKey || e.ctrlKey;
      if (!meta) return;
      const k = e.key.toLowerCase();
      if (k === "k") {
        e.preventDefault();
        setCommandOpen(true);
      } else if (k === "j") {
        e.preventDefault();
        openCapture();
      }
    }
    function onEsc(e: KeyboardEvent) {
      if (e.key === "Escape") {
        // Quick capture has its own Esc handling via Dialog; this is a safety net.
      }
      void closeCapture;
    }
    window.addEventListener("keydown", onKey);
    window.addEventListener("keydown", onEsc);
    return () => {
      window.removeEventListener("keydown", onKey);
      window.removeEventListener("keydown", onEsc);
    };
  }, [setCommandOpen, openCapture, closeCapture]);

  return null;
}
