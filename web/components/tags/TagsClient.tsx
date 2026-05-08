"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Pencil, Plus, Trash2 } from "lucide-react";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { EmptyState } from "@/components/common/EmptyState";
import { Field } from "@/components/common/Field";
import { PageHeader } from "@/components/common/PageHeader";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Skeleton } from "@/components/ui/skeleton";
import { tagsApi } from "@/lib/api/tags";
import { errorMessage } from "@/lib/error";
import type { TagResponse } from "@/types/tag";

type Values = { name: string; color: string };

function TagForm({
  open,
  onOpenChange,
  initial,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  initial?: TagResponse;
}) {
  const qc = useQueryClient();
  const isEdit = !!initial;
  const { register, handleSubmit, reset, formState } = useForm<Values>({
    defaultValues: { name: initial?.name ?? "", color: initial?.color ?? "#3B82F6" },
  });
  useEffect(() => {
    if (open) reset({ name: initial?.name ?? "", color: initial?.color ?? "#3B82F6" });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, initial?.id]);

  const create = useMutation({
    mutationFn: (v: Values) => tagsApi.create({ name: v.name, color: v.color || null }),
    onSuccess: () => {
      toast.success("Etiket oluşturuldu.");
      qc.invalidateQueries({ queryKey: ["tags"] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const update = useMutation({
    mutationFn: (v: Values) => tagsApi.update(initial!.id, { name: v.name, color: v.color || null }),
    onSuccess: () => {
      toast.success("Güncellendi.");
      qc.invalidateQueries({ queryKey: ["tags"] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader><DialogTitle>{isEdit ? "Etiketi düzenle" : "Yeni etiket"}</DialogTitle></DialogHeader>
        <form onSubmit={handleSubmit((v) => (isEdit ? update.mutate(v) : create.mutate(v)))} className="space-y-4">
          <Field label="İsim" required error={formState.errors.name?.message}>
            <Input autoFocus {...register("name", { required: "İsim gerekli." })} />
          </Field>
          <Field label="Renk" hint="Hex format (#RRGGBB)">
            <div className="flex items-center gap-2">
              <Input type="color" className="h-9 w-16 p-1" {...register("color")} />
              <Input {...register("color", { pattern: { value: /^#[0-9A-Fa-f]{6}$/, message: "Geçerli hex değil." } })} />
            </div>
            {formState.errors.color ? <p className="text-xs text-destructive">{formState.errors.color.message}</p> : null}
          </Field>
          <DialogFooter>
            <Button type="button" variant="ghost" onClick={() => onOpenChange(false)}>Vazgeç</Button>
            <Button type="submit" disabled={create.isPending || update.isPending}>
              {create.isPending || update.isPending ? "Kaydediliyor…" : isEdit ? "Güncelle" : "Oluştur"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

export function TagsClient() {
  const qc = useQueryClient();
  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<TagResponse | null>(null);

  const list = useQuery({ queryKey: ["tags"], queryFn: () => tagsApi.list() });
  const remove = useMutation({
    mutationFn: (id: number) => tagsApi.remove(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["tags"] }),
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <div className="space-y-6">
      <PageHeader
        title="Etiketler"
        description="Projeleri/eğitimleri etiketle, sonra filtrele."
        actions={<Button onClick={() => { setEditing(null); setFormOpen(true); }}><Plus className="h-4 w-4" /> Yeni etiket</Button>}
      />
      {list.isLoading ? (
        <Skeleton className="h-32 w-full" />
      ) : !list.data?.length ? (
        <EmptyState
          title="Etiket yok."
          description="İlk etiketini oluştur (örn. .NET, Claude, MVP)."
          action={<Button onClick={() => { setEditing(null); setFormOpen(true); }}><Plus className="h-4 w-4" /> Yeni etiket</Button>}
        />
      ) : (
        <div className="grid grid-cols-1 gap-3 md:grid-cols-2">
          {list.data.map((t) => (
            <Card key={t.id}>
              <CardContent className="flex items-center justify-between gap-2 py-3">
                <div className="flex items-center gap-2">
                  <Badge
                    variant="outline"
                    style={t.color ? { borderColor: t.color, color: t.color } : undefined}
                  >
                    {t.name}
                  </Badge>
                </div>
                <div className="flex items-center gap-1">
                  <Button size="icon-xs" variant="ghost" onClick={() => { setEditing(t); setFormOpen(true); }}><Pencil className="h-3 w-3" /></Button>
                  <Button size="icon-xs" variant="ghost" onClick={() => { if (confirm("Sil?")) remove.mutate(t.id); }}><Trash2 className="h-3 w-3" /></Button>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
      <TagForm open={formOpen} onOpenChange={setFormOpen} initial={editing ?? undefined} />
    </div>
  );
}
