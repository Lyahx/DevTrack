import { LearningModuleDetailClient } from "@/components/learning/LearningModuleDetailClient";

export default async function LearningModulePage({ params }: { params: Promise<{ id: string; moduleId: string }> }) {
  const { id, moduleId } = await params;
  return <LearningModuleDetailClient trackId={Number(id)} moduleId={Number(moduleId)} />;
}
