"use client";

import { useQuery } from "@tanstack/react-query";
import { ProjectStatusBadge } from "@/components/common/StatusBadge";
import { Skeleton } from "@/components/ui/skeleton";
import { ResumeView } from "@/components/resume/ResumeView";
import { resumeApi } from "@/lib/api/resume";

export function TrackResumeClient({ trackId }: { trackId: number }) {
  const data = useQuery({ queryKey: ["resume", "track", trackId], queryFn: () => resumeApi.forTrack(trackId) });

  if (data.isLoading) return <Skeleton className="h-96 w-full" />;
  if (!data.data) return <p>Eğitim bulunamadı.</p>;

  const r = data.data;

  return (
    <ResumeView
      header={{
        title: r.track.name,
        subtitle: r.track.description ?? r.track.source,
        badge: <ProjectStatusBadge status={r.track.status} />,
        daysSince: r.daysSinceLastActivity,
        tags: r.track.tags,
        continueHref: `/learning/${r.track.id}`,
        continueLabel: "Bu eğitime devam et",
      }}
      modules={r.modules}
      recentWorklogs={r.recentWorklogs}
      openNextSteps={r.openNextSteps}
      recentDecisions={r.recentDecisions}
      resources={r.resources}
      recentIdeas={r.recentIdeas}
      progressPercent={Number(r.progressPercent)}
    />
  );
}
