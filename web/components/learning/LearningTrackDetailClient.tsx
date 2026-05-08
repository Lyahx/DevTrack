"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowLeft, MoreVertical, Pencil, Plus, Trash2 } from "lucide-react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { toast } from "sonner";
import { DecisionTab } from "@/components/decisions/DecisionTab";
import { IdeaTab } from "@/components/ideas/IdeaTab";
import { LearningModuleFormModal } from "@/components/learning/LearningModuleFormModal";
import { LearningTrackFormModal } from "@/components/learning/LearningTrackFormModal";
import { NextStepTab } from "@/components/next-steps/NextStepTab";
import { ResourceTab } from "@/components/resources/ResourceTab";
import { ModuleStatusBadge, ProjectStatusBadge } from "@/components/common/StatusBadge";
import { TagChips } from "@/components/common/TagChips";
import { WorklogTab } from "@/components/worklogs/WorklogTab";
import { Button, buttonVariants } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Skeleton } from "@/components/ui/skeleton";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { learningModulesApi, learningTracksApi } from "@/lib/api/learning";
import { errorMessage } from "@/lib/error";
import { LEARNING_TRACK_STATUSES, PROJECT_STATUS_LABELS } from "@/types/enums";
import type { LearningModuleStatus, LearningTrackStatus } from "@/types/enums";
import { cn } from "@/lib/utils";

