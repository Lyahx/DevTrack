"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "sonner";
import { Field } from "@/components/common/Field";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { componentsApi } from "@/lib/api/components";
import { errorMessage } from "@/lib/error";
import { COMPONENT_TYPES, COMPONENT_TYPE_LABELS } from "@/types/enums";
import type { ComponentResponse } from "@/types/component";
import type { ComponentType } from "@/types/enums";

type Values = {
  name: string;
  type: ComponentType;
  techStack: string;
  localUrl: string;
  repoPath: string;
  currentStatusNote: string;
};

export function ComponentFormModal({
  open,
  onOpenChange,
  projectId,
  initial,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  projectId: number;
  initial?: ComponentResponse;
}) {
  const qc = useQueryClient();
  const isEdit = !!initial;
  const { register, handleSubmit, reset, control, formState } = useForm<Values>({
    defaultValues: {
      name: initial?.name ?? "",
      type: initial?.type ?? "Other",
      techStack: initial?.techStack ?? "",
      localUrl: initial?.localUrl ?? "",
      repoPath: initial?.repoPath ?? "",
      currentStatusNote: initial?.currentStatusNote ?? "",
    },
  });

  useEffect(() => {
    if (open) {
      reset({
        name: initial?.name ?? "",
        type: initial?.type ?? "Other",
        techStack: initial?.techStack ?? "",
        localUrl: initial?.localUrl ?? "",
        repoPath: initial?.repoPath ?? "",
        currentStatusNote: initial?.currentStatusNote ?? "",
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, initial?.id]);

  const create = useMutation({
    mutationFn: (v: Values) =>
      componentsApi.create(projectId, {
        name: v.name,
        type: v.type,
        techStack: v.techStack || null,
        localUrl: v.localUrl || null,
        repoPath: v.repoPath || null,
        currentStatusNote: v.currentStatusNote || null,
      }),
    onSuccess: () => {
      toast.success("Bileşen oluşturuldu.");
      qc.invalidateQueries({ queryKey: ["project", projectId] });
      qc.invalidateQueries({ queryKey: ["components", projectId] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const update = useMutation({
    mutationFn: (v: Values) =>
      componentsApi.update(initial!.id, {
        name: v.name,
        type: v.type,
        techStack: v.techStack || null,
        localUrl: v.localUrl || null,
        repoPath: v.repoPath || null,
        currentStatusNote: v.currentStatusNote || null,
      }),
    onSuccess: () => {
      toast.success("Bileşen güncellendi.");
      qc.invalidateQueries({ queryKey: ["component", initial!.id] });
      qc.invalidateQueries({ queryKey: ["components", projectId] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  const submitting = create.isPending || update.isPending;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle>{isEdit ? "Bileşeni düzenle" : "Yeni bileşen"}</DialogTitle>
          <DialogDescription>Projenin bir parçası: API, web app, mobil…</DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit((v) => (isEdit ? update.mutate(v) : create.mutate(v)))} className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <Field label="İsim" required error={formState.errors.name?.message}>
              <Input autoFocus {...register("name", { required: "İsim gerekli." })} />
            </Field>
            <Field label="Tür">
              <Controller
                control={control}
                name="type"
                render={({ field }) => (
                  <Select value={field.value} onValueChange={field.onChange}>
                    <SelectTrigger className="w-full"><SelectValue /></SelectTrigger>
                    <SelectContent>
                      {COMPONENT_TYPES.map((t) => (
                        <SelectItem key={t} value={t}>{COMPONENT_TYPE_LABELS[t]}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                )}
              />
            </Field>
          </div>
          <Field label="Tech stack">
            <Input placeholder="Next.js, .NET 10, PostgreSQL…" {...register("techStack")} />
          </Field>
          <div className="grid grid-cols-2 gap-3">
            <Field label="Local URL">
              <Input placeholder="http://localhost:3000" {...register("localUrl")} />
            </Field>
            <Field label="Repo yolu">
              <Input placeholder="src/AOS.Api" {...register("repoPath")} />
            </Field>
          </div>
          <Field label="Şu an nerede kaldım">
            <Textarea rows={3} placeholder="Refresh token endpoint'ini eklemeye başladım, validator yarıda kaldı…" {...register("currentStatusNote")} />
          </Field>
          <DialogFooter>
            <Button type="button" variant="ghost" onClick={() => onOpenChange(false)}>Vazgeç</Button>
            <Button type="submit" disabled={submitting}>{submitting ? "Kaydediliyor…" : isEdit ? "Güncelle" : "Oluştur"}</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
