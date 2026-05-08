"use client";

import { useQuery } from "@tanstack/react-query";
import { BookOpen, FolderKanban, LayoutDashboard, LogOut, Moon, Sun, Tags, Trash2, Bell, Plus } from "lucide-react";
import { useTheme } from "next-themes";
import { useRouter } from "next/navigation";
import { CommandDialog, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList, CommandSeparator } from "@/components/ui/command";
import { learningTracksApi } from "@/lib/api/learning";
import { projectsApi } from "@/lib/api/projects";
import { useAuthStore } from "@/store/auth";
import { useCommandPaletteStore } from "@/store/commandPalette";
import { useQuickCaptureStore } from "@/store/quickCapture";

export function CommandPalette() {
  const open = useCommandPaletteStore((s) => s.open);
  const setOpen = useCommandPaletteStore((s) => s.setOpen);
  const openCapture = useQuickCaptureStore((s) => s.openCapture);
  const router = useRouter();
  const { theme, setTheme } = useTheme();
  const logout = useAuthStore((s) => s.logout);

  const projects = useQuery({
    queryKey: ["projects", "command"],
    queryFn: () => projectsApi.list({ pageSize: 100 }),
    enabled: open,
  });
  const tracks = useQuery({
    queryKey: ["learning-tracks", "command"],
    queryFn: () => learningTracksApi.list({ pageSize: 100 }),
    enabled: open,
  });

  function go(path: string) {
    setOpen(false);
    router.push(path);
  }

  return (
    <CommandDialog open={open} onOpenChange={setOpen} title="Komutlar" description="Hızlıca git veya bir eylem çalıştır.">
      <CommandInput placeholder="Bir komut yaz veya ara…" />
      <CommandList>
        <CommandEmpty>Sonuç yok.</CommandEmpty>
        <CommandGroup heading="Git">
          <CommandItem onSelect={() => go("/")}>
            <LayoutDashboard className="mr-2 h-4 w-4" /> Pano
          </CommandItem>
          <CommandItem onSelect={() => go("/projects")}>
            <FolderKanban className="mr-2 h-4 w-4" /> Projeler
          </CommandItem>
          <CommandItem onSelect={() => go("/learning")}>
            <BookOpen className="mr-2 h-4 w-4" /> Eğitim
          </CommandItem>
          <CommandItem onSelect={() => go("/reminders")}>
            <Bell className="mr-2 h-4 w-4" /> Hatırlatmalar
          </CommandItem>
          <CommandItem onSelect={() => go("/tags")}>
            <Tags className="mr-2 h-4 w-4" /> Etiketler
          </CommandItem>
          <CommandItem onSelect={() => go("/trash")}>
            <Trash2 className="mr-2 h-4 w-4" /> Çöp
          </CommandItem>
        </CommandGroup>

        {projects.data?.items.length ? (
          <>
            <CommandSeparator />
            <CommandGroup heading="Projeye git">
              {projects.data.items.map((p) => (
                <CommandItem key={p.id} value={`Proje ${p.name}`} onSelect={() => go(`/projects/${p.id}`)}>
                  <FolderKanban className="mr-2 h-4 w-4" /> {p.name}
                </CommandItem>
              ))}
            </CommandGroup>
          </>
        ) : null}

        {tracks.data?.items.length ? (
          <>
            <CommandSeparator />
            <CommandGroup heading="Eğitime git">
              {tracks.data.items.map((t) => (
                <CommandItem key={t.id} value={`Eğitim ${t.name}`} onSelect={() => go(`/learning/${t.id}`)}>
                  <BookOpen className="mr-2 h-4 w-4" /> {t.name}
                </CommandItem>
              ))}
            </CommandGroup>
          </>
        ) : null}

        <CommandSeparator />
        <CommandGroup heading="Eylemler">
          <CommandItem
            onSelect={() => {
              setOpen(false);
              openCapture();
            }}
          >
            <Plus className="mr-2 h-4 w-4" /> Hızlı yakalayı aç (⌘J)
          </CommandItem>
          <CommandItem
            onSelect={() => {
              setOpen(false);
              setTheme(theme === "dark" ? "light" : "dark");
            }}
          >
            {theme === "dark" ? <Sun className="mr-2 h-4 w-4" /> : <Moon className="mr-2 h-4 w-4" />}
            Temayı değiştir
          </CommandItem>
          <CommandItem
            onSelect={() => {
              setOpen(false);
              logout();
              router.replace("/login");
            }}
          >
            <LogOut className="mr-2 h-4 w-4" /> Çıkış yap
          </CommandItem>
        </CommandGroup>
      </CommandList>
    </CommandDialog>
  );
}
