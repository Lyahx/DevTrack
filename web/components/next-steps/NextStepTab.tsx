"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Pencil, Plus, Trash2 } from "lucide-react";
import { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "sonner";
import { EmptyState } from "@/components/common/EmptyState";
import { Field } from "@/components/common/Field";
import type { Scope } from "@/components/common/Scope";
import { scopeKey, scopeToOwner } from "@/components/common/Scope";
import { PriorityBadge } from "@/components/common/StatusBadge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { nextStepsApi } from "@/lib/api/nextSteps";
import { errorMessage } from "@/lib/error";
import { NEXT_STEP_PRIORITIES, PRIORITY_LABELS } from "@/types/enums";
import type { NextStepResponse } from "@/types/activity";
import type { NextStepPriority } from "@/types/enums";

type Values = { description: string; priority: NextStepPriority };

function fetcher(scope: Scope) {
  switch (scope.kind) {
    case "project":
      return nextStepsApi.forProject(scope.id);
    case "component":
      return nextStepsApi.forComponent(scope.id);
    case "track":
      return nextStepsApi.forTrack(scope.id);
    case "module":
      return nextStepsApi.forModule(scope.id);
  }
}

function NextStepForm({
  open,
  onOpenChange,
  scope,
  initial,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  scope: Scope;
  initial?: NextStepResponse;
}) {
  const qc = useQueryClient();
  const isEdit = !!initial;
  const { register, handleSubmit, reset, control, formState } = useForm<Values>({
    defaultValues: {
      description: initial?.description ?? "",
      priority: initial?.priority ?? "Medium",
    },
  });
  useEffect(() => {
    if (open) reset({ description: initial?.description ?? "", priority: initial?.priority ?? "Medium" });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, initial?.id]);

  const create = useMutation({
    mutationFn: (v: Values) => nextStepsApi.create({ owner: scopeToOwner(scope), description: v.description, priority: v.priority }),
    onSuccess: () => {
      toast.success("Adım kaydedildi.");
      qc.invalidateQueries({ queryKey: ["next-steps"] });
      qc.invalidateQueries({ queryKey: ["resume"] });
      qc.invalidateQueries({ queryKey: ["dashboard"] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const update = useMutation({
    mutationFn: (v: Values) => nextStepsApi.update(initial!.id, v),
    onSuccess: () => {
      toast.success("Güncellendi.");
      qc.invalidateQueries({ queryKey: ["next-steps"] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg">
        <DialogHeader><DialogTitle>{isEdit ? "Adımı düzenle" : "Yeni adım"}</DialogTitle></DialogHeader>
        <form onSubmit={handleSubmit((v) => (isEdit ? update.mutate(v) : create.mutate(v)))} className="space-y-4">
          <Field label="Açıklama" required error={formState.errors.description?.message}>
            <Input autoFocus {...register("description", { required: "Açıklama gerekli." })} />
          </Field>
          <Field label="Öncelik">
            <Controller
              control={control}
              name="priority"
              render={({ field }) => (
                <Select value={field.value} onValueChange={field.onChange}>
                  <SelectTrigger className="w-full"><SelectValue /></SelectTrigger>
                  <SelectContent>
                    {NEXT_STEP_PRIORITIES.map((p) => (
                      <SelectItem key={p} value={p}>{PRIORITY_LABELS[p]}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              )}
            />
          </Field>
          <DialogFooter>
            <Button type="button" variant="ghost" onClick={() => onOpenChange(false)}>Vazgeç</Button>
            <Button type="submit" disabled={create.isPending || update.isPending}>
              {create.isPending || update.isPending ? "Kaydediliyor…" : isEdit ? "Güncelle" : "Kaydet"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

export function NextStepTab({ scope }: { scope: Scope }) {
  const qc = useQueryClient();
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<NextStepResponse | null>(null);

  const list = useQuery({
    queryKey: ["next-steps", "by-scope", ...scopeKey(scope)],
    queryFn: () => fetcher(scope),
  });

  const toggle = useMutation({
    mutationFn: ({ id, completed }: { id: number; completed: boolean }) =>
      completed ? nextStepsApi.uncomplete(id) : nextStepsApi.complete(id),
    onMutate: async ({ id, completed }) => {
      const key = ["next-steps", "by-scope", ...scopeKey(scope)] as const;
      await qc.cancelQueries({ queryKey: key });
      const prev = qc.getQueryData<NextStepResponse[]>(key);
      qc.setQueryData<NextStepResponse[]>(key, (old) =>
        (old ?? []).map((s) => (s.id === id ? { ...s, isCompleted: !completed, completedAt: completed ? null : new Date().toISOString() } : s)),
      );
      return { prev };
    },
    onError: (e, _v, ctx) => {
      if (ctx?.prev) qc.setQueryData(["next-steps", "by-scope", ...scopeKey(scope)], ctx.prev);
      toast.error(errorMessage(e));
    },
    onSettled: () => {
      qc.invalidateQueries({ queryKey: ["next-steps"] });
    },
  });

  const remove = useMutation({
    mutationFn: (id: number) => nextStepsApi.remove(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["next-steps"] }),
    onError: (e) => toast.error(errorMessage(e)),
  });

  const open = list.data?.filter((s) => !s.isCompleted) ?? [];
  const done = list.data?.filter((s) => s.isCompleted) ?? [];

  function renderItem(s: NextStepResponse) {
    return (
      <Card key={s.id}>
        <CardHeader className="flex flex-row items-start gap-3 pb-3">
          <Checkbox
            className="mt-0.5"
            checked={s.isCompleted}
            onCheckedChange={() => toggle.mutate({ id: s.id, completed: s.isCompleted })}
          />
          <div className="flex-1">
            <p className={`text-sm ${s.isCompleted ? "text-muted-foreground line-through" : ""}`}>{s.description}</p>
            <div className="mt-1.5 flex items-center gap-2">
              <PriorityBadge priority={s.priority} />
            </div>
          </div>
          <div className="flex items-center gap-1">
            <Button size="icon-xs" variant="ghost" onClick={() => { setEditing(s); setFormOpen(true); }} aria-label="Düzenle"><Pencil className="h-3 w-3" /></Button>
            <Button size="icon-xs" variant="ghost" onClick={() => { if (confirm("Sil?")) remove.mutate(s.id); }} aria-label="Sil"><Trash2 className="h-3 w-3" /></Button>
          </div>
        </CardHeader>
      </Card>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-medium">Sonraki adımlar</h2>
        <Button size="sm" onClick={() => { setEditing(null); setFormOpen(true); }}>
          <Plus className="h-4 w-4" /> Yeni adım
        </Button>
      </div>
      {list.isLoading ? (
        <div className="space-y-3">{[0, 1].map((i) => <Skeleton key={i} className="h-16 w-full" />)}</div>
      ) : !list.data?.length ? (
        <EmptyState
          title="Açık adım yok."
          description="Bir sonraki yapacağın şey ne? Yaz."
          action={<Button size="sm" onClick={() => { setEditing(null); setFormOpen(true); }}><Plus className="h-4 w-4" /> İlk adım</Button>}
        />
      ) : (
        <div className="space-y-4">
          {open.length > 0 && <div className="space-y-2">{open.map(renderItem)}</div>}
          {done.length > 0 && (
            <details className="rounded-md">
              <summary className="cursor-pointer text-sm text-muted-foreground">Tamamlananlar ({done.length})</summary>
              <div className="mt-3 space-y-2">{done.map(renderItem)}</div>
            </details>
          )}
        </div>
      )}
      <NextStepForm open={formOpen} onOpenChange={setFormOpen} scope={scope} initial={editing ?? undefined} />
    </div>
  );
}
