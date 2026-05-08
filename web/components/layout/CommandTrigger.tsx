"use client";

import { Search } from "lucide-react";
import { useCommandPaletteStore } from "@/store/commandPalette";

export function CommandTrigger() {
  const setOpen = useCommandPaletteStore((s) => s.setOpen);
  return (
    <button
      type="button"
      onClick={() => setOpen(true)}
      className="hidden h-7 w-full max-w-[320px] items-center gap-2 rounded-md border border-border bg-surface-2 px-2.5 text-[12px] text-text-muted transition-colors hover:border-border-strong md:inline-flex"
    >
      <Search className="h-3.5 w-3.5" />
      <span>Ara veya git…</span>
      <kbd className="ml-auto pointer-events-none select-none font-mono text-[10px] text-text-faint">
        ⌘K
      </kbd>
    </button>
  );
}
