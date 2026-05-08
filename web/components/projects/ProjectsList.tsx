"use client";

import { useQuery } from "@tanstack/react-query";
import { Plus } from "lucide-react";
import Link from "next/link";
import { useMemo, useState } from "react";
import { EmptyState } from "@/components/common/EmptyState";
import { PageHeader } from "@/components/common/PageHeader";
import { ProjectStatusBadge } from "@/components/common/StatusBadge";
import { TagChips } from "@/components/common/TagChips";
import { ProjectFormModal } from "@/components/projects/ProjectFormModal";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { projectsApi } from "@/lib/api/projects";
import { formatRelative } from "@/lib/date";
import { PROJECT_STATUSES, PROJECT_STATUS_LABELS } from "@/types/enums";
import type { ProjectStatus } from "@/types/enums";

export function ProjectsList() {
  const [statusFilter, setStatusFilter] = useState<ProjectStatus | "All">("All");
  const [search, setSearch] = useState("");
  const [includeDeleted, setIncludeDeleted] = useState(false);
  const [createOpen, setCreateOpen] = useState(false);

  const list = useQuery({
    queryKey: ["projects", "list", statusFilter, includeDeleted],
    queryFn: () =>
      projectsApi.list({
        status: statusFilter === "All" ? undefined : statusFilter,
        pageSize: 100,
        includeDeleted,
      }),
  });

  const filtered = useMemo(() => {
    if (!list.data) return [];
    if (!search) return list.data.items;
    const q = search.toLowerCase();
    return list.data.items.filter((p) => p.name.toLowerCase().includes(q) || p.goal?.toLowerCase().includes(q));
  }, [list.data, search]);

  return (
    <div className="space-y-6">
      <PageHeader
        title="Projeler"
        description="Üzerinde çalıştığın her şey."
        actions={
          <Button onClick={() => setCreateOpen(true)}>
            <Plus className="h-4 w-4" /> Yeni proje
          </Button>
        }
      />

      <div className="flex flex-wrap items-center gap-3">
        <Input
          placeholder="Ara…"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="max-w-xs"
        />
        <Select value={statusFilter} onValueChange={(v) => setStatusFilter(v as ProjectStatus | "All")}>
          <SelectTrigger className="w-44">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="All">Tüm durumlar</SelectItem>
            {PROJECT_STATUSES.map((s) => (
              <SelectItem key={s} value={s}>{PROJECT_STATUS_LABELS[s]}</SelectItem>
            ))}
          </SelectContent>
        </Select>
        <div className="flex items-center gap-2">
          <Switch id="incl-del" checked={includeDeleted} onCheckedChange={setIncludeDeleted} />
          <Label htmlFor="incl-del" className="text-sm text-muted-foreground">Silinenleri göster</Label>
        </div>
      </div>

      {list.isLoading ? (
        <div className="grid grid-cols-1 gap-3 md:grid-cols-2 lg:grid-cols-3">
          {[0, 1, 2, 3].map((i) => <Skeleton key={i} className="h-32" />)}
        </div>
      ) : filtered.length === 0 ? (
        <EmptyState
          title="Proje yok."
          description={search ? "Arama sonucunda eşleşme yok." : "İlkini oluşturalım."}
          action={!search ? <Button onClick={() => setCreateOpen(true)}><Plus className="h-4 w-4" /> Yeni proje</Button> : undefined}
        />
      ) : (
        <div className="grid grid-cols-1 gap-3 md:grid-cols-2 lg:grid-cols-3">
          {filtered.map((p) => (
            <Link key={p.id} href={`/projects/${p.id}`} className="group">
              <Card className={`h-full transition-colors group-hover:border-primary/50 ${p.isDeleted ? "opacity-60" : ""}`}>
                <CardHeader className="pb-2">
                  <div className="flex items-start justify-between gap-2">
                    <CardTitle className="text-base font-semibold group-hover:text-primary">{p.name}</CardTitle>
                    <ProjectStatusBadge status={p.status} />
                  </div>
                  {p.goal ? <p className="line-clamp-2 text-sm text-muted-foreground">{p.goal}</p> : null}
                </CardHeader>
                <CardContent className="flex items-center justify-between pt-0">
                  <span className="text-xs text-muted-foreground">{formatRelative(p.lastActivityAt ?? p.createdAt)}</span>
                  <TagChips tags={p.tags} />
                </CardContent>
              </Card>
            </Link>
          ))}
        </div>
      )}

      <ProjectFormModal open={createOpen} onOpenChange={setCreateOpen} />
    </div>
  );
}
