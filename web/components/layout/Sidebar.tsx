"use client";

import { useQuery } from "@tanstack/react-query";
import { BookOpen, FolderKanban, Plus, Tags, Trash2, Bell, LayoutDashboard, Zap } from "lucide-react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useState } from "react";
import { ProjectStatusDot } from "@/components/common/StatusBadge";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Skeleton } from "@/components/ui/skeleton";
import { learningTracksApi } from "@/lib/api/learning";
import { projectsApi } from "@/lib/api/projects";
import { ProjectFormModal } from "@/components/projects/ProjectFormModal";
import { LearningTrackFormModal } from "@/components/learning/LearningTrackFormModal";
import { useAuthStore } from "@/store/auth";
import { useQuickCaptureStore } from "@/store/quickCapture";
import { formatRelativeShort } from "@/lib/date";
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
        "relative flex items-center gap-2 rounded-md px-2 py-1.5 text-[13px] tracking-tight transition-colors",
        active
          ? "bg-surface-2 text-text"
          : "text-text-muted hover:bg-surface-2 hover:text-text-secondary",
      )}
    >
      {active ? (
        <span className="absolute left-0 top-1/2 h-4 w-0.5 -translate-y-1/2 rounded-r bg-primary" aria-hidden />
      ) : null}
      <span className={cn("[&_svg]:size-3.5", active ? "text-text" : "text-text-muted")}>{icon}</span>
      <span className="truncate">{label}</span>
    </Link>
  );
}

function SectionHeading({ icon, label, action }: { icon: React.ReactNode; label: string; action?: React.ReactNode }) {
  return (
    <div className="mb-1 flex items-center justify-between px-2">
      <span className="flex items-center gap-1.5 text-[10px] font-semibold uppercase tracking-wider text-text-faint">
        <span className="[&_svg]:size-3 text-text-faint">{icon}</span>
        {label}
      </span>
      {action}
    </div>
  );
}

