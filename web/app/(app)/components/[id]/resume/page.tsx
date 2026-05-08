import { ComponentResumeClient } from "@/components/resume/ComponentResumeClient";

export default async function ComponentResumePage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <ComponentResumeClient componentId={Number(id)} />;
}
