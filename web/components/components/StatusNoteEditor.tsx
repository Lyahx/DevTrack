"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Pencil } from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { componentsApi } from "@/lib/api/components";
import { errorMessage } from "@/lib/error";

export function StatusNoteEditor({
  componentId,
  value,
}: {
  componentId: number;
  value: string | null;
}) {
  const [editing, setEditing] = useState(false);
  const [text, setText] = useState(value ?? "");
  const taRef = useRef<HTMLTextAreaElement>(null);
  const qc = useQueryClient();

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setText(value ?? "");
  }, [value]);

  const save = useMutation({
    mutationFn: (v: string) => componentsApi.setStatusNote(componentId, { currentStatusNote: v || null }),
    onSuccess: () => {
      toast.success("Not kaydedildi.");
      qc.invalidateQueries({ queryKey: ["project"] });
      qc.invalidateQueries({ queryKey: ["component", componentId] });
      qc.invalidateQueries({ queryKey: ["resume"] });
      setEditing(false);
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  if (!editing) {
    return (
      <div className="group rounded-md border bg-muted/30 p-3 text-sm">
        <div className="mb-1.5 flex items-center justify-between">
          <span className="text-xs font-medium uppercase tracking-wider text-muted-foreground">📍 Şu an nerede kaldım</span>
          <Button
            size="icon-xs"
            variant="ghost"
            onClick={() => {
              setEditing(true);
              setTimeout(() => taRef.current?.focus(), 50);
            }}
            aria-label="Düzenle"
          >
            <Pencil className="h-3 w-3" />
          </Button>
        </div>
        {value ? (
          <p className="whitespace-pre-wrap text-foreground">{value}</p>
        ) : (
          <p className="text-muted-foreground italic">Henüz not eklenmemiş — bir sonraki defaya bir bilgi bırak.</p>
        )}
      </div>
    );
  }

  return (
    <div className="space-y-2 rounded-md border bg-muted/30 p-3">
      <span className="text-xs font-medium uppercase tracking-wider text-muted-foreground">📍 Şu an nerede kaldım</span>
      <Textarea
        ref={taRef}
        rows={4}
        value={text}
        onChange={(e) => setText(e.target.value)}
      />
      <div className="flex justify-end gap-2">
        <Button
          size="sm"
          variant="ghost"
          onClick={() => {
            setText(value ?? "");
            setEditing(false);
          }}
        >
          Vazgeç
        </Button>
        <Button size="sm" onClick={() => save.mutate(text)} disabled={save.isPending}>
          {save.isPending ? "Kaydediliyor…" : "Kaydet"}
        </Button>
      </div>
    </div>
  );
}
