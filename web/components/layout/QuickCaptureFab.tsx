"use client";

import { Plus } from "lucide-react";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";
import { useQuickCaptureStore } from "@/store/quickCapture";

export function QuickCaptureFab() {
  const openCapture = useQuickCaptureStore((s) => s.openCapture);
  return (
    <Tooltip>
      <TooltipTrigger
        type="button"
        onClick={() => openCapture()}
        aria-label="Hızlı yakala (Ctrl+J / ⌘J)"
        className="fixed bottom-6 right-6 z-50 flex h-12 w-12 items-center justify-center rounded-full bg-primary text-primary-foreground shadow-card ring-2 ring-surface-1 transition-all hover:brightness-110 hover:scale-105"
      >
        <Plus className="h-5 w-5" />
      </TooltipTrigger>
      <TooltipContent side="left">Hızlı yakala (Ctrl+J / ⌘J)</TooltipContent>
    </Tooltip>
  );
}
