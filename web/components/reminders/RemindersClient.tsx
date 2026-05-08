"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Bell, Check, RefreshCw, Trash2, X } from "lucide-react";
import Link from "next/link";
import { useState } from "react";
import { toast } from "sonner";
import { EmptyState } from "@/components/common/EmptyState";
import { PageHeader } from "@/components/common/PageHeader";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { remindersApi } from "@/lib/api/reminders";
import { formatRelative } from "@/lib/date";
import { errorMessage } from "@/lib/error";

type Filter = "All" | "Unread" | "Dismissed";

export function RemindersClient() {
  const qc = useQueryClient();
  const [filter, setFilter] = useState<Filter>("All");

  const list = useQuery({
    queryKey: ["reminders", filter],
    queryFn: () =>
      remindersApi.list({
        isRead: filter === "Unread" ? false : undefined,
        isDismissed: filter === "Dismissed" ? true : filter === "Unread" ? false : undefined,
        pageSize: 100,
      }),
  });

  const markRead = useMutation({
    mutationFn: (id: number) => remindersApi.markRead(id),
    onMutate: async (id) => {
      const key = ["reminders", filter] as const;
      await qc.cancelQueries({ queryKey: key });
      const prev = qc.getQueryData<typeof list.data>(key);
      qc.setQueryData<typeof list.data>(key, (old) =>
        old ? { ...old, items: old.items.map((r) => (r.id === id ? { ...r, isRead: true } : r)) } : old,
      );
      return { prev };
    },
    onError: (e, _id, ctx) => {
      if (ctx?.prev) qc.setQueryData(["reminders", filter], ctx.prev);
      toast.error(errorMessage(e));
    },
    onSettled: () => qc.invalidateQueries({ queryKey: ["reminders"] }),
  });

  const dismiss = useMutation({
    mutationFn: (id: number) => remindersApi.dismiss(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["reminders"] }),
    onError: (e) => toast.error(errorMessage(e)),
  });
  const remove = useMutation({
    mutationFn: (id: number) => remindersApi.remove(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["reminders"] }),
    onError: (e) => toast.error(errorMessage(e)),
  });
  const runGen = useMutation({
    mutationFn: () => remindersApi.runGenerator(),
    onSuccess: (r) => {
      toast.success(`${r.generated} hatırlatma oluşturuldu.`);
      qc.invalidateQueries({ queryKey: ["reminders"] });
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <div className="space-y-6">
      <PageHeader
        title="Hatırlatmalar"
        description="Sistem 14+ gündür dokunmadığın projeleri burada uyarır."
        actions={
          <Button variant="outline" size="sm" onClick={() => runGen.mutate()} disabled={runGen.isPending}>
            <RefreshCw className="h-4 w-4" /> Şimdi tara
          </Button>
        }
      />
      <Tabs value={filter} onValueChange={(v) => setFilter(v as Filter)}>
        <TabsList>
          <TabsTrigger value="All">Hepsi</TabsTrigger>
          <TabsTrigger value="Unread">Okunmamış</TabsTrigger>
          <TabsTrigger value="Dismissed">Atılan</TabsTrigger>
        </TabsList>
      </Tabs>
      {list.isLoading ? (
        <div className="space-y-3">{[0, 1, 2].map((i) => <Skeleton key={i} className="h-24 w-full" />)}</div>
      ) : !list.data?.items.length ? (
        <EmptyState icon={<Bell className="h-6 w-6" />} title="Hatırlatma yok." description="Sakin." />
      ) : (
        <div className="space-y-3">
          {list.data.items.map((r) => (
            <Card key={r.id} className={r.isRead ? "opacity-70" : "border-amber-300/40"}>
              <CardHeader className="pb-2">
                <div className="flex items-start justify-between gap-2">
                  <CardTitle className="text-base font-semibold">{r.title}</CardTitle>
                  <span className="text-xs text-muted-foreground">{formatRelative(r.generatedAt)}</span>
                </div>
              </CardHeader>
              <CardContent className="space-y-3">
                <p className="text-sm">{r.message}</p>
                <div className="flex items-center gap-2">
                  {r.relatedProjectId ? (
                    <Link href={`/projects/${r.relatedProjectId}/resume`} className="text-xs text-primary hover:underline">
                      Projeye git →
                    </Link>
                  ) : null}
                  {r.relatedLearningTrackId ? (
                    <Link href={`/learning/${r.relatedLearningTrackId}/resume`} className="text-xs text-primary hover:underline">
                      Eğitime git →
                    </Link>
                  ) : null}
                  <div className="ml-auto flex items-center gap-1">
                    {!r.isRead && (
                      <Button size="xs" variant="ghost" onClick={() => markRead.mutate(r.id)}>
                        <Check className="h-3 w-3" /> Okundu
                      </Button>
                    )}
                    {!r.isDismissed && (
                      <Button size="xs" variant="ghost" onClick={() => dismiss.mutate(r.id)}>
                        <X className="h-3 w-3" /> At
                      </Button>
                    )}
                    <Button size="icon-xs" variant="ghost" onClick={() => { if (confirm("Sil?")) remove.mutate(r.id); }}>
                      <Trash2 className="h-3 w-3" />
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
