"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowRight, Bell, Plus, ZapOff } from "lucide-react";
import Link from "next/link";
import { useState } from "react";
import { toast } from "sonner";
import { EmptyState } from "@/components/common/EmptyState";
import { OwnerBadge } from "@/components/common/OwnerBadge";
import { PageHeader } from "@/components/common/PageHeader";
import { ProjectStatusBadge } from "@/components/common/StatusBadge";
import { TagChips } from "@/components/common/TagChips";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Skeleton } from "@/components/ui/skeleton";
import { ProjectFormModal } from "@/components/projects/ProjectFormModal";
import { dashboardApi } from "@/lib/api/dashboard";
import { nextStepsApi } from "@/lib/api/nextSteps";
import { formatRelative } from "@/lib/date";
import { errorMessage } from "@/lib/error";
import { useAuthStore } from "@/store/auth";

export function DashboardClient() {
  const user = useAuthStore((s) => s.user);
  const qc = useQueryClient();
  const [createOpen, setCreateOpen] = useState(false);

  const dash = useQuery({
    queryKey: ["dashboard"],
    queryFn: () => dashboardApi.get(),
  });

  const completeStep = useMutation({
    mutationFn: (id: number) => nextStepsApi.complete(id),
    onMutate: async (id) => {
      await qc.cancelQueries({ queryKey: ["dashboard"] });
      const prev = qc.getQueryData<typeof dash.data>(["dashboard"]);
      qc.setQueryData<typeof dash.data>(["dashboard"], (old) =>
        old ? { ...old, highPriorityOpenNextSteps: old.highPriorityOpenNextSteps.filter((s) => s.id !== id), openNextStepsCount: Math.max(0, old.openNextStepsCount - 1) } : old,
      );
      return { prev };
    },
    onError: (e, _id, ctx) => {
      if (ctx?.prev) qc.setQueryData(["dashboard"], ctx.prev);
      toast.error(errorMessage(e));
    },
    onSettled: () => qc.invalidateQueries({ queryKey: ["dashboard", "next-steps"] }),
  });

  if (dash.isLoading) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-10 w-64" />
        <div className="grid grid-cols-1 gap-3 md:grid-cols-2 lg:grid-cols-3">
          {[0, 1, 2].map((i) => <Skeleton key={i} className="h-32" />)}
        </div>
      </div>
    );
  }

  const d = dash.data;
  if (!d) return null;

  return (
    <div className="space-y-8">
      <PageHeader
        title={`Hoş geldin, ${user?.username ?? ""}`}
        description="Bugün nereden devam edelim?"
        actions={
          <Button onClick={() => setCreateOpen(true)}>
            <Plus className="h-4 w-4" /> Yeni proje
          </Button>
        }
      />

      {d.staleProjects.length > 0 && (
        <Card className="border-amber-300/60 bg-amber-50/50 dark:border-amber-900/60 dark:bg-amber-950/30">
          <CardHeader className="pb-2">
            <CardTitle className="flex items-center gap-2 text-amber-900 dark:text-amber-300">
              <ZapOff className="h-4 w-4" />
              {d.staleProjects.length} proje 14+ gündür duruyor
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            {d.staleProjects.slice(0, 5).map((p) => (
              <div key={p.id} className="flex items-center justify-between gap-3 rounded-md bg-background/60 p-2">
                <div className="flex min-w-0 flex-1 items-center gap-2">
                  <ProjectStatusBadge status={p.status} />
                  <span className="truncate font-medium">{p.name}</span>
                  <span className="text-xs text-muted-foreground">
                    {p.lastActivityAt ? formatRelative(p.lastActivityAt) : "Hiç dokunulmadı"}
                  </span>
                </div>
                <Link href={`/projects/${p.id}/resume`}>
                  <Button size="sm" variant="outline">Devam et <ArrowRight className="h-3 w-3" /></Button>
                </Link>
              </div>
            ))}
          </CardContent>
        </Card>
      )}

      <section>
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-lg font-medium">Aktif projeler</h2>
          <Link href="/projects" className="text-sm text-muted-foreground hover:text-foreground">Tümünü gör →</Link>
        </div>
        {d.activeProjects.length === 0 ? (
          <EmptyState
            title="Henüz aktif proje yok."
            description="İlk projeni oluştur ve takip etmeye başla."
            action={<Button onClick={() => setCreateOpen(true)}><Plus className="h-4 w-4" /> Yeni proje</Button>}
          />
        ) : (
          <div className="grid grid-cols-1 gap-3 md:grid-cols-2 lg:grid-cols-3">
            {d.activeProjects.map((p) => (
              <Link key={p.id} href={`/projects/${p.id}/resume`} className="group">
                <Card className="h-full transition-colors group-hover:border-primary/50">
                  <CardHeader className="pb-2">
                    <div className="flex items-start justify-between gap-2">
                      <CardTitle className="text-base font-semibold group-hover:text-primary">{p.name}</CardTitle>
                      <ProjectStatusBadge status={p.status} />
                    </div>
                    {p.goal ? <p className="line-clamp-2 text-sm text-muted-foreground">{p.goal}</p> : null}
                  </CardHeader>
                  <CardContent className="flex items-center justify-between pt-0">
                    <span className="text-xs text-muted-foreground">{formatRelative(p.lastActivityAt ?? p.createdAt)}</span>
                    <TagChips tags={p.tags} />
                  </CardContent>
                </Card>
              </Link>
            ))}
          </div>
        )}
      </section>

      <section>
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-lg font-medium">Aktif eğitimler</h2>
          <Link href="/learning" className="text-sm text-muted-foreground hover:text-foreground">Tümünü gör →</Link>
        </div>
        {d.activeLearningTracks.length === 0 ? (
          <EmptyState title="Henüz aktif eğitim yok." description="Bir Claude eğitimi mi başladın? Burada takip et." />
        ) : (
          <div className="grid grid-cols-1 gap-3 md:grid-cols-2 lg:grid-cols-3">
            {d.activeLearningTracks.map((t) => (
              <Link key={t.id} href={`/learning/${t.id}/resume`} className="group">
                <Card className="h-full transition-colors group-hover:border-primary/50">
                  <CardHeader className="pb-2">
                    <div className="flex items-start justify-between gap-2">
                      <CardTitle className="text-base font-semibold group-hover:text-primary">{t.name}</CardTitle>
                      <ProjectStatusBadge status={t.status} />
                    </div>
                    {t.source ? <p className="text-xs text-muted-foreground">{t.source}</p> : null}
                  </CardHeader>
                  <CardContent className="flex items-center justify-between pt-0">
                    <span className="text-xs text-muted-foreground">{formatRelative(t.lastActivityAt ?? t.createdAt)}</span>
                    <TagChips tags={t.tags} />
                  </CardContent>
                </Card>
              </Link>
            ))}
          </div>
        )}
      </section>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <section>
          <div className="mb-3 flex items-center justify-between">
            <h2 className="text-lg font-medium">Son worklog&apos;lar</h2>
          </div>
          {d.recentWorklogs.length === 0 ? (
            <EmptyState title="Henüz worklog yok." description="Bir şeye dokunduğunda buraya yansır." />
          ) : (
            <ol className="space-y-3 border-l border-border pl-4">
              {d.recentWorklogs.map((w) => (
                <li key={w.id} className="relative">
                  <span className="absolute -left-[1.4rem] top-2 inline-block h-2 w-2 rounded-full bg-primary" />
                  <Card>
                    <CardContent className="space-y-1.5 p-3">
                      <div className="flex items-center gap-2 text-xs text-muted-foreground">
                        <span>{formatRelative(w.loggedAt)}</span>
                        <OwnerBadge owner={w.owner} />
                      </div>
                      <p className="line-clamp-3 text-sm">{w.whatIDid}</p>
                    </CardContent>
                  </Card>
                </li>
              ))}
            </ol>
          )}
        </section>

        <section>
          <div className="mb-3 flex items-center justify-between">
            <h2 className="text-lg font-medium">Yüksek öncelikli açık adımlar</h2>
            <span className="text-xs text-muted-foreground">{d.openNextStepsCount} açık adım</span>
          </div>
          {d.highPriorityOpenNextSteps.length === 0 ? (
            <EmptyState title="Yüksek öncelikli açık adım yok." description="Sakin." />
          ) : (
            <div className="space-y-2">
              {d.highPriorityOpenNextSteps.map((s) => (
                <Card key={s.id}>
                  <CardContent className="flex items-start gap-3 p-3">
                    <Checkbox className="mt-0.5" onCheckedChange={() => completeStep.mutate(s.id)} />
                    <div className="flex-1">
                      <p className="text-sm">{s.description}</p>
                      <div className="mt-1 flex items-center gap-2">
                        <OwnerBadge owner={s.owner} />
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          )}
        </section>
      </div>

      {d.unreadReminders.length > 0 && (
        <section>
          <div className="mb-3 flex items-center justify-between">
            <h2 className="text-lg font-medium flex items-center gap-2">
              <Bell className="h-4 w-4" /> Okunmamış hatırlatmalar
            </h2>
            <Link href="/reminders" className="text-sm text-muted-foreground hover:text-foreground">Tümünü gör →</Link>
          </div>
          <div className="space-y-2">
            {d.unreadReminders.slice(0, 3).map((r) => (
              <Card key={r.id} className="border-amber-300/40">
                <CardContent className="space-y-1 p-3">
                  <p className="text-sm font-medium">{r.title}</p>
                  <p className="text-sm text-muted-foreground">{r.message}</p>
                </CardContent>
              </Card>
            ))}
          </div>
        </section>
      )}

      <ProjectFormModal open={createOpen} onOpenChange={setCreateOpen} />
    </div>
  );
}
