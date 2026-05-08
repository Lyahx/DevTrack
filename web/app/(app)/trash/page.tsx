import type { Metadata } from "next";
import { TrashClient } from "@/components/trash/TrashClient";

export const metadata: Metadata = { title: "DevTrack — Çöp" };

export default function TrashPage() {
  return <TrashClient />;
}
