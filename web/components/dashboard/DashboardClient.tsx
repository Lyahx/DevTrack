"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowRight, Bell, BookOpen, CheckCircle2, FileText, FolderKanban, Plus, Sparkles, ZapOff } from "lucide-react";
import Link from "next/link";
import { useState } from "react";
import { toast } from "sonner";
import { EmptyState } from "@/components/common/EmptyState";
import { OwnerBadge } from "@/components/common/OwnerBadge";
import { PageHeader } from "@/components/common/PageHeader";
import { ProjectStatusBadge } from "@/components/common/StatusBadge";
import { TagChips } from "@/components/common/TagChips";
import { Button, buttonVariants } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Skeleton } from "@/components/ui/skeleton";
import { ProjectFormModal } from "@/components/projects/ProjectFormModal";
import { dashboardApi } from "@/lib/api/dashboard";
import { nextStepsApi } from "@/lib/api/nextSteps";
import { formatRelative } from "@/lib/date";
import { errorMessage } from "@/lib/error";
import { useAuthStore } from "@/store/auth";
import { cn } from "@/lib/utils";

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
        <Skeleton className="h-40 w-full" />
        <div className="grid grid-cols-1 gap-3 md:grid-cols-2 lg:grid-cols-3">
          {[0, 1, 2].map((i) => <Skeleton key={i} className="h-32" />)}
        </div>
      </div>
    );
  }

  const d = dash.data;
  if (!d) return null;

  const hero = d.activeProjects[0];
  const otherActiveProjects = d.activeProjects.slice(1);

  return (
    <div className="space-y-10">
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
                <Link href={`/projects/${p.id}/resume`} className={cn(buttonVariants({ variant: "outline", size: "sm" }))}>
                  Devam et <ArrowRight className="h-3 w-3" />
                </Link>
              </div>
            ))}
          </CardContent>
        </Card>
      )}

      {hero ? (
        <section>
          <div className="mb-2 flex items-center gap-2 text-xs font-semibold uppercase tracking-wider text-primary/80">
            <Sparkles className="h-3.5 w-3.5" /> Şimdi nereye dönelim
          </div>
          <Card className="relative overflow-hidden border-primary/30 bg-gradient-to-br from-primary/5 via-card to-card shadow-sm ring-1 ring-primary/10">
            <CardHeader className="pb-3">
              <div className="flex items-start justify-between gap-3">
                <CardTitle className="text-2xl font-semibold tracking-tight">{hero.name}</CardTitle>
                <ProjectStatusBadge status={hero.status} />
              </div>
              {hero.goal ? <p className="line-clamp-2 text-sm text-muted-foreground">{hero.goal}</p> : null}
            </CardHeader>
            <CardContent className="flex flex-wrap items-center justify-between gap-3 pt-0">
              <div className="flex flex-wrap items-center gap-3 text-xs text-muted-foreground">
                <span>Son hareket: {formatRelative(hero.lastActivityAt ?? hero.createdAt)}</span>
                {hero.tags.length > 0 ? <TagChips tags={hero.tags} /> : null}
              </div>
              <Link href={`/projects/${hero.id}/resume`} className={cn(buttonVariants())}>
                Resume Mode <ArrowRight className="h-4 w-4" />
              </Link>
            </CardContent>
          </Card>
        </section>
      ) : (
        <EmptyState
          icon={<FolderKanban className="h-6 w-6" />}
          title="Henüz aktif proje yok."
          description="İlk projeni oluştur ve takip etmeye başla."
          action={<Button onClick={() => setCreateOpen(true)}><Plus className="h-4 w-4" /> Yeni proje</Button>}
        />
      )}

      <div className="grid grid-cols-1 gap-8 lg:grid-cols-2">
        {otherActiveProjects.length > 0 && (
          <section>
            <div className="mb-3 flex items-center justify-between">
              <h2 className="text-sm font-semibold uppercase tracking-wider text-muted-foreground">Diğer aktif projeler</h2>
              <Link href="/projects" className="text-xs text-muted-foreground hover:text-foreground">Tümünü gör →</Link>
            </div>
            <div className="space-y-2">
              {otherActiveProjects.slice(0, 4).map((p) => (
                <Link key={p.id} href={`/projects/${p.id}/resume`} className="group block">
                  <Card className="border-border/60 transition-all hover:border-primary/40 hover:shadow-sm">
                    <CardContent className="flex items-center justify-between gap-3 p-3">
                      <div className="min-w-0 flex-1">
                        <div className="flex items-center gap-2">
                          <span className="truncate text-sm font-medium group-hover:text-primary">{p.name}</span>
                          <ProjectStatusBadge status={p.status} />
                        </div>
                        <p className="mt-0.5 text-xs text-muted-foreground">{formatRelative(p.lastActivityAt ?? p.createdAt)}</p>
                      </div>
                      <ArrowRight className="h-4 w-4 shrink-0 text-muted-foreground opacity-0 transition-opacity group-hover:opacity-100" />
                    </CardContent>
                  </Card>
                </Link>
              ))}
            </div>
          </section>
        )}

        <section className={otherActiveProjects.length === 0 ? "lg:col-span-2" : ""}>
          <div className="mb-3 flex items-center justify-between">
            <h2 className="text-sm font-semibold uppercase tracking-wider text-muted-foreground">Aktif eğitimler</h2>
            <Link href="/learning" className="text-xs text-muted-foreground hover:text-foreground">Tümünü gör →</Link>
          </div>
          {d.activeLearningTracks.length === 0 ? (
            <EmptyState icon={<BookOpen className="h-6 w-6" />} title="Henüz aktif eğitim yok." description="Bir Claude eğitimi mi başladın? Burada takip et." />
          ) : (
            <div className="space-y-2">
              {d.activeLearningTracks.slice(0, 4).map((t) => (
                <Link key={t.id} href={`/learning/${t.id}/resume`} className="group block">
                  <Card className="border-border/60 transition-all hover:border-primary/40 hover:shadow-sm">
                    <CardContent className="flex items-center justify-between gap-3 p-3">
                      <div className="min-w-0 flex-1">
                        <div className="flex items-center gap-2">
                          <span className="truncate text-sm font-medium group-hover:text-primary">{t.name}</span>
                          <ProjectStatusBadge status={t.status} />
                        </div>
                        {t.source ? <p className="mt-0.5 text-xs text-muted-foreground">{t.source}</p> : null}
                      </div>
                      <ArrowRight className="h-4 w-4 shrink-0 text-muted-foreground opacity-0 transition-opacity group-hover:opacity-100" />
                    </CardContent>
                  </Card>
                </Link>
              ))}
            </div>
          )}
        </section>
      </div>

      <div className="grid grid-cols-1 gap-8 lg:grid-cols-2">
        <section>
          <div className="mb-3 flex items-center justify-between">
            <h2 className="text-sm font-semibold uppercase tracking-wider text-muted-foreground">Yüksek öncelikli adımlar</h2>
            <span className="text-xs text-muted-foreground">{d.openNextStepsCount} açık</span>
          </div>
          {d.highPriorityOpenNextSteps.length === 0 ? (
            <EmptyState icon={<CheckCircle2 className="h-6 w-6" />} title="Yüksek öncelikli açık adım yok." description="Sakin." />
          ) : (
            <div className="space-y-1.5">
              {d.highPriorityOpenNextSteps.map((s) => (
                <div key={s.id} className="flex items-start gap-3 rounded-md border border-border/50 bg-card/50 p-3 transition-colors hover:bg-card">
                  <Checkbox className="mt-0.5" onCheckedChange={() => completeStep.mutate(s.id)} />
                  <div className="min-w-0 flex-1">
                    <p className="text-sm">{s.description}</p>
                    <div className="mt-1 flex items-center gap-2">
                      <OwnerBadge owner={s.owner} />
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </section>

        <section>
          <div className="mb-3 flex items-center justify-between">
            <h2 className="text-sm font-semibold uppercase tracking-wider text-muted-foreground">Son worklog&apos;lar</h2>
          </div>
          {d.recentWorklogs.length === 0 ? (
            <EmptyState icon={<FileText className="h-6 w-6" />} title="Henüz worklog yok." description="Bir şeye dokunduğunda buraya yansır." />
          ) : (
            <ol className="space-y-2.5 border-l border-border pl-4">
              {d.recentWorklogs.map((w) => (
                <li key={w.id} className="relative">
                  <span className="absolute -left-[1.4rem] top-2.5 inline-block h-2 w-2 rounded-full bg-primary/70" />
                  <div className="rounded-md border border-border/40 bg-card/50 p-3">
                    <div className="flex items-center gap-2 text-xs text-muted-foreground">
                      <span>{formatRelative(w.loggedAt)}</span>
                      <OwnerBadge owner={w.owner} />
                    </div>
                    <p className="mt-1 line-clamp-3 text-sm">{w.whatIDid}</p>
                  </div>
                </li>
              ))}
            </ol>
          )}
        </section>
      </div>

      {d.unreadReminders.length > 0 && (
        <section>
          <div className="mb-3 flex items-center justify-between">
            <h2 className="flex items-center gap-2 text-sm font-semibold uppercase tracking-wider text-muted-foreground">
              <Bell className="h-3.5 w-3.5" /> Okunmamış hatırlatmalar
            </h2>
            <Link href="/reminders" className="text-xs text-muted-foreground hover:text-foreground">Tümünü gör →</Link>
          </div>
          <div className="space-y-2">
            {d.unreadReminders.slice(0, 3).map((r) => (
              <div key={r.id} className="rounded-md border border-amber-300/40 bg-amber-50/30 p-3 dark:border-amber-900/40 dark:bg-amber-950/20">
                <p className="text-sm font-medium">{r.title}</p>
                <p className="text-sm text-muted-foreground">{r.message}</p>
              </div>
            ))}
          </div>
        </section>
      )}

      <ProjectFormModal open={createOpen} onOpenChange={setCreateOpen} />
    </div>
  );
}
