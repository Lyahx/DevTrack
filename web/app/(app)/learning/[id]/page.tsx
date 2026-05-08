import { LearningTrackDetailClient } from "@/components/learning/LearningTrackDetailClient";

export default async function LearningTrackPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <LearningTrackDetailClient trackId={Number(id)} />;
}
