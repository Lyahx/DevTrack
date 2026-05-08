"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Database, Eraser, Keyboard, LogOut, Moon, Sun } from "lucide-react";
import { useTheme } from "next-themes";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { PageHeader } from "@/components/common/PageHeader";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { devApi } from "@/lib/api/dev";
import { errorMessage } from "@/lib/error";
import { useAuthStore } from "@/store/auth";

export function SettingsClient() {
  const { theme, setTheme } = useTheme();
  const [mounted, setMounted] = useState(false);
  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setMounted(true);
  }, []);
  const user = useAuthStore((s) => s.user);
  const logout = useAuthStore((s) => s.logout);
  const router = useRouter();
  const qc = useQueryClient();

  const seed = useMutation({
    mutationFn: () => devApi.seed(),
    onSuccess: (r) => {
      toast.success(
        `Mock data hazır — ${r.projects} proje, ${r.components} bileşen, ${r.learningTracks} eğitim, ${r.worklogs} worklog, ${r.nextSteps} adım, ${r.ideas} fikir, ${r.resources} kaynak, ${r.reminders} hatırlatma.`,
        { duration: 6000 },
      );
      qc.invalidateQueries();
    },
    onError: (e) => toast.error(errorMessage(e)),
  });
  const wipe = useMutation({
    mutationFn: () => devApi.wipe(),
    onSuccess: (r) => {
      toast.success(`${r.rowsDeleted} satır silindi.`);
      qc.invalidateQueries();
    },
    onError: (e) => toast.error(errorMessage(e)),
  });

  return (
    <div className="space-y-6">
      <PageHeader title="Ayarlar" description="Hesap ve görünüm." />

      <Card>
        <CardHeader><CardTitle className="text-base">Hesap</CardTitle></CardHeader>
        <CardContent className="space-y-2 text-sm">
          <p><span className="text-muted-foreground">Kullanıcı adı:</span> {user?.username}</p>
          <p><span className="text-muted-foreground">E-posta:</span> {user?.email}</p>
          <Button
            variant="outline"
            className="mt-2"
            onClick={() => {
              logout();
              router.replace("/login");
            }}
          >
            <LogOut className="h-4 w-4" /> Çıkış yap
          </Button>
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle className="text-base">Görünüm</CardTitle></CardHeader>
        <CardContent className="space-y-3">
          {mounted ? (
            <Button
              variant="outline"
              onClick={() => setTheme(theme === "dark" ? "light" : "dark")}
            >
              {theme === "dark" ? <Sun className="h-4 w-4" /> : <Moon className="h-4 w-4" />}
              {theme === "dark" ? "Aydınlık tema" : "Karanlık tema"}
            </Button>
          ) : null}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-base">
            <Keyboard className="h-4 w-4" /> Kısayollar
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-2 text-sm">
          <div className="flex items-center justify-between">
            <span>Komut paleti</span>
            <kbd className="rounded bg-muted px-2 py-0.5 font-mono text-xs">Ctrl/⌘ + K</kbd>
          </div>
          <div className="flex items-center justify-between">
            <span>Hızlı yakala</span>
            <kbd className="rounded bg-muted px-2 py-0.5 font-mono text-xs">Ctrl/⌘ + J</kbd>
          </div>
          <div className="flex items-center justify-between">
            <span>Hızlı yakala — gönder</span>
            <kbd className="rounded bg-muted px-2 py-0.5 font-mono text-xs">Ctrl/⌘ + Enter</kbd>
          </div>
        </CardContent>
      </Card>

      <Card className="border-amber-300/40 bg-amber-50/30 dark:bg-amber-950/20">
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-base text-amber-900 dark:text-amber-300">
            <Database className="h-4 w-4" /> Geliştirici araçları
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-3">
          <p className="text-sm text-muted-foreground">
            Test için örnek veri yükle. Mevcut tüm verilerin önce silinir, sonra örnek dataset basılır.
          </p>
          <div className="flex flex-wrap gap-2">
            <Button
              onClick={() => {
                if (confirm("Mevcut tüm verilerin silinecek. Devam edelim mi?")) seed.mutate();
              }}
              disabled={seed.isPending || wipe.isPending}
            >
              <Database className="h-4 w-4" /> {seed.isPending ? "Yükleniyor…" : "Mock data yükle"}
            </Button>
            <Button
              variant="outline"
              onClick={() => {
                if (confirm("Tüm projelerin, eğitimlerin, worklog'ların hepsi silinecek. Emin misin?")) wipe.mutate();
              }}
              disabled={seed.isPending || wipe.isPending}
            >
              <Eraser className="h-4 w-4" /> {wipe.isPending ? "Siliniyor…" : "Tümünü sil"}
            </Button>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle className="text-base">DevTrack hakkında</CardTitle></CardHeader>
        <CardContent className="space-y-1 text-sm text-muted-foreground">
          <p>Kişisel proje ve öğrenme takipçisi.</p>
          <p>Front: Next.js 16 · Tailwind v4 · shadcn/ui · TanStack Query</p>
          <p>Back: .NET 10 · EF Core · MSSQL</p>
        </CardContent>
      </Card>
    </div>
  );
}
