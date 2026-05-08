"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Check, ChevronLeft, ChevronRight } from "lucide-react";
import { usePathname } from "next/navigation";
import { useMemo, useState } from "react";
import { toast } from "sonner";
import { PriorityBadge } from "@/components/common/StatusBadge";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Skeleton } from "@/components/ui/skeleton";
import { nextStepsApi } from "@/lib/api/nextSteps";
import { errorMessage } from "@/lib/error";
import { useAuthStore } from "@/store/auth";
import type { NextStepResponse } from "@/types/activity";

type PanelScope =
  | { kind: "global" }
  | { kind: "project"; id: number }
  | { kind: "component"; id: number }
  | { kind: "track"; id: number }
  | { kind: "module"; id: number };

function parseScope(pathname: string): PanelScope {
  let m = /^\/learning\/(\d+)\/modules\/(\d+)/.exec(pathname);
  if (m) return { kind: "module", id: Number(m[2]) };
  m = /^\/learning\/(\d+)/.exec(pathname);
  if (m) return { kind: "track", id: Number(m[1]) };
  m = /^\/components\/(\d+)/.exec(pathname);
  if (m) return { kind: "component", id: Number(m[1]) };
  m = /^\/projects\/(\d+)/.exec(pathname);
  if (m) return { kind: "project", id: Number(m[1]) };
  return { kind: "global" };
}

function scopeHeader(scope: PanelScope): string {
  switch (scope.kind) {
    case "project":
      return "Bu projedeki adımlar";
    case "component":
      return "Bu bileşendeki adımlar";
    case "track":
      return "Bu eğitimdeki adımlar";
    case "module":
      return "Bu modüldeki adımlar";
    default:
      return "Açık adımlar";
  }
}

function scopeEmpty(scope: PanelScope): string {
  return scope.kind === "global"
    ? "Açık adım yok. Temiz bir tahta."
    : "Bu kapsamda açık adım yok.";
}

function fetchForScope(scope: PanelScope): Promise<NextStepResponse[]> {
  switch (scope.kind) {
    case "project":
      return nextStepsApi.forProject(scope.id);
    case "component":
      return nextStepsApi.forComponent(scope.id);
    case "track":
      return nextStepsApi.forTrack(scope.id);
    case "module":
      return nextStepsApi.forModule(scope.id);
    default:
      return nextStepsApi.open();
  }
}

export function DetailPanel() {
  const [collapsed, setCollapsed] = useState(false);
  const status = useAuthStore((s) => s.status);
  const pathname = usePathname();
  const qc = useQueryClient();

  const scope = useMemo(() => parseScope(pathname ?? "/"), [pathname]);
  const queryKey = useMemo<readonly unknown[]>(
    () =>
      scope.kind === "global"
        ? ["next-steps", "open"]
        : ["next-steps", "panel-scope", scope.kind, scope.id],
    [scope],
  );

  const list = useQuery({
    queryKey,
    queryFn: () => fetchForScope(scope),
    enabled: status === "authenticated",
  });

  const items = useMemo<NextStepResponse[]>(() => {
    const data = list.data ?? [];
    return scope.kind === "global" ? data : data.filter((s) => !s.isCompleted);
  }, [list.data, scope]);

  const completeMutation = useMutation({
    mutationFn: (id: number) => nextStepsApi.complete(id),
    onMutate: async (id) => {
      await qc.cancelQueries({ queryKey });
      const prev = qc.getQueryData<NextStepResponse[]>(queryKey);
      qc.setQueryData<NextStepResponse[]>(queryKey, (old) => {
        if (!old) return old;
        if (scope.kind === "global") {
          return old.filter((s) => s.id !== id);
        }
        return old.map((s) =>
          s.id === id ? { ...s, isCompleted: true, completedAt: new Date().toISOString() } : s,
        );
      });
      return { prev };
    },
    onError: (e, _id, ctx) => {
      if (ctx?.prev) qc.setQueryData(queryKey, ctx.prev);
      toast.error(errorMessage(e));
    },
    onSettled: () => qc.invalidateQueries({ queryKey: ["next-steps"] }),
  });

  if (collapsed) {
    return (
      <aside className="hidden w-8 shrink-0 border-l bg-sidebar xl:flex xl:flex-col">
        <Button
          variant="ghost"
          size="icon"
          className="m-1"
          onClick={() => setCollapsed(false)}
          aria-label="Detay panelini aç"
        >
          <ChevronLeft className="h-4 w-4" />
        </Button>
      </aside>
    );
  }

  return (
    <aside className="hidden w-80 shrink-0 border-l bg-sidebar xl:flex xl:flex-col">
      <div className="flex items-center justify-between border-b px-4 py-2.5">
        <h3 className="text-sm font-semibold">{scopeHeader(scope)}</h3>
        <Button
          variant="ghost"
          size="icon"
          className="h-7 w-7"
          onClick={() => setCollapsed(true)}
          aria-label="Detay panelini kapat"
        >
          <ChevronRight className="h-4 w-4" />
        </Button>
      </div>
      <ScrollArea className="flex-1">
        <div className="space-y-2 p-3">
          {list.isLoading ? (
            <div className="space-y-2">
              <Skeleton className="h-12 w-full" />
              <Skeleton className="h-12 w-full" />
              <Skeleton className="h-12 w-full" />
            </div>
          ) : !items.length ? (
            <div className="rounded-md border border-dashed p-4 text-center text-sm text-muted-foreground">
              <Check className="mx-auto mb-2 h-5 w-5" />
              {scopeEmpty(scope)}
            </div>
          ) : (
            items.slice(0, 12).map((s) => (
              <div key={s.id} className="flex items-start gap-2 rounded-md border bg-card p-2">
                <Checkbox
                  checked={false}
                  className="mt-0.5"
                  onCheckedChange={() => completeMutation.mutate(s.id)}
                />
                <div className="flex min-w-0 flex-1 flex-col gap-1">
                  <span className="line-clamp-2 text-sm">{s.description}</span>
                  <PriorityBadge priority={s.priority} />
                </div>
              </div>
            ))
          )}
        </div>
      </ScrollArea>
    </aside>
  );
}
