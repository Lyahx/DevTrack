import type { Metadata } from "next";
import { LearningTracksList } from "@/components/learning/LearningTracksList";

export const metadata: Metadata = { title: "DevTrack — Eğitimler" };

export default function LearningPage() {
  return <LearningTracksList />;
}
