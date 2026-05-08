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
import { projectsApi } from "@/lib/api/projects";
import { errorMessage } from "@/lib/error";
import { PROJECT_STATUSES, PROJECT_STATUS_LABELS } from "@/types/enums";
import type { ProjectResponse } from "@/types/project";

const schema = z.object({
  name: z.string().min(1, "İsim gerekli.").max(200),
  description: z.string().max(2000).nullable().optional(),
  goal: z.string().max(1000).nullable().optional(),
  repoUrl: z.string().max(500).nullable().optional(),
  status: z.enum(["Active", "Paused", "Completed", "Abandoned"]),
});
type Values = z.infer<typeof schema>;

export function ProjectFormModal({
  open,
  onOpenChange,
  initial,
  onCreated,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  initial?: ProjectResponse;
  onCreated?: (project: ProjectResponse) => void;
}) {
  const qc = useQueryClient();
  const router = useRouter();
  const isEdit = !!initial;

  const { register, handleSubmit, reset, control, formState } = useForm<Values>({
    resolver: zodResolver(schema),
    defaultValues: {
      name: initial?.name ?? "",
      description: initial?.description ?? "",
      goal: initial?.goal ?? "",
      repoUrl: initial?.repoUrl ?? "",
      status: initial?.status ?? "Active",
    },
  });

  useEffect(() => {
    if (open) {
      reset({
        name: initial?.name ?? "",
        description: initial?.description ?? "",
        goal: initial?.goal ?? "",
        repoUrl: initial?.repoUrl ?? "",
        status: initial?.status ?? "Active",
      });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open, initial?.id]);

  const create = useMutation({
    mutationFn: (v: Values) => projectsApi.create(v),
    onSuccess: (p) => {
      toast.success("Proje oluşturuldu.");
      qc.invalidateQueries({ queryKey: ["projects"] });
      onOpenChange(false);
      if (onCreated) onCreated(p);
      else router.push(`/projects/${p.id}`);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const update = useMutation({
    mutationFn: (v: Values) => projectsApi.update(initial!.id, { name: v.name, description: v.description, goal: v.goal, repoUrl: v.repoUrl }),
    onSuccess: () => {
      toast.success("Proje güncellendi.");
      qc.invalidateQueries({ queryKey: ["projects"] });
      qc.invalidateQueries({ queryKey: ["project", initial!.id] });
      onOpenChange(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  const submitting = create.isPending || update.isPending;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle>{isEdit ? "Projeyi düzenle" : "Yeni proje"}</DialogTitle>
          <DialogDescription>
            {isEdit ? "Bilgileri güncelle." : "Üzerinde çalışacağın yeni bir proje aç."}
          </DialogDescription>
        </DialogHeader>
        <form
          onSubmit={handleSubmit((v) => (isEdit ? update.mutate(v) : create.mutate(v)))}
          className="space-y-4"
        >
          <Field label="İsim" required error={formState.errors.name?.message}>
            <Input autoFocus {...register("name")} />
          </Field>
          <Field label="Amaç" error={formState.errors.goal?.message}>
            <Input
              placeholder="Bu projeyle ne çözmek istiyorsun?"
              {...register("goal")}
            />
          </Field>
          <Field label="Açıklama" error={formState.errors.description?.message}>
            <Textarea rows={3} {...register("description")} />
          </Field>
          <Field label="Repo URL" error={formState.errors.repoUrl?.message}>
            <Input placeholder="https://github.com/…" {...register("repoUrl")} />
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
                      {PROJECT_STATUSES.map((s) => (
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
