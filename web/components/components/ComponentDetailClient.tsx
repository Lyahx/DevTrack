"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowLeft, ExternalLink, MoreVertical, Pencil, Trash2 } from "lucide-react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { toast } from "sonner";
import { ComponentFormModal } from "@/components/components/ComponentFormModal";
import { StatusNoteEditor } from "@/components/components/StatusNoteEditor";
import { DecisionTab } from "@/components/decisions/DecisionTab";
import { IdeaTab } from "@/components/ideas/IdeaTab";
import { NextStepTab } from "@/components/next-steps/NextStepTab";
import { ResourceTab } from "@/components/resources/ResourceTab";
import { TagChips } from "@/components/common/TagChips";
import { WorklogTab } from "@/components/worklogs/WorklogTab";
import { Badge } from "@/components/ui/badge";
import { Button, buttonVariants } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Skeleton } from "@/components/ui/skeleton";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { componentsApi } from "@/lib/api/components";
import { errorMessage } from "@/lib/error";
import { COMPONENT_TYPE_LABELS } from "@/types/enums";
import { cn } from "@/lib/utils";

export function ComponentDetailClient({ componentId }: { componentId: number }) {
  const router = useRouter();
  const qc = useQueryClient();
  const [editOpen, setEditOpen] = useState(false);

  const data = useQuery({
    queryKey: ["component", componentId],
    queryFn: () => componentsApi.get(componentId),
  });

  const remove = useMutation({
    mutationFn: () => componentsApi.remove(componentId),
    onSuccess: () => {
      toast.success("Bileşen silindi.");
      qc.invalidateQueries({ queryKey: ["projects"] });
      router.back();
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  if (data.isLoading) return <Skeleton className="h-48 w-full" />;
  if (!data.data) return <p>Bileşen bulunamadı.</p>;

  const c = data.data;

  return (
    <div className="space-y-6">
      <Link href={`/projects/${c.projectId}`} className="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground">
        <ArrowLeft className="h-3 w-3" /> Projeye dön
      </Link>
      <div className="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
        <div>
          <div className="flex items-center gap-3">
            <h1 className="text-2xl font-semibold tracking-tight">{c.name}</h1>
            <Badge variant="outline">{COMPONENT_TYPE_LABELS[c.type]}</Badge>
          </div>
          <div className="mt-1 flex flex-wrap items-center gap-2 text-xs text-muted-foreground">
            {c.techStack ? <span>{c.techStack}</span> : null}
            {c.localUrl ? (
              <a href={c.localUrl} target="_blank" rel="noopener noreferrer" className="inline-flex items-center gap-1 hover:text-foreground">
                <ExternalLink className="h-3 w-3" /> {c.localUrl}
              </a>
            ) : null}
            {c.repoPath ? <span>· {c.repoPath}</span> : null}
          </div>
          <div className="mt-2"><TagChips tags={c.tags} /></div>
        </div>
        <div className="flex items-center gap-2">
          <Link href={`/components/${c.id}/resume`} className={cn(buttonVariants({ variant: "default" }))}>
            Resume Mode
          </Link>
          <DropdownMenu>
            <DropdownMenuTrigger
              type="button"
              aria-label="Daha fazla"
              className={cn(buttonVariants({ variant: "ghost", size: "icon" }))}
            >
              <MoreVertical className="h-4 w-4" />
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuItem onClick={() => setEditOpen(true)}>
                <Pencil className="mr-2 h-4 w-4" /> Düzenle
              </DropdownMenuItem>
              <DropdownMenuItem
                variant="destructive"
                onClick={() => {
                  if (confirm("Bileşeni sil? Bağlı kayıtlar da soft-delete olur.")) remove.mutate();
                }}
              >
                <Trash2 className="mr-2 h-4 w-4" /> Sil
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>

      <Card>
        <CardHeader className="pb-3"><CardTitle className="text-base">Durum notu</CardTitle></CardHeader>
        <CardContent>
          <StatusNoteEditor componentId={c.id} value={c.currentStatusNote} />
        </CardContent>
      </Card>

      <Tabs defaultValue="worklogs">
        <TabsList>
          <TabsTrigger value="worklogs">Worklog</TabsTrigger>
          <TabsTrigger value="decisions">Kararlar</TabsTrigger>
          <TabsTrigger value="next-steps">Adımlar</TabsTrigger>
          <TabsTrigger value="ideas">Fikirler</TabsTrigger>
          <TabsTrigger value="resources">Kaynaklar</TabsTrigger>
        </TabsList>
        <TabsContent value="worklogs" className="mt-6"><WorklogTab scope={{ kind: "component", id: componentId }} /></TabsContent>
        <TabsContent value="decisions" className="mt-6"><DecisionTab scope={{ kind: "component", id: componentId }} /></TabsContent>
        <TabsContent value="next-steps" className="mt-6"><NextStepTab scope={{ kind: "component", id: componentId }} /></TabsContent>
        <TabsContent value="ideas" className="mt-6"><IdeaTab scope={{ kind: "component", id: componentId }} /></TabsContent>
        <TabsContent value="resources" className="mt-6"><ResourceTab scope={{ kind: "component", id: componentId }} /></TabsContent>
      </Tabs>

      <ComponentFormModal open={editOpen} onOpenChange={setEditOpen} projectId={c.projectId} initial={c} />
    </div>
  );
}
