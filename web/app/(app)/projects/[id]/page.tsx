import { ProjectDetailClient } from "@/components/projects/ProjectDetailClient";

export default async function ProjectDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  const projectId = Number(id);
  return <ProjectDetailClient projectId={projectId} />;
}
