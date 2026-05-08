"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Pencil, Plus, Trash2 } from "lucide-react";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { EmptyState } from "@/components/common/EmptyState";
import { Field } from "@/components/common/Field";
import type { Scope } from "@/components/common/Scope";
import { scopeKey, scopeToOwner } from "@/components/common/Scope";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Skeleton } from "@/components/ui/skeleton";
import { Textarea } from "@/components/ui/textarea";
import { decisionsApi } from "@/lib/api/decisions";
import { formatRelative } from "@/lib/date";
import { errorMessage } from "@/lib/error";
import type { DecisionResponse } from "@/types/activity";

type Values = { title: string; reasoning: string; alternatives: string };

function fetcher(scope: Scope) {
  switch (scope.kind) {
    case "project":
      return decisionsApi.forProject(scope.id);
    case "component":
      return decisionsApi.forComponent(scope.id);
    case "track":
      return decisionsApi.forTrack(scope.id);
    case "module":
      return decisionsApi.forModule(scope.id);
  }
}

function DecisionForm({
  open,
  onOpenChange,
  scope,
  initial,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  scope: Scope;
  initial?: DecisionResponse;
}) {
  const qc = useQueryClient();
  const isEdit = !!initial;
  const { register, handleSubmit, reset, formState } = useForm<Values>({
    defaultValues: {
      title: initial?.title ?? "",
      reasoning: initial?.reasoning ?? "",
      alternatives: initial?.alternatives ?? "",
    },
  });
  useEffect(() => {
    if (open) reset({ title: initial?.title ?? "", reasoning: initial?.reasoning ?? "", alternatives: initial?.alternatives ?? "" });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, initial?.id]);

  const create = useMutation({
    mutationFn: (v: Values) =>
      decisionsApi.create({
        owner: scopeToOwner(scope),
        title: v.title,
        reasoning: v.reasoning,
        alternatives: v.alternatives || null,
      }),
    onSuccess: () => {
      toast.success("Karar kaydedildi.");
      qc.invalidateQueries({ queryKey: ["decisions"] });
      qc.invalidateQueries({ queryKey: ["resume"] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const update = useMutation({
    mutationFn: (v: Values) =>
      decisionsApi.update(initial!.id, { title: v.title, reasoning: v.reasoning, alternatives: v.alternatives || null }),
    onSuccess: () => {
      toast.success("Güncellendi.");
      qc.invalidateQueries({ queryKey: ["decisions"] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle>{isEdit ? "Kararı düzenle" : "Yeni karar"}</DialogTitle>
        </DialogHeader>
        <form
          onSubmit={handleSubmit((v) => (isEdit ? update.mutate(v) : create.mutate(v)))}
          className="space-y-4"
        >
          <Field label="Başlık" required error={formState.errors.title?.message}>
            <Input autoFocus {...register("title", { required: "Başlık gerekli." })} />
          </Field>
          <Field label="Gerekçe" required error={formState.errors.reasoning?.message}>
            <Textarea rows={4} {...register("reasoning", { required: "Gerekçe gerekli." })} />
          </Field>
          <Field label="Alternatifler">
            <Textarea rows={3} {...register("alternatives")} />
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

export function DecisionTab({ scope }: { scope: Scope }) {
  const qc = useQueryClient();
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<DecisionResponse | null>(null);

  const list = useQuery({
    queryKey: ["decisions", "by-scope", ...scopeKey(scope)],
    queryFn: () => fetcher(scope),
  });

  const remove = useMutation({
    mutationFn: (id: number) => decisionsApi.remove(id),
    onSuccess: () => {
      toast.success("Karar silindi.");
      qc.invalidateQueries({ queryKey: ["decisions"] });
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-medium">Kararlar</h2>
        <Button size="sm" onClick={() => { setEditing(null); setFormOpen(true); }}>
          <Plus className="h-4 w-4" /> Yeni karar
        </Button>
      </div>
      {list.isLoading ? (
        <div className="space-y-3">{[0, 1].map((i) => <Skeleton key={i} className="h-24 w-full" />)}</div>
      ) : !list.data?.length ? (
        <EmptyState
          title="Henüz karar yok."
          description="Verdiğin önemli kararları, neden öyle karar verdiğinle birlikte not et."
          action={<Button size="sm" onClick={() => { setEditing(null); setFormOpen(true); }}><Plus className="h-4 w-4" /> İlk karar</Button>}
        />
      ) : (
        <div className="space-y-3">
          {list.data.map((d) => (
            <Card key={d.id}>
              <CardHeader className="flex flex-row items-start justify-between gap-2 pb-2">
                <CardTitle className="text-base font-semibold">{d.title}</CardTitle>
                <div className="flex items-center gap-1">
                  <span className="text-xs text-muted-foreground">{formatRelative(d.decidedAt)}</span>
                  <Button size="icon-xs" variant="ghost" onClick={() => { setEditing(d); setFormOpen(true); }} aria-label="Düzenle"><Pencil className="h-3 w-3" /></Button>
                  <Button size="icon-xs" variant="ghost" onClick={() => { if (confirm("Sil?")) remove.mutate(d.id); }} aria-label="Sil"><Trash2 className="h-3 w-3" /></Button>
                </div>
              </CardHeader>
              <CardContent className="space-y-2 pt-0">
                <p className="whitespace-pre-wrap text-sm">{d.reasoning}</p>
                {d.alternatives ? (
                  <div className="rounded-md bg-muted/50 p-2 text-xs">
                    <span className="font-medium">Alternatifler:</span>{" "}
                    <span className="text-muted-foreground">{d.alternatives}</span>
                  </div>
                ) : null}
              </CardContent>
            </Card>
          ))}
        </div>
      )}
      <DecisionForm open={formOpen} onOpenChange={setFormOpen} scope={scope} initial={editing ?? undefined} />
    </div>
  );
}