export function LearningTrackDetailClient({ trackId }: { trackId: number }) {
  const router = useRouter();
  const qc = useQueryClient();
  const [editTrackOpen, setEditTrackOpen] = useState(false);
  const [newModuleOpen, setNewModuleOpen] = useState(false);

  const track = useQuery({ queryKey: ["learning-track", trackId], queryFn: () => learningTracksApi.get(trackId) });
  const modules = useQuery({ queryKey: ["modules", trackId], queryFn: () => learningModulesApi.listForTrack(trackId) });

  const setStatus = useMutation({
    mutationFn: (status: LearningTrackStatus) => learningTracksApi.setStatus(trackId, { status }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["learning-track", trackId] });
      qc.invalidateQueries({ queryKey: ["learning-tracks"] });
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const remove = useMutation({
    mutationFn: () => learningTracksApi.remove(trackId),
    onSuccess: () => {
      toast.success("Eğitim silindi.");
      qc.invalidateQueries({ queryKey: ["learning-tracks"] });
      router.push("/learning");
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const setModuleStatus = useMutation({
    mutationFn: ({ id, status }: { id: number; status: LearningModuleStatus }) => learningModulesApi.setStatus(id, { status }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["modules", trackId] });
      qc.invalidateQueries({ queryKey: ["resume"] });
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  if (track.isLoading) return <Skeleton className="h-48 w-full" />;
  if (!track.data) return <p>Eğitim bulunamadı.</p>;

  const t = track.data;
  const totalModules = modules.data?.length ?? 0;
  const completed = modules.data?.filter((m) => m.status === "Completed").length ?? 0;
  const progress = totalModules === 0 ? 0 : Math.round((completed / totalModules) * 100);

  return (
    <div className="space-y-6">
      <Link href="/learning" className="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground">
        <ArrowLeft className="h-3 w-3" /> Eğitimler
      </Link>
      <div className="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
        <div>
          <div className="flex items-center gap-3">
            <h1 className="text-2xl font-semibold tracking-tight">{t.name}</h1>
            <ProjectStatusBadge status={t.status} />
          </div>
          {t.description ? <p className="mt-1 text-sm text-muted-foreground">{t.description}</p> : null}
          <div className="mt-2 flex flex-wrap items-center gap-2">
            {t.source ? <span className="text-xs text-muted-foreground">{t.source}</span> : null}
            <TagChips tags={t.tags} />
          </div>
          {totalModules > 0 ? (
            <div className="mt-3 max-w-xs">
              <div className="mb-1 flex justify-between text-xs text-muted-foreground">
                <span>İlerleme</span><span>{progress}%</span>
              </div>
              <div className="h-1.5 overflow-hidden rounded-full bg-muted">
                <div className="h-full rounded-full bg-primary transition-all" style={{ width: `${progress}%` }} />
              </div>
            </div>
          ) : null}
        </div>
        <div className="flex items-center gap-2">
          <Link href={`/learning/${trackId}/resume`} className={cn(buttonVariants({ variant: "default" }))}>Resume Mode</Link>
          <Select value={t.status} onValueChange={(v) => setStatus.mutate(v as LearningTrackStatus)}>
            <SelectTrigger className="w-44"><SelectValue /></SelectTrigger>
            <SelectContent>
              {LEARNING_TRACK_STATUSES.map((s) => (
                <SelectItem key={s} value={s}>{PROJECT_STATUS_LABELS[s]}</SelectItem>
              ))}
            </SelectContent>
          </Select>
          <DropdownMenu>
            <DropdownMenuTrigger
              type="button"
              aria-label="Daha fazla"
              className={cn(buttonVariants({ variant: "ghost", size: "icon" }))}
            >
              <MoreVertical className="h-4 w-4" />
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuItem onClick={() => setEditTrackOpen(true)}>
                <Pencil className="mr-2 h-4 w-4" /> Düzenle
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                variant="destructive"
                onClick={() => {
                  if (confirm("Eğitimi sil? Bağlı modül ve kayıtlar da soft-delete olur.")) remove.mutate();
                }}
              >
                <Trash2 className="mr-2 h-4 w-4" /> Sil
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>

      <Tabs defaultValue="overview">
        <TabsList>
          <TabsTrigger value="overview">Genel</TabsTrigger>
          <TabsTrigger value="worklogs">Worklog</TabsTrigger>
          <TabsTrigger value="decisions">Kararlar</TabsTrigger>
          <TabsTrigger value="next-steps">Adımlar</TabsTrigger>
          <TabsTrigger value="ideas">Fikirler</TabsTrigger>
          <TabsTrigger value="resources">Kaynaklar</TabsTrigger>
        </TabsList>
        <TabsContent value="overview" className="mt-6">
          <div className="space-y-3">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-medium">Modüller</h2>
              <Button size="sm" onClick={() => setNewModuleOpen(true)}>
                <Plus className="h-4 w-4" /> Yeni modül
              </Button>
            </div>
            {modules.isLoading ? (
              <Skeleton className="h-32 w-full" />
            ) : modules.data?.length ? (
              <div className="space-y-2">
                {modules.data.map((m) => (
                  <Card key={m.id}>
                    <CardHeader className="flex flex-row items-center gap-3 py-3">
                      <span className="w-6 text-xs text-muted-foreground">#{m.order}</span>
                      <Link href={`/learning/${trackId}/modules/${m.id}`} className="flex-1 text-base font-medium hover:underline">
                        {m.name}
                      </Link>
                      <Select value={m.status} onValueChange={(v) => setModuleStatus.mutate({ id: m.id, status: v as LearningModuleStatus })}>
                        <SelectTrigger className="w-36"><SelectValue /></SelectTrigger>
                        <SelectContent>
                          <SelectItem value="NotStarted">Başlamadı</SelectItem>
                          <SelectItem value="InProgress">Devam ediyor</SelectItem>
                          <SelectItem value="Completed">Tamamlandı</SelectItem>
                        </SelectContent>
                      </Select>
                    </CardHeader>
                  </Card>
                ))}
              </div>
            ) : (
              <Card><CardContent className="py-6 text-center text-sm text-muted-foreground">Henüz modül yok.</CardContent></Card>
            )}
          </div>
        </TabsContent>
        <TabsContent value="worklogs" className="mt-6"><WorklogTab scope={{ kind: "track", id: trackId }} /></TabsContent>
        <TabsContent value="decisions" className="mt-6"><DecisionTab scope={{ kind: "track", id: trackId }} /></TabsContent>
        <TabsContent value="next-steps" className="mt-6"><NextStepTab scope={{ kind: "track", id: trackId }} /></TabsContent>
        <TabsContent value="ideas" className="mt-6"><IdeaTab scope={{ kind: "track", id: trackId }} /></TabsContent>
        <TabsContent value="resources" className="mt-6"><ResourceTab scope={{ kind: "track", id: trackId }} /></TabsContent>
      </Tabs>

      <LearningTrackFormModal open={editTrackOpen} onOpenChange={setEditTrackOpen} initial={t} />
      <LearningModuleFormModal open={newModuleOpen} onOpenChange={setNewModuleOpen} trackId={trackId} defaultOrder={(modules.data?.length ?? 0)} />
    </div>
  );
}
