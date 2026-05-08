"use client";

import { useQuery } from "@tanstack/react-query";
import { Skeleton } from "@/components/ui/skeleton";
import { ProjectStatusBadge } from "@/components/common/StatusBadge";
import { ResumeView } from "@/components/resume/ResumeView";
import { resumeApi } from "@/lib/api/resume";

export function ProjectResumeClient({ projectId }: { projectId: number }) {
  const data = useQuery({ queryKey: ["resume", "project", projectId], queryFn: () => resumeApi.forProject(projectId) });

  if (data.isLoading) return <Skeleton className="h-96 w-full" />;
  if (!data.data) return <p>Proje bulunamadı.</p>;

  const r = data.data;

  return (
    <ResumeView
      header={{
        title: r.project.name,
        subtitle: r.project.goal,
        badge: <ProjectStatusBadge status={r.project.status} />,
        daysSince: r.daysSinceLastActivity,
        tags: r.project.tags,
        continueHref: `/projects/${r.project.id}`,
        continueLabel: "Bu projeye devam et",
      }}
      components={r.components}
      recentWorklogs={r.recentWorklogs}
      openNextSteps={r.openNextSteps}
      resources={r.resources}
      recentIdeas={r.recentIdeas}
    />
  );
}
