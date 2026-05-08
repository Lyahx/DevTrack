"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowRight, Bell, BookOpen, ChevronRight, FolderKanban, Plus, Sparkles } from "lucide-react";
import Link from "next/link";
import { useState } from "react";
import { toast } from "sonner";
import { EmptyState } from "@/components/common/EmptyState";
import { OwnerBadge } from "@/components/common/OwnerBadge";
import { PageHeader } from "@/components/common/PageHeader";
import { ProjectStatusBadge } from "@/components/common/StatusBadge";
import { TagChips } from "@/components/common/TagChips";
import { Button, buttonVariants } from "@/components/ui/button";
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
  const [staleExpanded, setStaleExpanded] = useState(false);

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
        <Skeleton className="h-8 w-64" />
        <div className="grid grid-cols-1 gap-3 md:grid-cols-2 xl:grid-cols-3">
          {[0, 1, 2].map((i) => <Skeleton key={i} className="h-28" />)}
        </div>
      </div>
    );
  }

  const d = dash.data;
  if (!d) return null;

  const hero = d.activeProjects[0];
  const otherProjects = d.activeProjects.slice(1);

  return (
    <div className="space-y-6">
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
        <div className="overflow-hidden rounded-md border border-border-subtle bg-surface-2">
          <button
            type="button"
            onClick={() => setStaleExpanded((v) => !v)}
            className="flex w-full items-center gap-3 border-l-2 border-warning px-4 py-3 text-left transition-colors hover:bg-surface-3"
          >
            <span className="flex-1 text-[13px] text-text-secondary">
              <span className="font-medium text-text">{d.staleProjects.length} projeye</span> 14+ gündür dokunmadın.
            </span>
            <ChevronRight className={`h-4 w-4 text-text-muted transition-transform ${staleExpanded ? "rotate-90" : ""}`} />
          </button>
          {staleExpanded && (
            <div className="space-y-1 border-t border-border-subtle p-2">
              {d.staleProjects.slice(0, 8).map((p) => (
                <Link
                  key={p.id}
                  href={`/projects/${p.id}/resume`}
                  className="flex items-center justify-between gap-3 rounded-md px-2 py-1.5 hover:bg-surface-3"
                >
                  <div className="flex min-w-0 flex-1 items-center gap-3">
                    <ProjectStatusBadge status={p.status} />
                    <span className="truncate text-[13px] text-text-secondary">{p.name}</span>
                    <span className="font-mono text-[10px] text-text-faint">
                      {p.lastActivityAt ? formatRelative(p.lastActivityAt) : "Hiç dokunulmadı"}
                    </span>
                  </div>
                  <ChevronRight className="h-3.5 w-3.5 text-text-faint" />
                </Link>
              ))}
            </div>
          )}
        </div>
      )}

      {hero ? (
        <section className="space-y-2">
          <div className="flex items-center gap-2 px-1 text-[10px] font-semibold uppercase tracking-wider text-primary">
            <Sparkles className="h-3 w-3" /> Şimdi nereye dönelim
          </div>
          <Link
            href={`/projects/${hero.id}/resume`}
            className="group block overflow-hidden rounded-lg border border-border bg-gradient-to-br from-primary/10 via-surface-1 to-surface-1 p-5 shadow-card ring-1 ring-primary/20 transition-all hover:ring-primary/40"
          >
            <div className="flex flex-col gap-3">
              <div className="flex items-start justify-between gap-3">
                <div className="space-y-1.5">
                  <h2 className="text-[20px] font-medium leading-tight tracking-tight text-text">{hero.name}</h2>
                  {hero.goal ? <p className="line-clamp-2 max-w-2xl text-[13px] text-text-secondary">{hero.goal}</p> : null}
                </div>
                <ProjectStatusBadge status={hero.status} />
              </div>
              <div className="flex flex-wrap items-center justify-between gap-3 pt-1">
                <div className="flex flex-wrap items-center gap-3 text-[12px] text-text-muted">
                  <span>Son hareket <span className="font-mono text-text-secondary">{formatRelative(hero.lastActivityAt ?? hero.createdAt)}</span></span>
                  {hero.tags.length > 0 ? <TagChips tags={hero.tags} max={4} /> : null}
                </div>
                <span className={cn(buttonVariants({ variant: "default", size: "sm" }), "transition-transform group-hover:translate-x-0.5")}>
                  Resume Mode <ArrowRight className="h-3.5 w-3.5" />
                </span>
              </div>
            </div>
          </Link>
        </section>
      ) : (
        <EmptyState
          icon={<FolderKanban className="h-6 w-6" />}
          title="Henüz aktif proje yok."
          description="İlk projeni oluştur ve takip etmeye başla."
          action={<Button onClick={() => setCreateOpen(true)}><Plus className="h-4 w-4" /> Yeni proje</Button>}
        />
      )}

      {otherProjects.length > 0 && (
        <section className="space-y-3">
          <div className="flex items-center justify-between">
            <h2 className="text-[10px] font-semibold uppercase tracking-wider text-text-faint">Diğer aktif projeler</h2>
            <Link href="/projects" className="text-[11px] text-text-faint hover:text-text-secondary">Tümünü gör →</Link>
          </div>
          <div className="grid grid-cols-1 gap-2 md:grid-cols-2 xl:grid-cols-3">
            {otherProjects.slice(0, 6).map((p) => (
              <Link key={p.id} href={`/projects/${p.id}/resume`} className="group block">
                <div className="flex h-full flex-col gap-2 rounded-md border border-border-subtle bg-surface-1 p-4 shadow-soft transition-all hover:bg-surface-2 hover:border-border hover:shadow-card hover:-translate-y-px">
                  <div className="flex items-start justify-between gap-2">
                    <ProjectStatusBadge status={p.status} />
                    <span className="font-mono text-[10px] text-text-faint">{formatRelative(p.lastActivityAt ?? p.createdAt)}</span>
                  </div>
                  <p className="text-[14px] font-medium text-text">{p.name}</p>
                  {p.goal ? <p className="line-clamp-2 text-[12px] text-text-muted">{p.goal}</p> : null}
                  {p.tags.length > 0 ? <TagChips tags={p.tags} max={3} /> : null}
                </div>
              </Link>
            ))}
          </div>
        </section>
      )}

      <section className="space-y-3">
        <div className="flex items-center justify-between">
          <h2 className="text-[10px] font-semibold uppercase tracking-wider text-text-faint">Aktif eğitimler</h2>
          <Link href="/learning" className="text-[11px] text-text-faint hover:text-text-secondary">Tümünü gör →</Link>
        </div>
        {d.activeLearningTracks.length === 0 ? (
          <EmptyState icon={<BookOpen className="h-6 w-6" />} title="Henüz aktif eğitim yok." description="Bir Claude eğitimi mi başladın? Burada takip et." />
        ) : (
          <div className="grid grid-cols-1 gap-2 md:grid-cols-2 xl:grid-cols-3">
            {d.activeLearningTracks.slice(0, 6).map((t) => (
              <Link key={t.id} href={`/learning/${t.id}/resume`} className="group block">
                <div className="flex h-full flex-col gap-2 rounded-md border border-border-subtle bg-surface-1 p-4 shadow-soft transition-all hover:bg-surface-2 hover:border-border hover:shadow-card hover:-translate-y-px">
                  <div className="flex items-start justify-between gap-2">
                    <ProjectStatusBadge status={t.status} />
                    <span className="font-mono text-[10px] text-text-faint">{formatRelative(t.lastActivityAt ?? t.createdAt)}</span>
                  </div>
                  <p className="text-[14px] font-medium text-text">{t.name}</p>
                  {t.source ? <p className="font-mono text-[11px] text-text-faint">{t.source}</p> : null}
                  {t.tags.length > 0 ? <TagChips tags={t.tags} max={3} /> : null}
                </div>
              </Link>
            ))}
          </div>
        )}
      </section>

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <section className="space-y-3">
          <div className="flex items-center justify-between">
            <h2 className="text-[10px] font-semibold uppercase tracking-wider text-text-faint">Yüksek öncelikli adımlar</h2>
            <span className="font-mono text-[10px] text-text-faint">{d.openNextStepsCount} açık</span>
          </div>
          {d.highPriorityOpenNextSteps.length === 0 ? (
            <p className="rounded-md border border-border-subtle bg-surface-1 px-3 py-2.5 text-[12px] text-text-faint">Sakin. Açık high-priority adım yok.</p>
          ) : (
            <div className="space-y-1.5">
              {d.highPriorityOpenNextSteps.map((s) => (
                <div key={s.id} className="flex items-start gap-3 rounded-md border border-border-subtle bg-surface-1 p-3 transition-colors hover:bg-surface-2">
                  <Checkbox className="mt-0.5" onCheckedChange={() => completeStep.mutate(s.id)} />
                  <div className="min-w-0 flex-1">
                    <p className="text-[13px] text-text-secondary">{s.description}</p>
                    <div className="mt-1"><OwnerBadge owner={s.owner} /></div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </section>

        <section className="space-y-3">
          <h2 className="text-[10px] font-semibold uppercase tracking-wider text-text-faint">Son worklog&apos;lar</h2>
          {d.recentWorklogs.length === 0 ? (
            <p className="rounded-md border border-border-subtle bg-surface-1 px-3 py-2.5 text-[12px] text-text-faint">Henüz worklog yok.</p>
          ) : (
            <ol className="space-y-2.5 border-l border-border pl-4">
              {d.recentWorklogs.map((w) => (
                <li key={w.id} className="relative">
                  <span className="absolute -left-[1.4rem] top-2.5 inline-block h-2 w-2 rounded-full bg-border-strong" />
                  <div className="rounded-md border border-border-subtle bg-surface-1 p-3">
                    <div className="flex items-center gap-2">
                      <span className="font-mono text-[10px] text-text-faint">{formatRelative(w.loggedAt)}</span>
                      <OwnerBadge owner={w.owner} />
                    </div>
                    <p className="mt-1 line-clamp-3 text-[13px] text-text-secondary">{w.whatIDid}</p>
                  </div>
                </li>
              ))}
            </ol>
          )}
        </section>
      </div>

      {d.unreadReminders.length > 0 && (
        <section className="space-y-3">
          <div className="flex items-center justify-between">
            <h2 className="flex items-center gap-2 text-[10px] font-semibold uppercase tracking-wider text-text-faint">
              <Bell className="h-3 w-3" /> Okunmamış hatırlatmalar
            </h2>
            <Link href="/reminders" className="text-[11px] text-text-faint hover:text-text-secondary">Tümünü gör →</Link>
          </div>
          <div className="space-y-1.5">
            {d.unreadReminders.slice(0, 3).map((r) => (
              <div key={r.id} className="rounded-md border border-border-subtle border-l-2 border-l-warning bg-surface-1 p-3">
                <p className="text-[13px] font-medium text-text">{r.title}</p>
                <p className="mt-0.5 text-[12px] text-text-muted">{r.message}</p>
              </div>
            ))}
          </div>
        </section>
      )}

      <ProjectFormModal open={createOpen} onOpenChange={setCreateOpen} />
    </div>
  );
}
