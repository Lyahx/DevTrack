"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  ArrowRight,
  BookOpen,
  CheckSquare,
  ExternalLink,
  FileText,
  Link2,
  MapPin,
  Sparkles,
} from "lucide-react";
import Link from "next/link";
import { toast } from "sonner";
import { ResourceTypeBadge, ModuleStatusBadge, PriorityBadge } from "@/components/common/StatusBadge";
import { TagChips } from "@/components/common/TagChips";
import { Button, buttonVariants } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { ideasApi } from "@/lib/api/ideas";
import { nextStepsApi } from "@/lib/api/nextSteps";
import { daysSinceLabel, formatDateTime, formatRelative } from "@/lib/date";
import { errorMessage } from "@/lib/error";
import type {
  IdeaResponse,
  NextStepResponse,
  ResourceResponse,
  WorklogResponse,
} from "@/types/activity";
import type { ComponentResponse } from "@/types/component";
import type { LearningModuleResponse } from "@/types/learning";
import { cn } from "@/lib/utils";

type Header = {
  title: string;
  subtitle?: string | null;
  badge?: React.ReactNode;
  daysSince: number | null;
  tags?: { id: number; name: string; color?: string | null }[];
  continueHref?: string;
  continueLabel?: string;
};

function SectionHeading({ icon, children }: { icon: React.ReactNode; children: React.ReactNode }) {
  return (
    <h2 className="flex items-center gap-2 text-[10px] font-semibold uppercase tracking-wider text-text-faint">
      <span className="[&_svg]:size-3.5 text-text-faint">{icon}</span>
      {children}
    </h2>
  );
}

function EmptyHint({ children }: { children: React.ReactNode }) {
  return <p className="rounded-md border border-border-subtle bg-surface-1 px-3 py-2.5 text-[12px] text-text-faint">{children}</p>;
}

