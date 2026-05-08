"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { FileText, Pencil, Plus, Trash2 } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";
import { EmptyState } from "@/components/common/EmptyState";
import type { Scope } from "@/components/common/Scope";
import { scopeKey } from "@/components/common/Scope";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { worklogsApi } from "@/lib/api/worklogs";
import { formatDateTime, formatRelative } from "@/lib/date";
import { errorMessage } from "@/lib/error";
import type { WorklogResponse } from "@/types/activity";
import { WorklogForm } from "./WorklogForm";

function fetcher(scope: Scope) {
  switch (scope.kind) {
    case "project":
      return worklogsApi.forProject(scope.id);
    case "component":
      return worklogsApi.forComponent(scope.id);
    case "track":
      return worklogsApi.forTrack(scope.id);
    case "module":
      return worklogsApi.forModule(scope.id);
  }
}

export function WorklogTab({ scope }: { scope: Scope }) {
  const qc = useQueryClient();
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<WorklogResponse | null>(null);

  const list = useQuery({
    queryKey: ["worklogs", "by-scope", ...scopeKey(scope)],
    queryFn: () => fetcher(scope),
  });

  const remove = useMutation({
    mutationFn: (id: number) => worklogsApi.remove(id),
    onSuccess: () => {
      toast.success("Worklog silindi.");
      qc.invalidateQueries({ queryKey: ["worklogs"] });
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-medium">Worklog&apos;lar</h2>
        <Button
          size="sm"
          onClick={() => {
            setEditing(null);
            setFormOpen(true);
          }}
        >
          <Plus className="h-4 w-4" /> Yeni worklog
        </Button>
      </div>

      {list.isLoading ? (
        <div className="space-y-3">
          {[0, 1, 2].map((i) => <Skeleton key={i} className="h-24 w-full" />)}
        </div>
      ) : !list.data?.length ? (
        <EmptyState
          icon={<FileText className="h-6 w-6" />}
          title="Henüz worklog yok."
          description="Bugün ne ilerledi? Hızlıca bir kayıt al."
          action={
            <Button
              size="sm"
              onClick={() => {
                setEditing(null);
                setFormOpen(true);
              }}
            >
              <Plus className="h-4 w-4" /> İlk worklog
            </Button>
          }
        />
      ) : (
        <div className="space-y-3">
          {list.data.map((w) => (
            <Card key={w.id}>
              <CardHeader className="flex flex-row items-start justify-between gap-2 pb-2">
                <CardTitle className="text-sm font-medium text-muted-foreground" title={formatDateTime(w.loggedAt)}>
                  {formatRelative(w.loggedAt)}
                </CardTitle>
                <div className="flex items-center gap-1">
                  <Button
                    size="icon-xs"
                    variant="ghost"
                    onClick={() => {
                      setEditing(w);
                      setFormOpen(true);
                    }}
                    aria-label="Düzenle"
                  >
                    <Pencil className="h-3 w-3" />
                  </Button>
                  <Button
                    size="icon-xs"
                    variant="ghost"
                    onClick={() => {
                      if (confirm("Sil?")) remove.mutate(w.id);
                    }}
                    aria-label="Sil"
                  >
                    <Trash2 className="h-3 w-3" />
                  </Button>
                </div>
              </CardHeader>
              <CardContent className="space-y-2 pt-0">
                <p className="whitespace-pre-wrap text-sm">{w.whatIDid}</p>
                {w.whatsLeft ? (
                  <div className="rounded-md bg-muted/50 p-2 text-xs text-muted-foreground">
                    <span className="font-medium">Geriye kalan:</span> {w.whatsLeft}
                  </div>
                ) : null}
                {w.reasoning ? (
                  <div className="rounded-md border-l-2 border-primary/40 bg-primary/5 px-3 py-2 text-xs text-text-secondary">
                    <span className="font-medium text-text">Neden:</span> <span className="whitespace-pre-wrap">{w.reasoning}</span>
                  </div>
                ) : null}
                {w.alternatives ? (
                  <p className="text-xs text-text-muted">
                    <span className="font-medium text-text-secondary">Alternatifler:</span> {w.alternatives}
                  </p>
                ) : null}
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      <WorklogForm
        open={formOpen}
        onOpenChange={setFormOpen}
        scope={scope}
        initial={editing ?? undefined}
      />
    </div>
  );
}
