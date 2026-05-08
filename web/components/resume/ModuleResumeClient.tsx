"use client";

import { useQuery } from "@tanstack/react-query";
import { ModuleStatusBadge } from "@/components/common/StatusBadge";
import { Skeleton } from "@/components/ui/skeleton";
import { ResumeView } from "@/components/resume/ResumeView";
import { resumeApi } from "@/lib/api/resume";

export function ModuleResumeClient({ moduleId }: { moduleId: number }) {
  const data = useQuery({ queryKey: ["resume", "module", moduleId], queryFn: () => resumeApi.forModule(moduleId) });

  if (data.isLoading) return <Skeleton className="h-96 w-full" />;
  if (!data.data) return <p>Modül bulunamadı.</p>;

  const r = data.data;
  const m = r.module;

  return (
    <ResumeView
      header={{
        title: m.name,
        badge: <ModuleStatusBadge status={m.status} />,
        daysSince: r.daysSinceLastActivity,
        continueHref: `/learning/${m.learningTrackId}/modules/${m.id}`,
        continueLabel: "Modüle devam et",
      }}
      recentWorklogs={r.recentWorklogs}
      openNextSteps={r.openNextSteps}
      resources={r.resources}
      recentIdeas={r.recentIdeas}
    />
  );
}