export function ResumeView({
  header,
  components,
  modules,
  recentWorklogs,
  openNextSteps,
  resources,
  recentIdeas,
  progressPercent,
}: {
  header: Header;
  components?: ComponentResponse[];
  modules?: LearningModuleResponse[];
  recentWorklogs: WorklogResponse[];
  openNextSteps: NextStepResponse[];
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
    <article className="mx-auto max-w-[720px] space-y-8 py-2">
      <header className="space-y-3">
        <div className="flex flex-wrap items-center gap-3">
          <h1 className="text-[24px] font-medium tracking-tight text-text">{header.title}</h1>
          {header.badge}
        </div>
        {header.subtitle ? <p className="text-[13px] text-text-secondary">{header.subtitle}</p> : null}
        <div className="flex flex-wrap items-center gap-3 text-[12px] text-text-muted">
          <span>{daysSinceLabel(header.daysSince)}</span>
          {progressPercent !== undefined ? (
            <>
              <span className="text-text-faint">·</span>
              <span>İlerleme: <span className="font-mono text-text-secondary">{progressPercent}%</span></span>
            </>
          ) : null}
          {header.tags && header.tags.length > 0 ? (
            <>
              <span className="text-text-faint">·</span>
              <TagChips tags={header.tags as never[]} />
            </>
          ) : null}
        </div>
      </header>

      {components && components.length > 0 ? (
        <section className="space-y-3">
          <SectionHeading icon={<MapPin />}>Şu an nerede kaldım</SectionHeading>
          <div className="space-y-3">
            {components.map((c) => (
              <div key={c.id} className="relative overflow-hidden rounded-md border border-border bg-surface-2 p-5 pl-6">
                <span className="absolute left-0 top-0 bottom-0 w-1 bg-primary" aria-hidden />
                <div className="mb-2 flex flex-wrap items-baseline gap-2">
                  <Link href={`/components/${c.id}`} className="text-[14px] font-medium text-text hover:underline">
                    {c.name}
                  </Link>
                  {c.techStack ? <span className="font-mono text-[11px] text-text-faint">{c.techStack}</span> : null}
                </div>
                {c.currentStatusNote ? (
                  <p className="whitespace-pre-wrap text-[13px] leading-relaxed text-text-secondary">{c.currentStatusNote}</p>
                ) : (
                  <p className="text-[12px] italic text-text-faint">Bu bileşene henüz not eklenmemiş.</p>
                )}
              </div>
            ))}
          </div>
        </section>
      ) : null}

      {modules && modules.length > 0 ? (
        <section className="space-y-3">
          <SectionHeading icon={<BookOpen />}>Modüller</SectionHeading>
          <div className="space-y-1.5">
            {modules.map((m) => (
              <Link
                key={m.id}
                href={`/learning/${m.learningTrackId}/modules/${m.id}`}
                className="group flex items-center gap-3 rounded-md border border-border-subtle bg-surface-1 p-3 transition-colors hover:bg-surface-2"
              >
                <span className="font-mono w-7 text-[11px] text-text-faint">#{m.order}</span>
                <span className="flex-1 text-[13px] text-text-secondary group-hover:text-text">{m.name}</span>
                <ModuleStatusBadge status={m.status} />
              </Link>
            ))}
          </div>
        </section>
      ) : null}

      <section className="space-y-3">
        <SectionHeading icon={<FileText />}>Son worklog&apos;lar</SectionHeading>
        {recentWorklogs.length === 0 ? (
          <EmptyHint>Henüz worklog yok.</EmptyHint>
        ) : (
          <ol className="space-y-3 border-l border-border pl-5">
            {recentWorklogs.map((w) => (
              <li key={w.id} className="relative">
                <span className="absolute -left-[1.65rem] top-3 inline-block h-2 w-2 rounded-full bg-border-strong" />
                <div className="rounded-md border border-border-subtle bg-surface-1 p-3">
                  <p className="font-mono text-[10px] text-text-faint" title={formatDateTime(w.loggedAt)}>{formatRelative(w.loggedAt)}</p>
                  <p className="mt-1 whitespace-pre-wrap text-[13px] leading-relaxed text-text-secondary">{w.whatIDid}</p>
                  {w.whatsLeft ? (
                    <p className="mt-1.5 text-[12px] italic text-text-muted">Geriye kalan: {w.whatsLeft}</p>
                  ) : null}
                  {w.reasoning ? (
                    <p className="mt-2 border-t border-border-subtle pt-2 text-[12px] text-text-muted">
                      <span className="font-medium text-text-secondary">Neden:</span> {w.reasoning}
                    </p>
                  ) : null}
                  {w.alternatives ? (
                    <p className="mt-1 text-[11px] text-text-faint">
                      <span className="font-medium text-text-muted">Alternatifler:</span> {w.alternatives}
                    </p>
                  ) : null}
                </div>
              </li>
            ))}
          </ol>
        )}
      </section>

      <section className="space-y-3">
        <SectionHeading icon={<CheckSquare />}>Açık adımlar ({openNextSteps.length})</SectionHeading>
        {openNextSteps.length === 0 ? (
          <EmptyHint>Açık adım yok.</EmptyHint>
        ) : (
          <div className="space-y-1.5">
            {openNextSteps.map((s) => (
              <div key={s.id} className="flex items-start gap-3 rounded-md border border-border-subtle bg-surface-1 p-3">
                <Checkbox className="mt-0.5" onCheckedChange={() => completeStep.mutate(s.id)} />
                <div className="flex-1">
                  <p className="text-[13px] text-text-secondary">{s.description}</p>
                  <div className="mt-1.5"><PriorityBadge priority={s.priority} /></div>
                </div>
              </div>
            ))}
          </div>
        )}
      </section>

      <section className="space-y-3">
        <SectionHeading icon={<Link2 />}>Kaynaklar ({resources.length})</SectionHeading>
        {resources.length === 0 ? (
          <EmptyHint>Henüz kaynak yok.</EmptyHint>
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
                      className="flex items-start gap-2 rounded-md border border-border-subtle bg-surface-1 p-3 transition-colors hover:bg-surface-2"
                    >
                      <ExternalLink className="mt-0.5 h-3.5 w-3.5 shrink-0 text-text-muted" />
                      <div className="min-w-0">
                        <p className="truncate text-[12px] font-medium text-text">{r.title}</p>
                        {r.notes ? <p className="line-clamp-2 text-[11px] text-text-faint">{r.notes}</p> : null}
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
        <SectionHeading icon={<Sparkles />}>Yakalanmış fikirler</SectionHeading>
        {recentIdeas.length === 0 ? (
          <EmptyHint>Açık fikir yok.</EmptyHint>
        ) : (
          <div className="space-y-1.5">
            {recentIdeas.map((i) => (
              <div key={i.id} className="flex items-start justify-between gap-3 rounded-md border border-border-subtle bg-surface-1 p-3">
                <p className="flex-1 text-[13px] text-text-secondary">{i.content}</p>
                <Button size="xs" variant="secondary" onClick={() => convertIdea.mutate(i.id)}>
                  Adıma dönüştür <ArrowRight className="h-3 w-3" />
                </Button>
              </div>
            ))}
          </div>
        )}
      </section>

      {header.continueHref ? (
        <div className="pt-2">
          <Link href={header.continueHref} className={cn(buttonVariants({ variant: "default", size: "lg" }), "w-full justify-center")}>
            {header.continueLabel ?? "Bu projeye devam et"} <ArrowRight className="h-4 w-4" />
          </Link>
        </div>
      ) : null}
    </article>
  );
}
