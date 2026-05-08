"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { Field } from "@/components/common/Field";
import type { Scope } from "@/components/common/Scope";
import { scopeToOwner } from "@/components/common/Scope";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Textarea } from "@/components/ui/textarea";
import { worklogsApi } from "@/lib/api/worklogs";
import { errorMessage } from "@/lib/error";
import type { WorklogResponse } from "@/types/activity";

type Values = {
  whatIDid: string;
  whatsLeft: string;
};

export function WorklogForm({
  open,
  onOpenChange,
  scope,
  initial,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  scope: Scope;
  initial?: WorklogResponse;
}) {
  const qc = useQueryClient();
  const isEdit = !!initial;
  const { register, handleSubmit, reset, formState } = useForm<Values>({
    defaultValues: {
      whatIDid: initial?.whatIDid ?? "",
      whatsLeft: initial?.whatsLeft ?? "",
    },
  });

  useEffect(() => {
    if (open) {
      reset({ whatIDid: initial?.whatIDid ?? "", whatsLeft: initial?.whatsLeft ?? "" });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, initial?.id]);

  const create = useMutation({
    mutationFn: (v: Values) => worklogsApi.create({ owner: scopeToOwner(scope), whatIDid: v.whatIDid, whatsLeft: v.whatsLeft || null }),
    onSuccess: () => {
      toast.success("Worklog kaydedildi.");
      qc.invalidateQueries({ queryKey: ["worklogs"] });
      qc.invalidateQueries({ queryKey: ["resume"] });
      qc.invalidateQueries({ queryKey: ["dashboard"] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const update = useMutation({
    mutationFn: (v: Values) => worklogsApi.update(initial!.id, { whatIDid: v.whatIDid, whatsLeft: v.whatsLeft || null }),
    onSuccess: () => {
      toast.success("Worklog güncellendi.");
      qc.invalidateQueries({ queryKey: ["worklogs"] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  const submitting = create.isPending || update.isPending;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle>{isEdit ? "Worklog düzenle" : "Yeni worklog"}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit((v) => (isEdit ? update.mutate(v) : create.mutate(v)))} className="space-y-4">
          <Field label="Ne yaptım" required error={formState.errors.whatIDid?.message}>
            <Textarea
              autoFocus
              rows={4}
              placeholder="Bugün ne ilerledi?"
              {...register("whatIDid", { required: "Doldur." })}
            />
          </Field>
          <Field label="Geriye ne kaldı">
            <Textarea
              rows={3}
              placeholder="Yarın bunu yapmam lazım…"
              {...register("whatsLeft")}
            />
          </Field>
          <DialogFooter>
            <Button type="button" variant="ghost" onClick={() => onOpenChange(false)}>
              Vazgeç
            </Button>
            <Button type="submit" disabled={submitting}>{submitting ? "Kaydediliyor…" : isEdit ? "Güncelle" : "Kaydet"}</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
