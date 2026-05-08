import { TrackResumeClient } from "@/components/resume/TrackResumeClient";

export default async function TrackResumePage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <TrackResumeClient trackId={Number(id)} />;
}
