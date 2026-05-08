"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "sonner";
import { Field } from "@/components/common/Field";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { learningModulesApi } from "@/lib/api/learning";
import { errorMessage } from "@/lib/error";
import { LEARNING_MODULE_STATUSES, MODULE_STATUS_LABELS } from "@/types/enums";
import type { LearningModuleResponse } from "@/types/learning";
import type { LearningModuleStatus } from "@/types/enums";

type Values = { name: string; order: number; status: LearningModuleStatus };

export function LearningModuleFormModal({
  open,
  onOpenChange,
  trackId,
  initial,
  defaultOrder,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  trackId: number;
  initial?: LearningModuleResponse;
  defaultOrder?: number;
}) {
  const qc = useQueryClient();
  const isEdit = !!initial;
  const { register, handleSubmit, reset, control, formState } = useForm<Values>({
    defaultValues: {
      name: initial?.name ?? "",
      order: initial?.order ?? defaultOrder ?? 0,
      status: initial?.status ?? "NotStarted",
    },
  });

  useEffect(() => {
    if (open) {
      reset({
        name: initial?.name ?? "",
        order: initial?.order ?? defaultOrder ?? 0,
        status: initial?.status ?? "NotStarted",
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, initial?.id, defaultOrder]);

  const create = useMutation({
    mutationFn: (v: Values) => learningModulesApi.create(trackId, { name: v.name, order: v.order, status: v.status }),
    onSuccess: () => {
      toast.success("Modül oluşturuldu.");
      qc.invalidateQueries({ queryKey: ["modules", trackId] });
      qc.invalidateQueries({ queryKey: ["learning-track", trackId] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const update = useMutation({
    mutationFn: (v: Values) => learningModulesApi.update(initial!.id, { name: v.name, order: v.order }),
    onSuccess: () => {
      toast.success("Modül güncellendi.");
      qc.invalidateQueries({ queryKey: ["modules", trackId] });
      qc.invalidateQueries({ queryKey: ["module", initial!.id] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-md">
        <DialogHeader><DialogTitle>{isEdit ? "Modülü düzenle" : "Yeni modül"}</DialogTitle></DialogHeader>
        <form onSubmit={handleSubmit((v) => (isEdit ? update.mutate(v) : create.mutate(v)))} className="space-y-4">
          <Field label="İsim" required error={formState.errors.name?.message}>
            <Input autoFocus {...register("name", { required: "İsim gerekli." })} />
          </Field>
          <Field label="Sıra" required>
            <Input type="number" min={0} {...register("order", { valueAsNumber: true, min: 0 })} />
          </Field>
          {!isEdit && (
            <Field label="Durum">
              <Controller
                control={control}
                name="status"
                render={({ field }) => (
                  <Select value={field.value} onValueChange={field.onChange}>
                    <SelectTrigger className="w-full"><SelectValue /></SelectTrigger>
                    <SelectContent>
                      {LEARNING_MODULE_STATUSES.map((s) => (
                        <SelectItem key={s} value={s}>{MODULE_STATUS_LABELS[s]}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                )}
              />
            </Field>
          )}
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
