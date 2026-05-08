"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowLeft, ExternalLink, MoreVertical, Pencil, Plus, Trash2 } from "lucide-react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { toast } from "sonner";
import { ComponentFormModal } from "@/components/components/ComponentFormModal";
import { StatusNoteEditor } from "@/components/components/StatusNoteEditor";
import { CommitList } from "@/components/projects/CommitList";
import { IdeaTab } from "@/components/ideas/IdeaTab";
import { NextStepTab } from "@/components/next-steps/NextStepTab";
import { ProjectFormModal } from "@/components/projects/ProjectFormModal";
import { ResourceTab } from "@/components/resources/ResourceTab";
import { ProjectStatusBadge } from "@/components/common/StatusBadge";
import { TagChips } from "@/components/common/TagChips";
import { WorklogTab } from "@/components/worklogs/WorklogTab";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Skeleton } from "@/components/ui/skeleton";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { buttonVariants } from "@/components/ui/button";
import { componentsApi } from "@/lib/api/components";
import { projectsApi } from "@/lib/api/projects";
import { errorMessage } from "@/lib/error";
import { PROJECT_STATUSES, PROJECT_STATUS_LABELS } from "@/types/enums";
import type { ProjectStatus } from "@/types/enums";
import { cn } from "@/lib/utils";

export function ProjectDetailClient({ projectId }: { projectId: number }) {
  const router = useRouter();
  const qc = useQueryClient();
  const [editProjectOpen, setEditProjectOpen] = useState(false);
  const [newComponentOpen, setNewComponentOpen] = useState(false);

  const project = useQuery({
    queryKey: ["project", projectId],
    queryFn: () => projectsApi.get(projectId),
  });
  const components = useQuery({
    queryKey: ["components", projectId],
    queryFn: () => componentsApi.listForProject(projectId),
  });

  const setStatus = useMutation({
    mutationFn: (status: ProjectStatus) => projectsApi.setStatus(projectId, { status }),
    onSuccess: () => {
      toast.success("Durum güncellendi.");
      qc.invalidateQueries({ queryKey: ["project", projectId] });
      qc.invalidateQueries({ queryKey: ["projects"] });
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  const remove = useMutation({
    mutationFn: () => projectsApi.remove(projectId),
    onSuccess: () => {
      toast.success("Proje silindi.");
      qc.invalidateQueries({ queryKey: ["projects"] });
      router.push("/projects");
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  if (project.isLoading) {
    return <Skeleton className="h-48 w-full" />;
  }
  if (!project.data) return <p>Proje bulunamadı.</p>;

  const p = project.data;

  return (
    <div className="space-y-6">
      <Link href="/projects" className="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground">
        <ArrowLeft className="h-3 w-3" /> Projeler
      </Link>

      <div className="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
        <div>
          <div className="flex items-center gap-3">
            <h1 className="text-2xl font-semibold tracking-tight">{p.name}</h1>
            <ProjectStatusBadge status={p.status} />
          </div>
          {p.goal ? <p className="mt-1 text-sm text-muted-foreground">{p.goal}</p> : null}
          <div className="mt-2 flex flex-wrap items-center gap-2">
            <TagChips tags={p.tags} />
            {p.repoUrl ? (
              <a href={p.repoUrl} target="_blank" rel="noopener noreferrer" className="inline-flex items-center gap-1 text-xs text-muted-foreground hover:text-foreground">
                <ExternalLink className="h-3 w-3" /> Repo
              </a>
            ) : null}
          </div>
        </div>
        <div className="flex items-center gap-2">
          <Link href={`/projects/${projectId}/resume`} className={cn(buttonVariants({ variant: "default" }))}>
            Resume Mode
          </Link>
          <Select value={p.status} onValueChange={(v) => setStatus.mutate(v as ProjectStatus)}>
            <SelectTrigger className="w-44"><SelectValue /></SelectTrigger>
            <SelectContent>
              {PROJECT_STATUSES.map((s) => (
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
              <DropdownMenuItem onClick={() => setEditProjectOpen(true)}>
                <Pencil className="mr-2 h-4 w-4" /> Düzenle
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                variant="destructive"
                onClick={() => {
                  if (confirm("Projeyi sil? Bağlı tüm worklog/karar/adım/fikir/kaynak da soft-delete olur.")) remove.mutate();
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
          <TabsTrigger value="commits">Commit&apos;ler</TabsTrigger>
          <TabsTrigger value="next-steps">Adımlar</TabsTrigger>
          <TabsTrigger value="ideas">Fikirler</TabsTrigger>
          <TabsTrigger value="resources">Kaynaklar</TabsTrigger>
        </TabsList>
        <TabsContent value="overview" className="mt-6">
          <div className="space-y-6">
            {p.description ? (
              <Card>
                <CardHeader className="pb-2"><CardTitle className="text-base">Açıklama</CardTitle></CardHeader>
                <CardContent className="whitespace-pre-wrap text-sm text-muted-foreground">{p.description}</CardContent>
              </Card>
            ) : null}
            <section>
              <div className="mb-3 flex items-center justify-between">
                <h2 className="text-lg font-medium">Bileşenler</h2>
                <Button size="sm" onClick={() => setNewComponentOpen(true)}>
                  <Plus className="h-4 w-4" /> Yeni bileşen
                </Button>
              </div>
              {components.isLoading ? (
                <Skeleton className="h-32 w-full" />
              ) : components.data?.length ? (
                <div className="space-y-3">
                  {components.data.map((c) => (
                    <Card key={c.id}>
                      <CardHeader className="pb-2">
                        <div className="flex flex-wrap items-center justify-between gap-2">
                          <Link href={`/components/${c.id}`} className="text-base font-semibold hover:underline">
                            {c.name}
                          </Link>
                          <div className="flex items-center gap-2 text-xs text-muted-foreground">
                            <span>{c.type}</span>
                            {c.techStack ? <span>· {c.techStack}</span> : null}
                            {c.localUrl ? (
                              <a href={c.localUrl} target="_blank" rel="noopener noreferrer" className="hover:text-foreground">
                                <ExternalLink className="h-3 w-3" />
                              </a>
                            ) : null}
                          </div>
                        </div>
                      </CardHeader>
                      <CardContent>
                        <StatusNoteEditor componentId={c.id} value={c.currentStatusNote} />
                      </CardContent>
                    </Card>
                  ))}
                </div>
              ) : (
                <Card>
                  <CardContent className="py-6 text-center text-sm text-muted-foreground">
                    Henüz bileşen yok. Bir API, bir web app, vs. ekle.
                  </CardContent>
                </Card>
              )}
            </section>
          </div>
        </TabsContent>
        <TabsContent value="worklogs" className="mt-6">
          <WorklogTab scope={{ kind: "project", id: projectId }} />
        </TabsContent>
        <TabsContent value="commits" className="mt-6">
          <CommitList projectId={projectId} hasRepo={!!p.repoUrl} />
        </TabsContent>
        <TabsContent value="next-steps" className="mt-6">
          <NextStepTab scope={{ kind: "project", id: projectId }} />
        </TabsContent>
        <TabsContent value="ideas" className="mt-6">
          <IdeaTab scope={{ kind: "project", id: projectId }} />
        </TabsContent>
        <TabsContent value="resources" className="mt-6">
          <ResourceTab scope={{ kind: "project", id: projectId }} />
        </TabsContent>
      </Tabs>

      <ProjectFormModal open={editProjectOpen} onOpenChange={setEditProjectOpen} initial={p} />
      <ComponentFormModal open={newComponentOpen} onOpenChange={setNewComponentOpen} projectId={projectId} />
    </div>
  );
}
