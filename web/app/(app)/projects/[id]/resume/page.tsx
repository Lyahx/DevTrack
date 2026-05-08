import { ProjectResumeClient } from "@/components/resume/ProjectResumeClient";

export default async function ProjectResumePage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <ProjectResumeClient projectId={Number(id)} />;
}
