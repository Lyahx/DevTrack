"use client";

import { useQuery } from "@tanstack/react-query";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
import { ResumeView } from "@/components/resume/ResumeView";
import { resumeApi } from "@/lib/api/resume";
import { COMPONENT_TYPE_LABELS } from "@/types/enums";

export function ComponentResumeClient({ componentId }: { componentId: number }) {
  const data = useQuery({ queryKey: ["resume", "component", componentId], queryFn: () => resumeApi.forComponent(componentId) });

  if (data.isLoading) return <Skeleton className="h-96 w-full" />;
  if (!data.data) return <p>Bileşen bulunamadı.</p>;

  const r = data.data;
  const c = r.component;

  return (
    <ResumeView
      header={{
        title: c.name,
        subtitle: c.currentStatusNote,
        badge: <Badge variant="outline">{COMPONENT_TYPE_LABELS[c.type]}</Badge>,
        daysSince: r.daysSinceLastActivity,
        tags: c.tags,
        continueHref: `/components/${c.id}`,
        continueLabel: "Bu bileşene devam et",
      }}
      recentWorklogs={r.recentWorklogs}
      openNextSteps={r.openNextSteps}
      resources={r.resources}
      recentIdeas={r.recentIdeas}
    />
  );
}
