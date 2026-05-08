"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowLeft, MoreVertical, Pencil, Trash2 } from "lucide-react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { toast } from "sonner";
import { DecisionTab } from "@/components/decisions/DecisionTab";
import { IdeaTab } from "@/components/ideas/IdeaTab";
import { LearningModuleFormModal } from "@/components/learning/LearningModuleFormModal";
import { NextStepTab } from "@/components/next-steps/NextStepTab";
import { ResourceTab } from "@/components/resources/ResourceTab";
import { ModuleStatusBadge } from "@/components/common/StatusBadge";
import { WorklogTab } from "@/components/worklogs/WorklogTab";
import { buttonVariants } from "@/components/ui/button";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Skeleton } from "@/components/ui/skeleton";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { learningModulesApi } from "@/lib/api/learning";
import { errorMessage } from "@/lib/error";
import type { LearningModuleStatus } from "@/types/enums";
import { cn } from "@/lib/utils";

export function LearningModuleDetailClient({ trackId, moduleId }: { trackId: number; moduleId: number }) {
  const router = useRouter();
  const qc = useQueryClient();
  const [editOpen, setEditOpen] = useState(false);

  const moduleQuery = useQuery({ queryKey: ["module", moduleId], queryFn: () => learningModulesApi.get(moduleId) });

  const setStatus = useMutation({
    mutationFn: (status: LearningModuleStatus) => learningModulesApi.setStatus(moduleId, { status }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["module", moduleId] });
      qc.invalidateQueries({ queryKey: ["modules", trackId] });
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const remove = useMutation({
    mutationFn: () => learningModulesApi.remove(moduleId),
    onSuccess: () => {
      toast.success("Modül silindi.");
      qc.invalidateQueries({ queryKey: ["modules", trackId] });
      router.push(`/learning/${trackId}`);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  if (moduleQuery.isLoading) return <Skeleton className="h-48 w-full" />;
  if (!moduleQuery.data) return <p>Modül bulunamadı.</p>;

  const m = moduleQuery.data;

  return (
    <div className="space-y-6">
      <Link href={`/learning/${trackId}`} className="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground">
        <ArrowLeft className="h-3 w-3" /> Eğitime dön
      </Link>
      <div className="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
        <div>
          <div className="flex items-center gap-3">
            <span className="text-sm text-muted-foreground">#{m.order}</span>
            <h1 className="text-2xl font-semibold tracking-tight">{m.name}</h1>
            <ModuleStatusBadge status={m.status} />
          </div>
        </div>
        <div className="flex items-center gap-2">
          <Link href={`/learning/${trackId}/modules/${moduleId}/resume`} className={cn(buttonVariants({ variant: "default" }))}>Resume Mode</Link>
          <Select value={m.status} onValueChange={(v) => setStatus.mutate(v as LearningModuleStatus)}>
            <SelectTrigger className="w-36"><SelectValue /></SelectTrigger>
            <SelectContent>
              <SelectItem value="NotStarted">Başlamadı</SelectItem>
              <SelectItem value="InProgress">Devam ediyor</SelectItem>
              <SelectItem value="Completed">Tamamlandı</SelectItem>
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
              <DropdownMenuItem onClick={() => setEditOpen(true)}>
                <Pencil className="mr-2 h-4 w-4" /> Düzenle
              </DropdownMenuItem>
              <DropdownMenuItem
                variant="destructive"
                onClick={() => {
                  if (confirm("Modülü sil?")) remove.mutate();
                }}
              >
                <Trash2 className="mr-2 h-4 w-4" /> Sil
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>

      <Tabs defaultValue="worklogs">
        <TabsList>
          <TabsTrigger value="worklogs">Worklog</TabsTrigger>
          <TabsTrigger value="decisions">Kararlar</TabsTrigger>
          <TabsTrigger value="next-steps">Adımlar</TabsTrigger>
          <TabsTrigger value="ideas">Fikirler</TabsTrigger>
          <TabsTrigger value="resources">Kaynaklar</TabsTrigger>
        </TabsList>
        <TabsContent value="worklogs" className="mt-6"><WorklogTab scope={{ kind: "module", id: moduleId }} /></TabsContent>
        <TabsContent value="decisions" className="mt-6"><DecisionTab scope={{ kind: "module", id: moduleId }} /></TabsContent>
        <TabsContent value="next-steps" className="mt-6"><NextStepTab scope={{ kind: "module", id: moduleId }} /></TabsContent>
        <TabsContent value="ideas" className="mt-6"><IdeaTab scope={{ kind: "module", id: moduleId }} /></TabsContent>
        <TabsContent value="resources" className="mt-6"><ResourceTab scope={{ kind: "module", id: moduleId }} /></TabsContent>
      </Tabs>

      <LearningModuleFormModal open={editOpen} onOpenChange={setEditOpen} trackId={trackId} initial={m} />
    </div>
  );
}