export function Sidebar() {
  const pathname = usePathname();
  const status = useAuthStore((s) => s.status);
  const openCapture = useQuickCaptureStore((s) => s.openCapture);
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
    <aside className="hidden w-60 shrink-0 border-r border-border bg-surface-1 md:flex md:flex-col">
      <ScrollArea className="flex-1">
        <nav className="space-y-5 p-3">
          <div className="space-y-0.5">
            <NavLink
              href="/"
              icon={<LayoutDashboard />}
              label="Pano"
              active={pathname === "/"}
            />
          </div>

          <div>
            <SectionHeading
              icon={<FolderKanban />}
              label="Projeler"
              action={
                <button
                  type="button"
                  onClick={() => setProjectModalOpen(true)}
                  aria-label="Yeni proje"
                  className="flex h-5 w-5 items-center justify-center rounded text-text-muted hover:bg-surface-2 hover:text-text"
                >
                  <Plus className="h-3 w-3" />
                </button>
              }
            />
            <div className="space-y-0.5">
              {projects.isLoading ? (
                <div className="space-y-1 px-2">
                  <Skeleton className="h-5 w-full" />
                  <Skeleton className="h-5 w-3/4" />
                </div>
              ) : projects.data?.items.length ? (
                projects.data.items.map((p) => {
                  const active = pathname.startsWith(`/projects/${p.id}`);
                  return (
                    <Link
                      key={p.id}
                      href={`/projects/${p.id}`}
                      className={cn(
                        "group/nav relative flex items-center gap-2 rounded-md px-2 py-1 text-[13px] tracking-tight transition-colors",
                        active
                          ? "bg-surface-2 text-text"
                          : "text-text-muted hover:bg-surface-2 hover:text-text-secondary",
                      )}
                    >
                      {active ? (
                        <span className="absolute left-0 top-1/2 h-4 w-0.5 -translate-y-1/2 rounded-r bg-primary" aria-hidden />
                      ) : null}
                      <ProjectStatusDot status={p.status} />
                      <span className="flex-1 truncate">{p.name}</span>
                      {p.lastActivityAt ? (
                        <span className="font-mono text-[10px] text-text-faint">{formatRelativeShort(p.lastActivityAt)}</span>
                      ) : null}
                    </Link>
                  );
                })
              ) : (
                <p className="px-2 text-[12px] text-text-faint">Henüz proje yok.</p>
              )}
              <Link
                href="/projects"
                className="block px-2 pt-1 text-[11px] text-text-faint hover:text-text-secondary"
              >
                Tümünü gör →
              </Link>
            </div>
          </div>

          <div>
            <SectionHeading
              icon={<BookOpen />}
              label="Eğitim"
              action={
                <button
                  type="button"
                  onClick={() => setTrackModalOpen(true)}
                  aria-label="Yeni eğitim"
                  className="flex h-5 w-5 items-center justify-center rounded text-text-muted hover:bg-surface-2 hover:text-text"
                >
                  <Plus className="h-3 w-3" />
                </button>
              }
            />
            <div className="space-y-0.5">
              {tracks.isLoading ? (
                <div className="space-y-1 px-2">
                  <Skeleton className="h-5 w-full" />
                </div>
              ) : tracks.data?.items.length ? (
                tracks.data.items.map((t) => {
                  const active = pathname.startsWith(`/learning/${t.id}`);
                  return (
                    <Link
                      key={t.id}
                      href={`/learning/${t.id}`}
                      className={cn(
                        "group/nav relative flex items-center gap-2 rounded-md px-2 py-1 text-[13px] tracking-tight transition-colors",
                        active
                          ? "bg-surface-2 text-text"
                          : "text-text-muted hover:bg-surface-2 hover:text-text-secondary",
                      )}
                    >
                      {active ? (
                        <span className="absolute left-0 top-1/2 h-4 w-0.5 -translate-y-1/2 rounded-r bg-primary" aria-hidden />
                      ) : null}
                      <ProjectStatusDot status={t.status} />
                      <span className="flex-1 truncate">{t.name}</span>
                      {t.lastActivityAt ? (
                        <span className="font-mono text-[10px] text-text-faint">{formatRelativeShort(t.lastActivityAt)}</span>
                      ) : null}
                    </Link>
                  );
                })
              ) : (
                <p className="px-2 text-[12px] text-text-faint">Henüz eğitim yok.</p>
              )}
              <Link
                href="/learning"
                className="block px-2 pt-1 text-[11px] text-text-faint hover:text-text-secondary"
              >
                Tümünü gör →
              </Link>
            </div>
          </div>

          <div className="space-y-0.5">
            <p className="mb-1 px-2 text-[10px] font-semibold uppercase tracking-wider text-text-faint">
              Hızlı bağlantılar
            </p>
            <NavLink
              href="/reminders"
              icon={<Bell />}
              label="Hatırlatmalar"
              active={pathname.startsWith("/reminders")}
            />
            <NavLink
              href="/tags"
              icon={<Tags />}
              label="Etiketler"
              active={pathname.startsWith("/tags")}
            />
            <NavLink
              href="/trash"
              icon={<Trash2 />}
              label="Çöp"
              active={pathname.startsWith("/trash")}
            />
          </div>
        </nav>
      </ScrollArea>

      <div className="border-t border-border p-3">
        <button
          type="button"
          onClick={() => openCapture()}
          className="flex w-full items-center gap-2 rounded-md px-2 py-1.5 text-[12px] text-text-muted transition-colors hover:bg-surface-2 hover:text-text"
        >
          <Zap className="h-3.5 w-3.5 text-primary" />
          <span className="flex-1 text-left">Hızlı yakala</span>
          <kbd className="font-mono text-[10px] text-text-faint">⌘J</kbd>
        </button>
      </div>

      <ProjectFormModal open={projectModalOpen} onOpenChange={setProjectModalOpen} />
      <LearningTrackFormModal open={trackModalOpen} onOpenChange={setTrackModalOpen} />
    </aside>
  );
}
