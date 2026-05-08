import type { Metadata } from "next";
import { TagsClient } from "@/components/tags/TagsClient";

export const metadata: Metadata = { title: "DevTrack — Etiketler" };

export default function TagsPage() {
  return <TagsClient />;
}
