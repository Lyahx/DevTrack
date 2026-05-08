"use client";

import { Plus } from "lucide-react";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";
import { buttonVariants } from "@/components/ui/button";
import { useQuickCaptureStore } from "@/store/quickCapture";
import { cn } from "@/lib/utils";

export function QuickCaptureFab() {
  const openCapture = useQuickCaptureStore((s) => s.openCapture);
  return (
    <Tooltip>
      <TooltipTrigger
        type="button"
        onClick={() => openCapture()}
        aria-label="Hızlı yakala (Ctrl+J / ⌘J)"
        className={cn(
          buttonVariants({ variant: "default", size: "icon" }),
          "fixed bottom-6 right-6 z-50 h-12 w-12 rounded-full shadow-lg",
        )}
      >
        <Plus className="h-5 w-5" />
      </TooltipTrigger>
      <TooltipContent side="left">Hızlı yakala (Ctrl+J / ⌘J)</TooltipContent>
    </Tooltip>
  );
}
