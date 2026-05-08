"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { AlertTriangle, ArrowRight, ExternalLink, Sparkles, Wand2 } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";
import { learningTracksApi } from "@/lib/api/learning";
import { errorMessage } from "@/lib/error";
import type {
  AiDecisionItem,
  AiExtractionResult,
  AiIdeaItem,
  AiNextStepItem,
  AiResourceItem,
  AiWorklogItem,
} from "@/types/aiImport";

type Selection = {
  worklogs: Set<number>;
  decisions: Set<number>;
  nextSteps: Set<number>;
  ideas: Set<number>;
  resources: Set<number>;
};

function selectAllOf<T>(arr: T[]) {
  return new Set(arr.map((_, i) => i));
}

export function AiImportModal({
  open,
  onOpenChange,
  trackId,
  aiChatUrl,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  trackId: number;
  aiChatUrl?: string | null;
}) {
  const qc = useQueryClient();
  const [transcript, setTranscript] = useState("");
  const [extracted, setExtracted] = useState<AiExtractionResult | null>(null);
  const [selection, setSelection] = useState<Selection>({
    worklogs: new Set(),
    decisions: new Set(),
    nextSteps: new Set(),
    ideas: new Set(),
    resources: new Set(),
  });

  useEffect(() => {
    if (open) {
      setTranscript("");
      setExtracted(null);
      setSelection({
        worklogs: new Set(),
        decisions: new Set(),
        nextSteps: new Set(),
        ideas: new Set(),
        resources: new Set(),
      });
    }
  }, [open]);

  const extract = useMutation({
    mutationFn: () => learningTracksApi.aiExtract(trackId, transcript),
    onSuccess: (data) => {
      setExtracted(data);
      // Default: hepsi seçili
      setSelection({
        worklogs: selectAllOf(data.worklogs),
        decisions: selectAllOf(data.decisions),
        nextSteps: selectAllOf(data.nextSteps),
        ideas: selectAllOf(data.ideas),
        resources: selectAllOf(data.resources),
      });
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  const totalCount = extracted
    ? extracted.worklogs.length + extracted.decisions.length + extracted.nextSteps.length + extracted.ideas.length + extracted.resources.length
    : 0;

  const looksLikeOnlyUrl = useMemo(() => {
    const t = transcript.trim();
    if (t.length === 0 || t.length > 500) return false;
    return /^https?:\/\/\S+$/i.test(t);
  }, [transcript]);
  const selectedCount =
    selection.worklogs.size + selection.decisions.size + selection.nextSteps.size + selection.ideas.size + selection.resources.size;

  const apply = useMutation({
    mutationFn: () => {
      if (!extracted) throw new Error("Önce çıkarım yap.");
      const payload: AiExtractionResult = {
        worklogs: extracted.worklogs.filter((_, i) => selection.worklogs.has(i)),
        decisions: extracted.decisions.filter((_, i) => selection.decisions.has(i)),
        nextSteps: extracted.nextSteps.filter((_, i) => selection.nextSteps.has(i)),
        ideas: extracted.ideas.filter((_, i) => selection.ideas.has(i)),
        resources: extracted.resources.filter((_, i) => selection.resources.has(i)),
      };
      return learningTracksApi.aiApply(trackId, payload);
    },
    onSuccess: (r) => {
      const total = r.worklogsCreated + r.decisionsCreated + r.nextStepsCreated + r.ideasCreated + r.resourcesCreated;
      toast.success(`${total} öğe kaydedildi.`);
      // Close first, defer invalidations until after the close animation
      // so the dialog teardown doesn't race with multiple list re-renders.
      onOpenChange(false);
      window.setTimeout(() => {
        qc.invalidateQueries({ queryKey: ["worklogs"] });
        qc.invalidateQueries({ queryKey: ["decisions"] });
        qc.invalidateQueries({ queryKey: ["next-steps"] });
        qc.invalidateQueries({ queryKey: ["ideas"] });
        qc.invalidateQueries({ queryKey: ["resources"] });
        qc.invalidateQueries({ queryKey: ["resume"] });
        qc.invalidateQueries({ queryKey: ["learning-track", trackId] });
      }, 250);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  function toggle<K extends keyof Selection>(key: K, idx: number) {
    setSelection((prev) => {
      const next = new Set(prev[key]);
      if (next.has(idx)) next.delete(idx);
      else next.add(idx);
      return { ...prev, [key]: next };
    });
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[640px]">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Sparkles className="h-4 w-4 text-primary" /> AI ile içe aktar
          </DialogTitle>
          <DialogDescription>
            {extracted
              ? "Çıkarılan öğeleri gözden geçir, kaydetmek istemediklerini tikten çıkar."
              : "Claude / ChatGPT konuşmanı yapıştır — yerel AI modeli (LM Studio / Ollama) öğeleri çıkarır."}
          </DialogDescription>
        </DialogHeader>

        {!extracted ? (
          <div className="space-y-3">
            <ol className="space-y-1.5 rounded-md border border-border-subtle bg-surface-2 p-3 text-[12px] text-text-secondary">
              {aiChatUrl ? (
                <li className="flex items-start gap-2">
                  <span className="font-mono text-[10px] text-text-faint">1.</span>
                  <span>
                    Sohbeti aç:{" "}
                    <a href={aiChatUrl} target="_blank" rel="noopener noreferrer" className="inline-flex items-center gap-0.5 font-mono text-[11px] text-text underline underline-offset-2">
                      <ExternalLink className="h-3 w-3" />
                      {aiChatUrl.length > 50 ? `${aiChatUrl.slice(0, 50)}…` : aiChatUrl}
                    </a>
                  </span>
                </li>
              ) : (
                <li className="flex items-start gap-2">
                  <span className="font-mono text-[10px] text-text-faint">1.</span>
                  <span>Claude.ai (veya başka AI) sohbet sayfasına git.</span>
                </li>
              )}
              <li className="flex items-start gap-2">
                <span className="font-mono text-[10px] text-text-faint">2.</span>
                <span>Sayfada sohbet metnini seç (<kbd className="rounded bg-surface-3 px-1 font-mono text-[10px]">Ctrl/⌘+A</kbd> + <kbd className="rounded bg-surface-3 px-1 font-mono text-[10px]">Ctrl/⌘+C</kbd>).</span>
              </li>
              <li className="flex items-start gap-2">
                <span className="font-mono text-[10px] text-text-faint">3.</span>
                <span>Aşağıya <strong className="font-medium text-text">sohbet metnini</strong> yapıştır — URL değil.</span>
              </li>
            </ol>

            <Textarea
              autoFocus
              rows={12}
              placeholder="Sohbet metnini buraya yapıştır — örn. 'Kullanıcı: Bugün CQRS çalıştım… / Claude: Outbox pattern'a bakmalısın…'"
              value={transcript}
              onChange={(e) => setTranscript(e.target.value)}
              className="font-mono text-[12px]"
            />

            {looksLikeOnlyUrl ? (
              <div className="flex items-start gap-2 rounded-md border border-warning/40 bg-warning-soft p-2.5 text-[12px] text-warning">
                <AlertTriangle className="mt-0.5 h-3.5 w-3.5 shrink-0" />
                <span>
                  Sadece URL yapıştırmışsın. AI URL'i ziyaret etmez — sayfaya gidip <strong>içindeki metni</strong> kopyalayıp buraya yapıştırman gerek.
                </span>
              </div>
            ) : null}

            <p className="text-[11px] text-text-faint">
              Daha uzun konuşmalar daha fazla zaman alır. Yerel modelin pencere büyüklüğüne dikkat — 8K-32K token civarı çoğu modele yeter.
            </p>
          </div>
        ) : totalCount === 0 ? (
          <div className="space-y-3">
            <div className="flex items-start gap-2 rounded-md border border-warning/40 bg-warning-soft p-3 text-[12px] text-warning">
              <AlertTriangle className="mt-0.5 h-4 w-4 shrink-0" />
              <div className="space-y-1">
                <p className="font-medium">AI hiçbir öğe çıkaramadı.</p>
                <p className="text-[11px] opacity-90">
                  Çoğunlukla şu sebeplerden olur:
                </p>
                <ul className="ml-3 list-disc space-y-0.5 text-[11px] opacity-90">
                  <li>Yapıştırılan metin URL'den ibaretti — sayfanın içindeki sohbet metni gerek.</li>
                  <li>Transcript çok kısa (1-2 cümle) — modelin parçalayacak bir şey yok.</li>
                  <li>Model JSON formatına uymadı — daha büyük bir model dene.</li>
                </ul>
              </div>
            </div>
            <Button
              variant="secondary"
              onClick={() => {
                setExtracted(null);
              }}
            >
              Yeniden dene
            </Button>
          </div>
        ) : (
          <div className="space-y-3">
            <Tabs defaultValue="worklogs">
              <TabsList>
                <TabsTrigger value="worklogs">Worklog ({extracted.worklogs.length})</TabsTrigger>
                <TabsTrigger value="decisions">Karar ({extracted.decisions.length})</TabsTrigger>
                <TabsTrigger value="nextSteps">Adım ({extracted.nextSteps.length})</TabsTrigger>
                <TabsTrigger value="ideas">Fikir ({extracted.ideas.length})</TabsTrigger>
                <TabsTrigger value="resources">Kaynak ({extracted.resources.length})</TabsTrigger>
              </TabsList>

              <TabsContent value="worklogs" className="mt-3 max-h-[360px] overflow-y-auto pr-1">
                <PreviewList
                  items={extracted.worklogs}
                  selected={selection.worklogs}
                  onToggle={(i) => toggle("worklogs", i)}
                  render={(w: AiWorklogItem) => (
                    <>
                      <p className="text-[13px] text-text-secondary">{w.whatIDid}</p>
                      {w.whatsLeft ? <p className="mt-0.5 text-[11px] italic text-text-muted">Geriye kalan: {w.whatsLeft}</p> : null}
                    </>
                  )}
                />
              </TabsContent>

              <TabsContent value="decisions" className="mt-3 max-h-[360px] overflow-y-auto pr-1">
                <PreviewList
                  items={extracted.decisions}
                  selected={selection.decisions}
                  onToggle={(i) => toggle("decisions", i)}
                  render={(d: AiDecisionItem) => (
                    <>
                      <p className="text-[13px] font-medium text-text">{d.title}</p>
                      <p className="mt-0.5 text-[12px] text-text-muted">{d.reasoning}</p>
                      {d.alternatives ? <p className="mt-0.5 text-[11px] text-text-faint">Alternatifler: {d.alternatives}</p> : null}
                    </>
                  )}
                />
              </TabsContent>

              <TabsContent value="nextSteps" className="mt-3 max-h-[360px] overflow-y-auto pr-1">
                <PreviewList
                  items={extracted.nextSteps}
                  selected={selection.nextSteps}
                  onToggle={(i) => toggle("nextSteps", i)}
                  render={(s: AiNextStepItem) => (
                    <>
                      <p className="text-[13px] text-text-secondary">{s.description}</p>
                      <span
                        className={
                          "mt-1 inline-flex items-center rounded-md px-1.5 py-0.5 text-[10px] font-medium " +
                          (s.priority === "High"
                            ? "bg-warning text-white"
                            : s.priority === "Medium"
                            ? "bg-info-soft text-info"
                            : "bg-surface-3 text-text-muted")
                        }
                      >
                        {s.priority}
                      </span>
                    </>
                  )}
                />
              </TabsContent>

              <TabsContent value="ideas" className="mt-3 max-h-[360px] overflow-y-auto pr-1">
                <PreviewList
                  items={extracted.ideas}
                  selected={selection.ideas}
                  onToggle={(i) => toggle("ideas", i)}
                  render={(i: AiIdeaItem) => <p className="text-[13px] text-text-secondary">{i.content}</p>}
                />
              </TabsContent>

              <TabsContent value="resources" className="mt-3 max-h-[360px] overflow-y-auto pr-1">
                <PreviewList
                  items={extracted.resources}
                  selected={selection.resources}
                  onToggle={(i) => toggle("resources", i)}
                  render={(r: AiResourceItem) => (
                    <>
                      <p className="text-[13px] font-medium text-text">{r.title}</p>
                      <a href={r.url} target="_blank" rel="noopener noreferrer" className="font-mono text-[11px] text-text-faint underline underline-offset-2 hover:text-text-muted">
                        {r.url}
                      </a>
                      <p className="mt-0.5 text-[11px] text-text-muted">
                        <span className="font-medium">{r.type}</span>
                        {r.notes ? ` · ${r.notes}` : null}
                      </p>
                    </>
                  )}
                />
              </TabsContent>
            </Tabs>
            <p className="text-[11px] text-text-faint">
              {selectedCount} / {totalCount} öğe seçili
            </p>
          </div>
        )}

        <DialogFooter>
          <Button type="button" variant="ghost" onClick={() => onOpenChange(false)}>
            Vazgeç
          </Button>
          {!extracted ? (
            <Button onClick={() => extract.mutate()} disabled={!transcript.trim() || extract.isPending}>
              {extract.isPending ? "Çıkarılıyor…" : (
                <>
                  <Wand2 className="h-4 w-4" /> Çıkar
                </>
              )}
            </Button>
          ) : (
            <Button onClick={() => apply.mutate()} disabled={selectedCount === 0 || apply.isPending}>
              {apply.isPending ? "Kaydediliyor…" : (
                <>
                  Kaydet ({selectedCount}) <ArrowRight className="h-4 w-4" />
                </>
              )}
            </Button>
          )}
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

function PreviewList<T>({
  items,
  selected,
  onToggle,
  render,
}: {
  items: T[];
  selected: Set<number>;
  onToggle: (i: number) => void;
  render: (item: T) => React.ReactNode;
}) {
  if (items.length === 0) {
    return <p className="rounded-md border border-border-subtle bg-surface-1 px-3 py-3 text-[12px] text-text-faint">Bu kategoride öğe çıkarılmadı.</p>;
  }
  return (
    <div className="space-y-1.5">
      {items.map((item, i) => (
        <label
          key={i}
          className="flex cursor-pointer items-start gap-3 rounded-md border border-border-subtle bg-surface-1 p-3 transition-colors hover:bg-surface-2"
        >
          <Checkbox
            className="mt-0.5"
            checked={selected.has(i)}
            onCheckedChange={() => onToggle(i)}
          />
          <div className="min-w-0 flex-1">{render(item)}</div>
        </label>
      ))}
    </div>
  );
}
