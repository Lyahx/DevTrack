"use client";

import { useQuery } from "@tanstack/react-query";
import { Check, ChevronsUpDown } from "lucide-react";
import { useMemo, useState } from "react";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { buttonVariants } from "@/components/ui/button";
import { componentsApi } from "@/lib/api/components";
import { learningModulesApi, learningTracksApi } from "@/lib/api/learning";
import { projectsApi } from "@/lib/api/projects";
import { OWNER_TYPE_LABELS } from "@/types/enums";
import type { OwnerReference } from "@/types/owner";
import { cn } from "@/lib/utils";

type Option = OwnerReference & { label: string };

export function OwnerPicker({
  value,
  onChange,
  placeholder = "Sahip seç",
}: {
  value: OwnerReference | null;
  onChange: (value: OwnerReference) => void;
  placeholder?: string;
}) {
  const [open, setOpen] = useState(false);

  const projects = useQuery({
    queryKey: ["projects", "all"],
    queryFn: () => projectsApi.list({ pageSize: 200 }),
  });
  const tracks = useQuery({
    queryKey: ["learning-tracks", "all"],
    queryFn: () => learningTracksApi.list({ pageSize: 200 }),
  });
  const components = useQuery({
    queryKey: ["components", "for-owner-picker", projects.data?.items.map((p) => p.id).join(",")],
    enabled: !!projects.data,
    queryFn: async () => {
      const all = await Promise.all(
        (projects.data?.items ?? []).map(async (p) => {
          try {
            const list = await componentsApi.listForProject(p.id);
            return list.map((c) => ({ ...c, projectName: p.name }));
          } catch {
            return [];
          }
        }),
      );
      return all.flat();
    },
  });
  const modules = useQuery({
    queryKey: ["modules", "for-owner-picker", tracks.data?.items.map((t) => t.id).join(",")],
    enabled: !!tracks.data,
    queryFn: async () => {
      const all = await Promise.all(
        (tracks.data?.items ?? []).map(async (t) => {
          try {
            const list = await learningModulesApi.listForTrack(t.id);
            return list.map((m) => ({ ...m, trackName: t.name }));
          } catch {
            return [];
          }
        }),
      );
      return all.flat();
    },
  });

  const options = useMemo<Option[]>(() => {
    const out: Option[] = [];
    projects.data?.items.forEach((p) => out.push({ type: "Project", id: p.id, label: p.name }));
    components.data?.forEach((c) => out.push({ type: "Component", id: c.id, label: `${c.projectName} / ${c.name}` }));
    tracks.data?.items.forEach((t) => out.push({ type: "LearningTrack", id: t.id, label: t.name }));
    modules.data?.forEach((m) => out.push({ type: "LearningModule", id: m.id, label: `${m.trackName} / ${m.name}` }));
    return out;
  }, [projects.data, components.data, tracks.data, modules.data]);

  const selected = options.find((o) => value && o.type === value.type && o.id === value.id);

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger
        type="button"
        className={cn(buttonVariants({ variant: "outline" }), "w-full justify-between font-normal")}
      >
        {selected ? (
          <span className="truncate">
            <span className="text-muted-foreground">[{OWNER_TYPE_LABELS[selected.type]}]</span>{" "}
            {selected.label}
          </span>
        ) : (
          <span className="text-muted-foreground">{placeholder}</span>
        )}
        <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
      </PopoverTrigger>
      <PopoverContent className="w-(--radix-popover-trigger-width,--anchor-width) p-0" align="start">
        <Command>
          <CommandInput placeholder="Ara…" />
          <CommandList>
            <CommandEmpty>Sonuç yok.</CommandEmpty>
            <CommandGroup heading="Projeler">
              {options.filter((o) => o.type === "Project").map((o) => (
                <CommandItem
                  key={`p-${o.id}`}
                  value={`Proje ${o.label}`}
                  onSelect={() => {
                    onChange({ type: o.type, id: o.id });
                    setOpen(false);
                  }}
                >
                  <Check className={cn("mr-2 h-4 w-4", selected?.type === o.type && selected.id === o.id ? "opacity-100" : "opacity-0")} />
                  {o.label}
                </CommandItem>
              ))}
            </CommandGroup>
            <CommandGroup heading="Bileşenler">
              {options.filter((o) => o.type === "Component").map((o) => (
                <CommandItem
                  key={`c-${o.id}`}
                  value={`Bileşen ${o.label}`}
                  onSelect={() => {
                    onChange({ type: o.type, id: o.id });
                    setOpen(false);
                  }}
                >
                  <Check className={cn("mr-2 h-4 w-4", selected?.type === o.type && selected.id === o.id ? "opacity-100" : "opacity-0")} />
                  {o.label}
                </CommandItem>
              ))}
            </CommandGroup>
            <CommandGroup heading="Eğitimler">
              {options.filter((o) => o.type === "LearningTrack").map((o) => (
                <CommandItem
                  key={`t-${o.id}`}
                  value={`Eğitim ${o.label}`}
                  onSelect={() => {
                    onChange({ type: o.type, id: o.id });
                    setOpen(false);
                  }}
                >
                  <Check className={cn("mr-2 h-4 w-4", selected?.type === o.type && selected.id === o.id ? "opacity-100" : "opacity-0")} />
                  {o.label}
                </CommandItem>
              ))}
            </CommandGroup>
            <CommandGroup heading="Modüller">
              {options.filter((o) => o.type === "LearningModule").map((o) => (
                <CommandItem
                  key={`m-${o.id}`}
                  value={`Modül ${o.label}`}
                  onSelect={() => {
                    onChange({ type: o.type, id: o.id });
                    setOpen(false);
                  }}
                >
                  <Check className={cn("mr-2 h-4 w-4", selected?.type === o.type && selected.id === o.id ? "opacity-100" : "opacity-0")} />
                  {o.label}
                </CommandItem>
              ))}
            </CommandGroup>
          </CommandList>
        </Command>
      </PopoverContent>
    </Popover>
  );
}
