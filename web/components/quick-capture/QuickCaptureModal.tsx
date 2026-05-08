"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useEffect, useRef, useState } from "react";
import { toast } from "sonner";
import { OwnerPicker } from "@/components/common/OwnerPicker";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { decisionsApi } from "@/lib/api/decisions";
import { ideasApi } from "@/lib/api/ideas";
import { nextStepsApi } from "@/lib/api/nextSteps";
import { worklogsApi } from "@/lib/api/worklogs";
import { errorMessage } from "@/lib/error";
import { useQuickCaptureStore } from "@/store/quickCapture";
import type { OwnerReference } from "@/types/owner";
import { cn } from "@/lib/utils";

type CaptureType = "Idea" | "NextStep" | "Worklog" | "Decision";

const TYPE_LABELS: Record<CaptureType, string> = {
  Idea: "Fikir",
  NextStep: "Adım",
  Worklog: "Worklog",
  Decision: "Karar",
};

const TYPES: CaptureType[] = ["Idea", "NextStep", "Worklog", "Decision"];

export function QuickCaptureModal() {
  const open = useQuickCaptureStore((s) => s.open);
  const close = useQuickCaptureStore((s) => s.close);
  const defaultOwner = useQuickCaptureStore((s) => s.defaultOwner);

  const [content, setContent] = useState("");
  const [type, setType] = useState<CaptureType>("Idea");
  const [owner, setOwner] = useState<OwnerReference | null>(null);
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const qc = useQueryClient();

  useEffect(() => {
    if (open) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setOwner(defaultOwner ?? null);
      setContent("");
      setType("Idea");
      setTimeout(() => textareaRef.current?.focus(), 50);
    }
  }, [open, defaultOwner]);

  const submit = useMutation({
    mutationFn: async () => {
      if (!owner) throw new Error("Önce bir sahip seç.");
      if (!content.trim()) throw new Error("İçerik boş olamaz.");
      switch (type) {
        case "Idea":
          return ideasApi.create({ owner, content });
        case "NextStep":
          return nextStepsApi.create({ owner, description: content, priority: "Medium" });
        case "Worklog":
          return worklogsApi.create({ owner, whatIDid: content });
        case "Decision":
          return decisionsApi.create({ owner, title: content.slice(0, 100), reasoning: content });
      }
    },
    onSuccess: () => {
      toast.success(`${TYPE_LABELS[type]} kaydedildi.`);
      qc.invalidateQueries({ queryKey: ["worklogs"] });
      qc.invalidateQueries({ queryKey: ["ideas"] });
      qc.invalidateQueries({ queryKey: ["next-steps"] });
      qc.invalidateQueries({ queryKey: ["decisions"] });
      qc.invalidateQueries({ queryKey: ["dashboard"] });
      qc.invalidateQueries({ queryKey: ["resume"] });
      close();
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <Dialog open={open} onOpenChange={(v) => (v ? null : close())}>
      <DialogContent className="gap-3 p-5 sm:max-w-[540px]">
        <DialogHeader className="sr-only">
          <DialogTitle>Hızlı yakala</DialogTitle>
          <DialogDescription>İçeriği yaz, türü seç, sahibi belirle.</DialogDescription>
        </DialogHeader>
        <textarea
          ref={textareaRef}
          placeholder="Bir fikir, bir not, bir sonraki adım…"
          rows={3}
          value={content}
          onChange={(e) => setContent(e.target.value)}
          onKeyDown={(e) => {
            if ((e.metaKey || e.ctrlKey) && e.key === "Enter") {
              e.preventDefault();
              submit.mutate();
            }
          }}
          className="w-full resize-none border-0 bg-transparent text-[16px] leading-relaxed text-text outline-none placeholder:text-text-faint"
        />

        <div className="flex items-center gap-2">
          {TYPES.map((t) => (
            <button
              key={t}
              type="button"
              onClick={() => setType(t)}
              className={cn(
                "rounded-md px-2 py-1 text-[11px] font-medium tracking-tight transition-colors",
                type === t
                  ? "bg-surface-3 text-text"
                  : "text-text-muted hover:bg-surface-2 hover:text-text-secondary",
              )}
            >
              {TYPE_LABELS[t]}
            </button>
          ))}
          <div className="ml-auto">
            <OwnerPicker value={owner} onChange={setOwner} />
          </div>
        </div>

        <div className="flex items-center justify-between border-t border-border-subtle pt-3">
          <span className="font-mono text-[10px] text-text-faint">⌘↵ kaydet · ESC kapat</span>
          <Button onClick={() => submit.mutate()} disabled={submit.isPending}>
            {submit.isPending ? "Kaydediliyor…" : "Yakala"}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
