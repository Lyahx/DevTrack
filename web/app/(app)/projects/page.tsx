import type { Metadata } from "next";
import { ProjectsList } from "@/components/projects/ProjectsList";

export const metadata: Metadata = { title: "DevTrack — Projeler" };

export default function ProjectsPage() {
  return <ProjectsList />;
}
