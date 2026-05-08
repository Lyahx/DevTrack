"use client";

import { useQuery } from "@tanstack/react-query";
import { BookOpen, FolderKanban, Plus, Tags, Trash2, Bell, LayoutDashboard } from "lucide-react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useState } from "react";
import { ProjectStatusDot } from "@/components/common/StatusBadge";
import { Button } from "@/components/ui/button";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Skeleton } from "@/components/ui/skeleton";
import { learningTracksApi } from "@/lib/api/learning";
import { projectsApi } from "@/lib/api/projects";
import { ProjectFormModal } from "@/components/projects/ProjectFormModal";
import { LearningTrackFormModal } from "@/components/learning/LearningTrackFormModal";
import { useAuthStore } from "@/store/auth";
import { cn } from "@/lib/utils";

function NavLink({
  href,
  icon,
  label,
  active,
}: {
  href: string;
  icon: React.ReactNode;
  label: string;
  active?: boolean;
}) {
  return (
    <Link
      href={href}
      className={cn(
        "flex items-center gap-2 rounded-md px-2 py-1.5 text-sm transition-colors",
        active
          ? "bg-accent text-accent-foreground"
          : "text-muted-foreground hover:bg-accent/50 hover:text-foreground",
      )}
    >
      {icon}
      <span className="truncate">{label}</span>
    </Link>
  );
}

export function Sidebar() {
  const pathname = usePathname();
  const status = useAuthStore((s) => s.status);
  const [projectModalOpen, setProjectModalOpen] = useState(false);
  const [trackModalOpen, setTrackModalOpen] = useState(false);

  const projects = useQuery({
    queryKey: ["projects", "sidebar"],
    queryFn: () => projectsApi.list({ status: "Active", pageSize: 50 }),
    enabled: status === "authenticated",
  });
  const tracks = useQuery({
    queryKey: ["learning-tracks", "sidebar"],
    queryFn: () => learningTracksApi.list({ status: "Active", pageSize: 50 }),
    enabled: status === "authenticated",
  });

  return (
    <aside className="hidden w-56 shrink-0 border-r bg-sidebar text-sidebar-foreground md:flex md:flex-col">
      <ScrollArea className="flex-1">
        <nav className="space-y-6 p-3">
          <div className="space-y-1">
            <NavLink
              href="/"
              icon={<LayoutDashboard className="h-4 w-4" />}
              label="Pano"
              active={pathname === "/"}
            />
          </div>

          <div>
            <div className="mb-1 flex items-center justify-between px-2">
              <span className="flex items-center gap-1.5 text-[11px] font-semibold uppercase tracking-wider text-muted-foreground">
                <FolderKanban className="h-3.5 w-3.5" /> Projeler
              </span>
              <Button
                size="icon"
                variant="ghost"
                className="h-6 w-6"
                onClick={() => setProjectModalOpen(true)}
                aria-label="Yeni proje"
              >
                <Plus className="h-3.5 w-3.5" />
              </Button>
            </div>
            <div className="space-y-0.5">
              {projects.isLoading ? (
                <div className="space-y-1 px-2">
                  <Skeleton className="h-5 w-full" />
                  <Skeleton className="h-5 w-3/4" />
                </div>
              ) : projects.data?.items.length ? (
                projects.data.items.map((p) => (
                  <Link
                    key={p.id}
                    href={`/projects/${p.id}`}
                    className={cn(
                      "flex items-center gap-2 rounded-md px-2 py-1 text-sm",
                      pathname.startsWith(`/projects/${p.id}`)
                        ? "bg-accent text-accent-foreground"
                        : "text-muted-foreground hover:bg-accent/50 hover:text-foreground",
                    )}
                  >
                    <ProjectStatusDot status={p.status} />
                    <span className="truncate">{p.name}</span>
                  </Link>
                ))
              ) : (
                <p className="px-2 text-xs text-muted-foreground">Henüz proje yok.</p>
              )}
              <Link
                href="/projects"
                className="block px-2 pt-1 text-xs text-muted-foreground hover:text-foreground"
              >
                Tümünü gör →
              </Link>
            </div>
          </div>

          <div>
            <div className="mb-1 flex items-center justify-between px-2">
              <span className="flex items-center gap-1.5 text-[11px] font-semibold uppercase tracking-wider text-muted-foreground">
                <BookOpen className="h-3.5 w-3.5" /> Eğitim
              </span>
              <Button
                size="icon"
                variant="ghost"
                className="h-6 w-6"
                onClick={() => setTrackModalOpen(true)}
                aria-label="Yeni eğitim"
              >
                <Plus className="h-3.5 w-3.5" />
              </Button>
            </div>
            <div className="space-y-0.5">
              {tracks.isLoading ? (
                <div className="space-y-1 px-2">
                  <Skeleton className="h-5 w-full" />
                </div>
              ) : tracks.data?.items.length ? (
                tracks.data.items.map((t) => (
                  <Link
                    key={t.id}
                    href={`/learning/${t.id}`}
                    className={cn(
                      "flex items-center gap-2 rounded-md px-2 py-1 text-sm",
                      pathname.startsWith(`/learning/${t.id}`)
                        ? "bg-accent text-accent-foreground"
                        : "text-muted-foreground hover:bg-accent/50 hover:text-foreground",
                    )}
                  >
                    <ProjectStatusDot status={t.status} />
                    <span className="truncate">{t.name}</span>
                  </Link>
                ))
              ) : (
                <p className="px-2 text-xs text-muted-foreground">Henüz eğitim yok.</p>
              )}
              <Link
                href="/learning"
                className="block px-2 pt-1 text-xs text-muted-foreground hover:text-foreground"
              >
                Tümünü gör →
              </Link>
            </div>
          </div>

          <div className="space-y-1">
            <p className="px-2 text-[11px] font-semibold uppercase tracking-wider text-muted-foreground">
              Hızlı bağlantılar
            </p>
            <NavLink
              href="/reminders"
              icon={<Bell className="h-4 w-4" />}
              label="Hatırlatmalar"
              active={pathname.startsWith("/reminders")}
            />
            <NavLink
              href="/tags"
              icon={<Tags className="h-4 w-4" />}
              label="Etiketler"
              active={pathname.startsWith("/tags")}
            />
            <NavLink
              href="/trash"
              icon={<Trash2 className="h-4 w-4" />}
              label="Çöp"
              active={pathname.startsWith("/trash")}
            />
          </div>
        </nav>
      </ScrollArea>

      <ProjectFormModal open={projectModalOpen} onOpenChange={setProjectModalOpen} />
      <LearningTrackFormModal open={trackModalOpen} onOpenChange={setTrackModalOpen} />
    </aside>
  );
}
