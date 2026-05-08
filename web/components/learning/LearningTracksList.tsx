"use client";

import { useQuery } from "@tanstack/react-query";
import { BookOpen, Plus } from "lucide-react";
import Link from "next/link";
import { useMemo, useState } from "react";
import { EmptyState } from "@/components/common/EmptyState";
import { PageHeader } from "@/components/common/PageHeader";
import { ProjectStatusBadge } from "@/components/common/StatusBadge";
import { TagChips } from "@/components/common/TagChips";
import { LearningTrackFormModal } from "@/components/learning/LearningTrackFormModal";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { learningTracksApi } from "@/lib/api/learning";
import { formatRelative } from "@/lib/date";
import { LEARNING_TRACK_STATUSES, PROJECT_STATUS_LABELS } from "@/types/enums";
import type { LearningTrackStatus } from "@/types/enums";

export function LearningTracksList() {
  const [statusFilter, setStatusFilter] = useState<LearningTrackStatus | "All">("All");
  const [search, setSearch] = useState("");
  const [includeDeleted, setIncludeDeleted] = useState(false);
  const [createOpen, setCreateOpen] = useState(false);

  const list = useQuery({
    queryKey: ["learning-tracks", "list", statusFilter, includeDeleted],
    queryFn: () =>
      learningTracksApi.list({
        status: statusFilter === "All" ? undefined : statusFilter,
        pageSize: 100,
        includeDeleted,
      }),
  });

  const filtered = useMemo(() => {
    if (!list.data) return [];
    if (!search) return list.data.items;
    const q = search.toLowerCase();
    return list.data.items.filter((t) => t.name.toLowerCase().includes(q));
  }, [list.data, search]);

  return (
    <div className="space-y-6">
      <PageHeader
        title="Eğitimler"
        description="Claude, Coursera, kitaplar — takip ettiğin her şey."
        actions={<Button onClick={() => setCreateOpen(true)}><Plus className="h-4 w-4" /> Yeni eğitim</Button>}
      />

      <div className="flex flex-wrap items-center gap-3">
        <Input placeholder="Ara…" value={search} onChange={(e) => setSearch(e.target.value)} className="max-w-xs" />
        <Select value={statusFilter} onValueChange={(v) => setStatusFilter(v as LearningTrackStatus | "All")}>
          <SelectTrigger className="w-44"><SelectValue /></SelectTrigger>
          <SelectContent>
            <SelectItem value="All">Tüm durumlar</SelectItem>
            {LEARNING_TRACK_STATUSES.map((s) => (
              <SelectItem key={s} value={s}>{PROJECT_STATUS_LABELS[s]}</SelectItem>
            ))}
          </SelectContent>
        </Select>
        <div className="flex items-center gap-2">
          <Switch id="incl-del-tr" checked={includeDeleted} onCheckedChange={setIncludeDeleted} />
          <Label htmlFor="incl-del-tr" className="text-sm text-muted-foreground">Silinenleri göster</Label>
        </div>
      </div>

      {list.isLoading ? (
        <div className="grid grid-cols-1 gap-3 md:grid-cols-2 lg:grid-cols-3">
          {[0, 1, 2].map((i) => <Skeleton key={i} className="h-32" />)}
        </div>
      ) : filtered.length === 0 ? (
        <EmptyState
          icon={<BookOpen className="h-6 w-6" />}
          title="Eğitim yok."
          description={search ? "Sonuç yok." : "İlk eğitimi başlat."}
          action={!search ? <Button onClick={() => setCreateOpen(true)}><Plus className="h-4 w-4" /> Yeni eğitim</Button> : undefined}
        />
      ) : (
        <div className="grid grid-cols-1 gap-3 md:grid-cols-2 lg:grid-cols-3">
          {filtered.map((t) => (
            <Link key={t.id} href={`/learning/${t.id}`} className="group">
              <Card className={`h-full transition-all hover:border-border hover:shadow-card hover:-translate-y-px ${t.isDeleted ? "opacity-60" : ""}`}>
                <CardHeader className="pb-2">
                  <div className="flex items-start justify-between gap-2">
                    <CardTitle className="text-base font-semibold">{t.name}</CardTitle>
                    <ProjectStatusBadge status={t.status} />
                  </div>
                  {t.source ? <p className="text-xs text-muted-foreground">{t.source}</p> : null}
                </CardHeader>
                <CardContent className="flex items-center justify-between pt-0">
                  <span className="text-xs text-muted-foreground">{formatRelative(t.lastActivityAt ?? t.createdAt)}</span>
                  <TagChips tags={t.tags} />
                </CardContent>
              </Card>
            </Link>
          ))}
        </div>
      )}

      <LearningTrackFormModal open={createOpen} onOpenChange={setCreateOpen} />
    </div>
  );
}
