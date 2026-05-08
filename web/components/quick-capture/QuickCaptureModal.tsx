"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useEffect, useRef, useState } from "react";
import { toast } from "sonner";
import { OwnerPicker } from "@/components/common/OwnerPicker";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { Textarea } from "@/components/ui/textarea";
import { decisionsApi } from "@/lib/api/decisions";
import { ideasApi } from "@/lib/api/ideas";
import { nextStepsApi } from "@/lib/api/nextSteps";
import { worklogsApi } from "@/lib/api/worklogs";
import { errorMessage } from "@/lib/error";
import { useQuickCaptureStore } from "@/store/quickCapture";
import type { OwnerReference } from "@/types/owner";

type CaptureType = "Idea" | "NextStep" | "Worklog" | "Decision";

const TYPE_LABELS: Record<CaptureType, string> = {
  Idea: "Fikir",
  NextStep: "Sonraki adım",
  Worklog: "Worklog",
  Decision: "Karar",
};

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
      <DialogContent className="max-w-xl">
        <DialogHeader>
          <DialogTitle>Hızlı yakala</DialogTitle>
          <DialogDescription>İçeriği yaz, türü seç, sahibi belirle. Ctrl/⌘+Enter ile gönder.</DialogDescription>
        </DialogHeader>
        <div className="space-y-3">
          <Textarea
            ref={textareaRef}
            placeholder="Bir fikir, bir not, bir sonraki adım…"
            rows={4}
            value={content}
            onChange={(e) => setContent(e.target.value)}
            onKeyDown={(e) => {
              if ((e.metaKey || e.ctrlKey) && e.key === "Enter") {
                e.preventDefault();
                submit.mutate();
              }
            }}
          />
          <div className="grid grid-cols-1 gap-3 md:grid-cols-2">
            <div className="space-y-1.5">
              <Label className="text-xs uppercase tracking-wider text-muted-foreground">Sahip</Label>
              <OwnerPicker value={owner} onChange={setOwner} />
            </div>
            <div className="space-y-1.5">
              <Label className="text-xs uppercase tracking-wider text-muted-foreground">Tür</Label>
              <RadioGroup
                value={type}
                onValueChange={(v) => setType(v as CaptureType)}
                className="flex flex-wrap gap-3"
              >
                {(["Idea", "NextStep", "Worklog", "Decision"] as CaptureType[]).map((t) => (
                  <div key={t} className="flex items-center gap-1.5">
                    <RadioGroupItem id={`qc-${t}`} value={t} />
                    <Label htmlFor={`qc-${t}`} className="cursor-pointer text-sm">
                      {TYPE_LABELS[t]}
                    </Label>
                  </div>
                ))}
              </RadioGroup>
            </div>
          </div>
        </div>
        <DialogFooter>
          <Button variant="ghost" onClick={close}>Kapat</Button>
          <Button onClick={() => submit.mutate()} disabled={submit.isPending}>
            {submit.isPending ? "Kaydediliyor…" : "Yakala"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
