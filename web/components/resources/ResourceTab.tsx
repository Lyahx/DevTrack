"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ExternalLink, Pencil, Plus, Trash2 } from "lucide-react";
import { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "sonner";
import { EmptyState } from "@/components/common/EmptyState";
import { Field } from "@/components/common/Field";
import type { Scope } from "@/components/common/Scope";
import { scopeKey, scopeToOwner } from "@/components/common/Scope";
import { ResourceTypeBadge } from "@/components/common/StatusBadge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { Textarea } from "@/components/ui/textarea";
import { resourcesApi } from "@/lib/api/resources";
import { errorMessage } from "@/lib/error";
import { RESOURCE_TYPES, RESOURCE_TYPE_LABELS } from "@/types/enums";
import type { ResourceResponse } from "@/types/activity";
import type { ResourceType } from "@/types/enums";

type Values = { title: string; url: string; type: ResourceType; notes: string };

function fetcher(scope: Scope) {
  switch (scope.kind) {
    case "project":
      return resourcesApi.forProject(scope.id);
    case "component":
      return resourcesApi.forComponent(scope.id);
    case "track":
      return resourcesApi.forTrack(scope.id);
    case "module":
      return resourcesApi.forModule(scope.id);
  }
}

function ResourceForm({
  open,
  onOpenChange,
  scope,
  initial,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  scope: Scope;
  initial?: ResourceResponse;
}) {
  const qc = useQueryClient();
  const isEdit = !!initial;
  const { register, handleSubmit, reset, control, formState } = useForm<Values>({
    defaultValues: {
      title: initial?.title ?? "",
      url: initial?.url ?? "",
      type: initial?.type ?? "Other",
      notes: initial?.notes ?? "",
    },
  });
  useEffect(() => {
    if (open) reset({ title: initial?.title ?? "", url: initial?.url ?? "", type: initial?.type ?? "Other", notes: initial?.notes ?? "" });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, initial?.id]);

  const create = useMutation({
    mutationFn: (v: Values) => resourcesApi.create({ owner: scopeToOwner(scope), title: v.title, url: v.url, type: v.type, notes: v.notes || null }),
    onSuccess: () => {
      toast.success("Kaynak kaydedildi.");
      qc.invalidateQueries({ queryKey: ["resources"] });
      qc.invalidateQueries({ queryKey: ["resume"] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const update = useMutation({
    mutationFn: (v: Values) => resourcesApi.update(initial!.id, { title: v.title, url: v.url, type: v.type, notes: v.notes || null }),
    onSuccess: () => {
      toast.success("Güncellendi.");
      qc.invalidateQueries({ queryKey: ["resources"] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg">
        <DialogHeader><DialogTitle>{isEdit ? "Kaynağı düzenle" : "Yeni kaynak"}</DialogTitle></DialogHeader>
        <form onSubmit={handleSubmit((v) => (isEdit ? update.mutate(v) : create.mutate(v)))} className="space-y-4">
          <Field label="Başlık" required error={formState.errors.title?.message}>
            <Input autoFocus {...register("title", { required: "Başlık gerekli." })} />
          </Field>
          <Field label="URL" required error={formState.errors.url?.message}>
            <Input
              type="url"
              placeholder="https://…"
              {...register("url", {
                required: "URL gerekli.",
                pattern: { value: /^https?:\/\/.+/i, message: "Geçerli bir URL gir." },
              })}
            />
          </Field>
          <Field label="Tür">
            <Controller
              control={control}
              name="type"
              render={({ field }) => (
                <Select value={field.value} onValueChange={field.onChange}>
                  <SelectTrigger className="w-full"><SelectValue /></SelectTrigger>
                  <SelectContent>
                    {RESOURCE_TYPES.map((t) => (
                      <SelectItem key={t} value={t}>{RESOURCE_TYPE_LABELS[t]}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              )}
            />
          </Field>
          <Field label="Notlar">
            <Textarea rows={3} {...register("notes")} />
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

export function ResourceTab({ scope }: { scope: Scope }) {
  const qc = useQueryClient();
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<ResourceResponse | null>(null);

  const list = useQuery({
    queryKey: ["resources", "by-scope", ...scopeKey(scope)],
    queryFn: () => fetcher(scope),
  });

  const remove = useMutation({
    mutationFn: (id: number) => resourcesApi.remove(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["resources"] }),
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-medium">Kaynaklar</h2>
        <Button size="sm" onClick={() => { setEditing(null); setFormOpen(true); }}>
          <Plus className="h-4 w-4" /> Yeni kaynak
        </Button>
      </div>
      {list.isLoading ? (
        <div className="space-y-2">{[0, 1].map((i) => <Skeleton key={i} className="h-16 w-full" />)}</div>
      ) : !list.data?.length ? (
        <EmptyState
          title="Henüz kaynak yok."
          description="Claude konuşmaları, dokümanlar, video — referans olabilecek her şey buraya."
          action={<Button size="sm" onClick={() => { setEditing(null); setFormOpen(true); }}><Plus className="h-4 w-4" /> İlk kaynak</Button>}
        />
      ) : (
        <div className="grid grid-cols-1 gap-3 md:grid-cols-2">
          {list.data.map((r) => (
            <Card key={r.id}>
              <CardHeader className="flex flex-row items-start justify-between gap-2 pb-2">
                <div className="min-w-0">
                  <CardTitle className="truncate text-base">{r.title}</CardTitle>
                  <ResourceTypeBadge type={r.type} />
                </div>
                <div className="flex items-center gap-1">
                  <a href={r.url} target="_blank" rel="noopener noreferrer" className="text-muted-foreground hover:text-foreground" aria-label="Aç"><ExternalLink className="h-4 w-4" /></a>
                  <Button size="icon-xs" variant="ghost" onClick={() => { setEditing(r); setFormOpen(true); }} aria-label="Düzenle"><Pencil className="h-3 w-3" /></Button>
                  <Button size="icon-xs" variant="ghost" onClick={() => { if (confirm("Sil?")) remove.mutate(r.id); }} aria-label="Sil"><Trash2 className="h-3 w-3" /></Button>
                </div>
              </CardHeader>
              {r.notes ? (
                <CardContent className="pt-0 text-sm text-muted-foreground">{r.notes}</CardContent>
              ) : null}
            </Card>
          ))}
        </div>
      )}
      <ResourceForm open={formOpen} onOpenChange={setFormOpen} scope={scope} initial={editing ?? undefined} />
    </div>
  );
}
