"use client";

import { useQuery } from "@tanstack/react-query";
import { Trash2 } from "lucide-react";
import { EmptyState } from "@/components/common/EmptyState";
import { PageHeader } from "@/components/common/PageHeader";
import { ProjectStatusBadge } from "@/components/common/StatusBadge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { learningTracksApi } from "@/lib/api/learning";
import { projectsApi } from "@/lib/api/projects";
import { formatRelative } from "@/lib/date";

export function TrashClient() {
  const projects = useQuery({
    queryKey: ["projects", "trash"],
    queryFn: () => projectsApi.list({ pageSize: 200, includeDeleted: true }),
    select: (d) => d.items.filter((p) => p.isDeleted),
  });
  const tracks = useQuery({
    queryKey: ["learning-tracks", "trash"],
    queryFn: () => learningTracksApi.list({ pageSize: 200, includeDeleted: true }),
    select: (d) => d.items.filter((t) => t.isDeleted),
  });

  return (
    <div className="space-y-6">
      <PageHeader title="Çöp" description="Silinmiş projeler ve eğitimler." />
      <Card className="border-warning/40 bg-warning-soft">
        <CardContent className="py-3 text-sm text-warning">
          ℹ️ Restore (geri yükleme) henüz yok. Geri almak istersen — yani kendine — destek bileti açabilirsin. Şaka.
          Şu an için DB&apos;de elle <code>IsDeleted=false</code> yapman gerekir.
        </CardContent>
      </Card>

      <section>
        <h2 className="mb-3 text-lg font-medium">Silinmiş projeler</h2>
        {projects.isLoading ? (
          <Skeleton className="h-32 w-full" />
        ) : !projects.data?.length ? (
          <EmptyState icon={<Trash2 className="h-6 w-6" />} title="Silinmiş proje yok." />
        ) : (
          <div className="space-y-2">
            {projects.data.map((p) => (
              <Card key={p.id} className="opacity-70">
                <CardHeader className="flex flex-row items-center justify-between gap-2 py-3">
                  <div className="flex items-center gap-2">
                    <CardTitle className="text-base">{p.name}</CardTitle>
                    <ProjectStatusBadge status={p.status} />
                  </div>
                  <span className="text-xs text-muted-foreground">{formatRelative(p.deletedAt)}&apos;de silindi</span>
                </CardHeader>
              </Card>
            ))}
          </div>
        )}
      </section>

      <section>
        <h2 className="mb-3 text-lg font-medium">Silinmiş eğitimler</h2>
        {tracks.isLoading ? (
          <Skeleton className="h-32 w-full" />
        ) : !tracks.data?.length ? (
          <EmptyState icon={<Trash2 className="h-6 w-6" />} title="Silinmiş eğitim yok." />
        ) : (
          <div className="space-y-2">
            {tracks.data.map((t) => (
              <Card key={t.id} className="opacity-70">
                <CardHeader className="flex flex-row items-center justify-between gap-2 py-3">
                  <div className="flex items-center gap-2">
                    <CardTitle className="text-base">{t.name}</CardTitle>
                    <ProjectStatusBadge status={t.status} />
                  </div>
                  <span className="text-xs text-muted-foreground">{formatRelative(t.deletedAt)}&apos;de silindi</span>
                </CardHeader>
              </Card>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}
