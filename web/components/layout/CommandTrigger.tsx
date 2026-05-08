"use client";

import { Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useCommandPaletteStore } from "@/store/commandPalette";

export function CommandTrigger() {
  const setOpen = useCommandPaletteStore((s) => s.setOpen);
  return (
    <Button
      variant="outline"
      onClick={() => setOpen(true)}
      className="hidden h-9 w-full max-w-md justify-start gap-2 px-3 text-muted-foreground md:inline-flex"
    >
      <Search className="h-4 w-4" />
      <span className="text-sm">Ara veya git…</span>
      <kbd className="ml-auto pointer-events-none hidden select-none rounded bg-muted px-1.5 py-0.5 font-mono text-[10px] font-medium text-muted-foreground sm:inline-block">
        Ctrl K
      </kbd>
    </Button>
  );
}
