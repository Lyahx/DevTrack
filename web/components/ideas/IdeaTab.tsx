"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowRight, Pencil, Plus, Trash2 } from "lucide-react";
import { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "sonner";
import { EmptyState } from "@/components/common/EmptyState";
import { Field } from "@/components/common/Field";
import type { Scope } from "@/components/common/Scope";
import { scopeKey, scopeToOwner } from "@/components/common/Scope";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { Textarea } from "@/components/ui/textarea";
import { ideasApi } from "@/lib/api/ideas";
import { formatRelative } from "@/lib/date";
import { errorMessage } from "@/lib/error";
import { NEXT_STEP_PRIORITIES, PRIORITY_LABELS } from "@/types/enums";
import type { IdeaResponse } from "@/types/activity";
import type { NextStepPriority } from "@/types/enums";

function fetcher(scope: Scope) {
  switch (scope.kind) {
    case "project":
      return ideasApi.forProject(scope.id);
    case "component":
      return ideasApi.forComponent(scope.id);
    case "track":
      return ideasApi.forTrack(scope.id);
    case "module":
      return ideasApi.forModule(scope.id);
  }
}

function IdeaForm({
  open,
  onOpenChange,
  scope,
  initial,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  scope: Scope;
  initial?: IdeaResponse;
}) {
  const qc = useQueryClient();
  const isEdit = !!initial;
  const { register, handleSubmit, reset, formState } = useForm<{ content: string }>({
    defaultValues: { content: initial?.content ?? "" },
  });
  useEffect(() => {
    if (open) reset({ content: initial?.content ?? "" });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, initial?.id]);

  const create = useMutation({
    mutationFn: (v: { content: string }) => ideasApi.create({ owner: scopeToOwner(scope), content: v.content }),
    onSuccess: () => {
      toast.success("Fikir kaydedildi.");
      qc.invalidateQueries({ queryKey: ["ideas"] });
      qc.invalidateQueries({ queryKey: ["resume"] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const update = useMutation({
    mutationFn: (v: { content: string }) => ideasApi.update(initial!.id, v),
    onSuccess: () => {
      toast.success("Güncellendi.");
      qc.invalidateQueries({ queryKey: ["ideas"] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg">
        <DialogHeader><DialogTitle>{isEdit ? "Fikri düzenle" : "Yeni fikir"}</DialogTitle></DialogHeader>
        <form onSubmit={handleSubmit((v) => (isEdit ? update.mutate(v) : create.mutate(v)))} className="space-y-4">
          <Field label="İçerik" required error={formState.errors.content?.message}>
            <Textarea autoFocus rows={5} {...register("content", { required: "İçerik gerekli." })} />
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

function ConvertModal({
  idea,
  open,
  onOpenChange,
}: {
  idea: IdeaResponse | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}) {
  const qc = useQueryClient();
  const { handleSubmit, control, reset } = useForm<{ priority: NextStepPriority }>({
    defaultValues: { priority: "Medium" },
  });
  useEffect(() => { if (open) reset({ priority: "Medium" }); }, [open, reset]);

  const convert = useMutation({
    mutationFn: (v: { priority: NextStepPriority }) => ideasApi.convert(idea!.id, v),
    onSuccess: () => {
      toast.success("Fikir, sonraki adıma dönüştü.");
      qc.invalidateQueries({ queryKey: ["ideas"] });
      qc.invalidateQueries({ queryKey: ["next-steps"] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Sonraki adıma dönüştür</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit((v) => convert.mutate(v))} className="space-y-4">
          {idea ? <p className="rounded-md bg-muted/50 p-3 text-sm">{idea.content}</p> : null}
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
            <Button type="submit" disabled={convert.isPending}>{convert.isPending ? "Dönüştürülüyor…" : "Dönüştür"}</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

export function IdeaTab({ scope }: { scope: Scope }) {
  const qc = useQueryClient();
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<IdeaResponse | null>(null);
  const [convertOpen, setConvertOpen] = useState(false);
  const [converting, setConverting] = useState<IdeaResponse | null>(null);

  const list = useQuery({
    queryKey: ["ideas", "by-scope", ...scopeKey(scope)],
    queryFn: () => fetcher(scope),
  });

  const remove = useMutation({
    mutationFn: (id: number) => ideasApi.remove(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["ideas"] }),
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-medium">Fikirler</h2>
        <Button size="sm" onClick={() => { setEditing(null); setFormOpen(true); }}>
          <Plus className="h-4 w-4" /> Yeni fikir
        </Button>
      </div>
      {list.isLoading ? (
        <div className="space-y-2">{[0, 1].map((i) => <Skeleton key={i} className="h-20 w-full" />)}</div>
      ) : !list.data?.length ? (
        <EmptyState
          title="Henüz fikir yok."
          description="Aklına gelenleri buraya bırak — kaybolmasın."
          action={<Button size="sm" onClick={() => { setEditing(null); setFormOpen(true); }}><Plus className="h-4 w-4" /> İlk fikir</Button>}
        />
      ) : (
        <div className="space-y-2">
          {list.data.map((i) => (
            <Card key={i.id} className={i.isConvertedToNextStep ? "opacity-60" : undefined}>
              <CardHeader className="flex flex-row items-start justify-between gap-2 pb-2">
                <span className="text-xs text-muted-foreground">{formatRelative(i.capturedAt)}</span>
                <div className="flex items-center gap-1">
                  {!i.isConvertedToNextStep && (
                    <Button
                      size="xs"
                      variant="outline"
                      onClick={() => { setConverting(i); setConvertOpen(true); }}
                    >
                      Adıma dönüştür <ArrowRight className="h-3 w-3" />
                    </Button>
                  )}
                  <Button size="icon-xs" variant="ghost" onClick={() => { setEditing(i); setFormOpen(true); }} aria-label="Düzenle"><Pencil className="h-3 w-3" /></Button>
                  <Button size="icon-xs" variant="ghost" onClick={() => { if (confirm("Sil?")) remove.mutate(i.id); }} aria-label="Sil"><Trash2 className="h-3 w-3" /></Button>
                </div>
              </CardHeader>
              <CardContent className="pt-0">
                <p className="whitespace-pre-wrap text-sm">{i.content}</p>
                {i.isConvertedToNextStep ? (
                  <p className="mt-2 text-xs italic text-muted-foreground">Bu fikir bir sonraki adıma dönüştü.</p>
                ) : null}
              </CardContent>
            </Card>
          ))}
        </div>
      )}
      <IdeaForm open={formOpen} onOpenChange={setFormOpen} scope={scope} initial={editing ?? undefined} />
      <ConvertModal idea={converting} open={convertOpen} onOpenChange={setConvertOpen} />
    </div>
  );
}
