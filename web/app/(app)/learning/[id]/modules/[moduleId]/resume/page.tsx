import { ModuleResumeClient } from "@/components/resume/ModuleResumeClient";

export default async function ModuleResumePage({ params }: { params: Promise<{ moduleId: string }> }) {
  const { moduleId } = await params;
  return <ModuleResumeClient moduleId={Number(moduleId)} />;
}
