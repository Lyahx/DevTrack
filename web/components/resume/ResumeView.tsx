"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { ArrowRight, ExternalLink } from "lucide-react";
import Link from "next/link";
import { toast } from "sonner";
import { ResourceTypeBadge, ModuleStatusBadge } from "@/components/common/StatusBadge";
import { TagChips } from "@/components/common/TagChips";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Separator } from "@/components/ui/separator";
import { ideasApi } from "@/lib/api/ideas";
import { nextStepsApi } from "@/lib/api/nextSteps";
import { daysSinceLabel, formatDateTime, formatRelative } from "@/lib/date";
import { errorMessage } from "@/lib/error";
import type {
  DecisionResponse,
  IdeaResponse,
  NextStepResponse,
  ResourceResponse,
  WorklogResponse,
} from "@/types/activity";
import type { ComponentResponse } from "@/types/component";
import type { LearningModuleResponse } from "@/types/learning";

type Header = {
  title: string;
  subtitle?: string | null;
  badge?: React.ReactNode;
  daysSince: number | null;
  tags?: { id: number; name: string; color?: string | null }[];
  continueHref?: string;
  continueLabel?: string;
};

export function ResumeView({
  header,
  components,
  modules,
  recentWorklogs,
  openNextSteps,
  recentDecisions,
  resources,
  recentIdeas,
  progressPercent,
}: {
  header: Header;
  components?: ComponentResponse[];
  modules?: LearningModuleResponse[];
  recentWorklogs: WorklogResponse[];
  openNextSteps: NextStepResponse[];
  recentDecisions: DecisionResponse[];
  resources: ResourceResponse[];
  recentIdeas: IdeaResponse[];
  progressPercent?: number;
}) {
  const qc = useQueryClient();

  const completeStep = useMutation({
    mutationFn: (id: number) => nextStepsApi.complete(id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["resume"] });
      qc.invalidateQueries({ queryKey: ["next-steps"] });
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const convertIdea = useMutation({
    mutationFn: (id: number) => ideasApi.convert(id, { priority: "Medium" }),
    onSuccess: () => {
      toast.success("Adıma dönüştü.");
      qc.invalidateQueries({ queryKey: ["resume"] });
      qc.invalidateQueries({ queryKey: ["ideas"] });
      qc.invalidateQueries({ queryKey: ["next-steps"] });
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  const groupedResources = resources.reduce<Record<string, ResourceResponse[]>>((acc, r) => {
    (acc[r.type] = acc[r.type] ?? []).push(r);
    return acc;
  }, {});

  return (
    <article className="mx-auto max-w-3xl space-y-12 py-4">
      <header className="space-y-3">
        <div className="flex flex-wrap items-center gap-3">
          <h1 className="text-3xl font-semibold tracking-tight">{header.title}</h1>
          {header.badge}
        </div>
        {header.subtitle ? <p className="text-base text-muted-foreground">{header.subtitle}</p> : null}
        <div className="flex flex-wrap items-center gap-3 text-sm text-muted-foreground">
          <span>{daysSinceLabel(header.daysSince)}</span>
          {progressPercent !== undefined ? (
            <>
              <Separator orientation="vertical" className="h-4" />
              <span>İlerleme: {progressPercent}%</span>
            </>
          ) : null}
          {header.tags && header.tags.length > 0 ? (
            <>
              <Separator orientation="vertical" className="h-4" />
              <TagChips tags={header.tags as never[]} />
            </>
          ) : null}
        </div>
      </header>

      {components && components.length > 0 ? (
        <section className="space-y-3">
          <h2 className="text-lg font-medium text-muted-foreground">📍 Şu an nerede kaldım</h2>
          <div className="space-y-3">
            {components.map((c) => (
              <Card key={c.id} className="bg-muted/30">
                <CardHeader className="pb-2">
                  <CardTitle className="text-base">
                    <Link href={`/components/${c.id}`} className="hover:text-primary">{c.name}</Link>
                    {c.techStack ? <span className="ml-2 text-xs font-normal text-muted-foreground">{c.techStack}</span> : null}
                  </CardTitle>
                </CardHeader>
                <CardContent className="text-sm">
                  {c.currentStatusNote ? (
                    <p className="whitespace-pre-wrap leading-relaxed">{c.currentStatusNote}</p>
                  ) : (
                    <p className="italic text-muted-foreground">Bu bileşene henüz not eklenmemiş.</p>
                  )}
                </CardContent>
              </Card>
            ))}
          </div>
        </section>
      ) : null}

      {modules && modules.length > 0 ? (
        <section className="space-y-3">
          <h2 className="text-lg font-medium text-muted-foreground">📚 Modüller</h2>
          <div className="space-y-2">
            {modules.map((m) => (
              <Link
                key={m.id}
                href={`/learning/${m.learningTrackId}/modules/${m.id}`}
                className="group flex items-center gap-3 rounded-md border bg-card p-3"
              >
                <span className="w-6 text-xs text-muted-foreground">#{m.order}</span>
                <span className="flex-1 group-hover:text-primary">{m.name}</span>
                <ModuleStatusBadge status={m.status} />
              </Link>
            ))}
          </div>
        </section>
      ) : null}

      <section className="space-y-3">
        <h2 className="text-lg font-medium text-muted-foreground">📝 Son worklog&apos;lar</h2>
        {recentWorklogs.length === 0 ? (
          <Card><CardContent className="py-4 text-sm text-muted-foreground">Henüz worklog yok.</CardContent></Card>
        ) : (
          <ol className="space-y-3 border-l border-border pl-5">
            {recentWorklogs.map((w) => (
              <li key={w.id} className="relative">
                <span className="absolute -left-[1.65rem] top-2 inline-block h-2.5 w-2.5 rounded-full bg-primary" />
                <Card>
                  <CardContent className="space-y-1 py-3">
                    <p className="text-xs text-muted-foreground" title={formatDateTime(w.loggedAt)}>{formatRelative(w.loggedAt)}</p>
                    <p className="whitespace-pre-wrap leading-relaxed">{w.whatIDid}</p>
                    {w.whatsLeft ? (
                      <p className="text-xs italic text-muted-foreground">Geriye kalan: {w.whatsLeft}</p>
                    ) : null}
                  </CardContent>
                </Card>
              </li>
            ))}
          </ol>
        )}
      </section>

      <section className="space-y-3">
        <h2 className="text-lg font-medium text-muted-foreground">✅ Açık adımlar ({openNextSteps.length})</h2>
        {openNextSteps.length === 0 ? (
          <Card><CardContent className="py-4 text-sm text-muted-foreground">Açık adım yok. Sakin.</CardContent></Card>
        ) : (
          <div className="space-y-2">
            {openNextSteps.map((s) => (
              <Card key={s.id}>
                <CardContent className="flex items-start gap-3 py-3">
                  <Checkbox className="mt-0.5" onCheckedChange={() => completeStep.mutate(s.id)} />
                  <div className="flex-1">
                    <p>{s.description}</p>
                    <p className="text-xs text-muted-foreground">{s.priority}</p>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </section>

      <section className="space-y-3">
        <h2 className="text-lg font-medium text-muted-foreground">💡 Son kararlar</h2>
        {recentDecisions.length === 0 ? (
          <Card><CardContent className="py-4 text-sm text-muted-foreground">Henüz karar yok.</CardContent></Card>
        ) : (
          <div className="space-y-3">
            {recentDecisions.map((d) => (
              <details key={d.id} className="group rounded-md border bg-card">
                <summary className="cursor-pointer list-none p-3">
                  <div className="flex items-center justify-between">
                    <span className="font-medium">{d.title}</span>
                    <span className="text-xs text-muted-foreground">{formatRelative(d.decidedAt)}</span>
                  </div>
                </summary>
                <div className="border-t px-3 py-2 text-sm">
                  <p className="whitespace-pre-wrap">{d.reasoning}</p>
                  {d.alternatives ? (
                    <p className="mt-2 text-xs text-muted-foreground"><span className="font-medium">Alternatifler:</span> {d.alternatives}</p>
                  ) : null}
                </div>
              </details>
            ))}
          </div>
        )}
      </section>

      <section className="space-y-3">
        <h2 className="text-lg font-medium text-muted-foreground">🔗 Kaynaklar ({resources.length})</h2>
        {resources.length === 0 ? (
          <Card><CardContent className="py-4 text-sm text-muted-foreground">Henüz kaynak yok.</CardContent></Card>
        ) : (
          <div className="space-y-4">
            {Object.entries(groupedResources).map(([type, list]) => (
              <div key={type} className="space-y-2">
                <ResourceTypeBadge type={type as never} />
                <div className="grid grid-cols-1 gap-2 md:grid-cols-2">
                  {list.map((r) => (
                    <a
                      key={r.id}
                      href={r.url}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="flex items-start gap-2 rounded-md border bg-card p-3 hover:border-primary/50"
                    >
                      <ExternalLink className="mt-0.5 h-4 w-4 shrink-0 text-muted-foreground" />
                      <div className="min-w-0">
                        <p className="truncate text-sm font-medium">{r.title}</p>
                        {r.notes ? <p className="line-clamp-2 text-xs text-muted-foreground">{r.notes}</p> : null}
                      </div>
                    </a>
                  ))}
                </div>
              </div>
            ))}
          </div>
        )}
      </section>

      <section className="space-y-3">
        <h2 className="text-lg font-medium text-muted-foreground">✨ Yakalanmış fikirler</h2>
        {recentIdeas.length === 0 ? (
          <Card><CardContent className="py-4 text-sm text-muted-foreground">Açık fikir yok.</CardContent></Card>
        ) : (
          <div className="space-y-2">
            {recentIdeas.map((i) => (
              <Card key={i.id}>
                <CardContent className="flex items-start justify-between gap-3 py-3">
                  <p className="flex-1 text-sm">{i.content}</p>
                  <Button size="xs" variant="outline" onClick={() => convertIdea.mutate(i.id)}>
                    Adıma dönüştür <ArrowRight className="h-3 w-3" />
                  </Button>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </section>

      {header.continueHref ? (
        <div className="pt-4 text-center">
          <Link href={header.continueHref} className="text-sm text-primary hover:underline">
            {header.continueLabel ?? "Bu projeye devam et"} →
          </Link>
        </div>
      ) : null}
    </article>
  );
}
