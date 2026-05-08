"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "sonner";
import { z } from "zod";
import { Field } from "@/components/common/Field";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { learningTracksApi } from "@/lib/api/learning";
import { errorMessage } from "@/lib/error";
import { LEARNING_TRACK_STATUSES, PROJECT_STATUS_LABELS } from "@/types/enums";
import type { LearningTrackResponse } from "@/types/learning";

const schema = z.object({
  name: z.string().min(1, "İsim gerekli.").max(200),
  description: z.string().max(2000).nullable().optional(),
  source: z.string().max(200).nullable().optional(),
  status: z.enum(["Active", "Paused", "Completed", "Abandoned"]),
});
type Values = z.infer<typeof schema>;

export function LearningTrackFormModal({
  open,
  onOpenChange,
  initial,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  initial?: LearningTrackResponse;
}) {
  const qc = useQueryClient();
  const router = useRouter();
  const isEdit = !!initial;

  const { register, handleSubmit, reset, control, formState } = useForm<Values>({
    resolver: zodResolver(schema),
    defaultValues: {
      name: initial?.name ?? "",
      description: initial?.description ?? "",
      source: initial?.source ?? "",
      status: initial?.status ?? "Active",
    },
  });

  useEffect(() => {
    if (open) {
      reset({
        name: initial?.name ?? "",
        description: initial?.description ?? "",
        source: initial?.source ?? "",
        status: initial?.status ?? "Active",
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, initial?.id]);

  const create = useMutation({
    mutationFn: (v: Values) => learningTracksApi.create(v),
    onSuccess: (t) => {
      toast.success("Eğitim oluşturuldu.");
      qc.invalidateQueries({ queryKey: ["learning-tracks"] });
      onOpenChange(false);
      router.push(`/learning/${t.id}`);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const update = useMutation({
    mutationFn: (v: Values) => learningTracksApi.update(initial!.id, { name: v.name, description: v.description, source: v.source }),
    onSuccess: () => {
      toast.success("Eğitim güncellendi.");
      qc.invalidateQueries({ queryKey: ["learning-tracks"] });
      qc.invalidateQueries({ queryKey: ["learning-track", initial!.id] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  const submitting = create.isPending || update.isPending;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle>{isEdit ? "Eğitimi düzenle" : "Yeni eğitim"}</DialogTitle>
          <DialogDescription>
            {isEdit ? "Bilgileri güncelle." : "Yeni bir eğitim takip et."}
          </DialogDescription>
        </DialogHeader>
        <form
          onSubmit={handleSubmit((v) => (isEdit ? update.mutate(v) : create.mutate(v)))}
          className="space-y-4"
        >
          <Field label="İsim" required error={formState.errors.name?.message}>
            <Input autoFocus {...register("name")} />
          </Field>
          <Field label="Kaynak" error={formState.errors.source?.message}>
            <Input placeholder="Claude, Coursera, kitap…" {...register("source")} />
          </Field>
          <Field label="Açıklama" error={formState.errors.description?.message}>
            <Textarea rows={3} {...register("description")} />
          </Field>
          {!isEdit && (
            <Field label="Durum">
              <Controller
                control={control}
                name="status"
                render={({ field }) => (
                  <Select value={field.value} onValueChange={field.onChange}>
                    <SelectTrigger className="w-full">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      {LEARNING_TRACK_STATUSES.map((s) => (
                        <SelectItem key={s} value={s}>{PROJECT_STATUS_LABELS[s]}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                )}
              />
            </Field>
          )}
          <DialogFooter>
            <Button type="button" variant="ghost" onClick={() => onOpenChange(false)}>
              Vazgeç
            </Button>
            <Button type="submit" disabled={submitting}>
              {submitting ? "Kaydediliyor…" : isEdit ? "Güncelle" : "Oluştur"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
