"use client";

import { ArrowRight, BookOpen, CheckSquare, Compass, FolderKanban, Lightbulb, Link2, MapPin, Monitor, Moon, Sparkles, Sun, Zap } from "lucide-react";
import Link from "next/link";
import { useTheme } from "next-themes";
import { GuestGuard } from "@/components/auth/AuthGuard";
import { buttonVariants } from "@/components/ui/button";
import { cn } from "@/lib/utils";

function MiniThemeToggle() {
  const { theme, setTheme } = useTheme();
  const current = theme ?? "system";
  const next: Record<string, string> = { light: "dark", dark: "system", system: "light" };
  const Icon = current === "light" ? Sun : current === "dark" ? Moon : Monitor;
  return (
    <button
      type="button"
      onClick={() => setTheme(next[current])}
      aria-label="Temayı değiştir"
      className="flex h-8 w-8 items-center justify-center rounded-md text-text-muted transition-colors hover:bg-surface-2 hover:text-text"
    >
      <Icon className="h-4 w-4" />
    </button>
  );
}

/* ── Mock UI components — pure CSS previews ────────────────────────── */

function ResumeMockCard() {
  return (
    <div className="overflow-hidden rounded-xl border border-border bg-surface-1 shadow-card transition-transform duration-300 hover:-translate-y-1">
      <div className="flex items-center gap-1.5 border-b border-border-subtle bg-surface-2 px-3 py-2">
        <div className="h-2 w-2 rounded-full bg-warning/60" />
        <div className="h-2 w-2 rounded-full bg-info/60" />
        <div className="h-2 w-2 rounded-full bg-success/60" />
        <div className="ml-auto font-mono text-[10px] text-text-faint">/projects/aos/resume</div>
      </div>
      <div className="space-y-3 p-4">
        <div className="space-y-0.5">
          <div className="flex items-center gap-2">
            <div className="text-[15px] font-medium text-text">AOS</div>
            <span className="inline-flex items-center gap-1 rounded bg-success-soft px-1.5 py-0.5 text-[9px] font-medium text-success">
              <span className="h-1.5 w-1.5 rounded-full bg-success" /> Aktif
            </span>
          </div>
          <div className="font-mono text-[10px] text-text-faint">2 saat önce · 3 worklog · 2 açık adım</div>
        </div>
        <div className="relative overflow-hidden rounded-md border border-border bg-surface-2 p-3 pl-4">
          <span className="absolute left-0 top-0 bottom-0 w-1 bg-primary" />
          <div className="mb-1.5 flex items-center gap-1.5 text-[9px] font-semibold uppercase tracking-wider text-text-faint">
            <MapPin className="h-2.5 w-2.5" /> Şu an nerede kaldım
          </div>
          <p className="text-[11px] leading-relaxed text-text-secondary">
            Refresh token endpoint'ini eklemeye başladım. Validator yarıda kaldı — <span className="font-mono text-[10px]">RefreshTokenRequest</span> için MaxLength sınırı net değil. Yarın access token rotation'ı tamamla.
          </p>
        </div>
        <div className="space-y-1.5">
          <div className="flex items-center gap-1.5 text-[9px] font-semibold uppercase tracking-wider text-text-faint">
            <CheckSquare className="h-2.5 w-2.5" /> Açık adımlar
          </div>
          {[
            { t: "Refresh token endpoint'ini bitir", p: "Yüksek", c: "bg-warning text-white" },
            { t: "Ders kayıt: çakışan saat uyarısı UI", p: "Orta", c: "bg-info-soft text-info" },
          ].map((s, i) => (
            <div key={i} className="flex items-center gap-2 rounded-md border border-border-subtle bg-surface-1 p-1.5">
              <div className="h-3 w-3 shrink-0 rounded border border-border-strong" />
              <div className="flex-1 truncate text-[10.5px] text-text-secondary">{s.t}</div>
              <span className={`ml-auto shrink-0 rounded px-1 py-0.5 text-[8px] font-medium ${s.c}`}>{s.p}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

function QuickCaptureMock() {
  return (
    <div className="overflow-hidden rounded-xl border border-border bg-surface-2 shadow-card transition-transform duration-300 hover:-translate-y-1">
      <div className="space-y-3 p-4">
        <div className="space-y-0.5">
          <p className="text-[12px] leading-snug text-text">
            Outbox pattern'ı bittikten sonra DevTrack'e uygula — read model olarak elasticsearch dene
          </p>
          <p className="font-mono text-[10px] text-text-faint">cursor</p>
        </div>
        <div className="flex flex-wrap items-center gap-1">
          {[
            { l: "Fikir", a: false },
            { l: "Adım", a: false },
            { l: "Worklog", a: true },
          ].map((t) => (
            <span
              key={t.l}
              className={cn(
                "rounded px-1.5 py-0.5 text-[9px] font-medium",
                t.a ? "bg-surface-3 text-text" : "text-text-muted",
              )}
            >
              {t.l}
            </span>
          ))}
          <span className="ml-auto rounded bg-surface-1 px-1.5 py-0.5 text-[9px] font-medium text-text-secondary">
            Sistem Tasarımı
          </span>
        </div>
        <div className="flex items-center justify-between border-t border-border-subtle pt-3">
          <span className="font-mono text-[9px] text-text-faint">⌘↵ kaydet · ESC kapat</span>
          <div className="rounded-md bg-primary px-2.5 py-1 text-[10px] font-medium text-primary-foreground">Yakala</div>
        </div>
      </div>
    </div>
  );
}

function SidebarMock() {
  const items = [
    { label: "AOS", time: "2h", color: "bg-success" },
    { label: "FoodTracker", time: "3d", color: "bg-success" },
    { label: "DevTrack", time: "now", color: "bg-success" },
    { label: "Pixel Roguelike", time: "45d", color: "bg-text-muted" },
  ];
  return (
    <div className="overflow-hidden rounded-xl border border-border bg-surface-1 shadow-card transition-transform duration-300 hover:-translate-y-1">
      <div className="space-y-3 p-4">
        <div className="text-[9px] font-semibold uppercase tracking-wider text-text-faint">Projeler</div>
        <div className="space-y-1">
          {items.map((it, i) => (
            <div
              key={it.label}
              className={cn(
                "relative flex items-center gap-2 rounded px-2 py-1 text-[11px]",
                i === 0 ? "bg-surface-2 text-text" : "text-text-muted",
              )}
            >
              {i === 0 ? (
                <span className="absolute left-0 top-1/2 h-3 w-0.5 -translate-y-1/2 rounded-r bg-primary" />
              ) : null}
              <span className={`h-1.5 w-1.5 rounded-full ${it.color}`} />
              <span className="flex-1 truncate">{it.label}</span>
              <span className="font-mono text-[9px] text-text-faint">{it.time}</span>
            </div>
          ))}
        </div>
        <div className="text-[9px] font-semibold uppercase tracking-wider text-text-faint">Eğitim</div>
        <div className="space-y-1">
          {[
            { label: "Sistem Tasarımı", time: "5d" },
            { label: "React 19", time: "22d" },
          ].map((it) => (
            <div key={it.label} className="flex items-center gap-2 rounded px-2 py-1 text-[11px] text-text-muted">
              <span className="h-1.5 w-1.5 rounded-full bg-success" />
              <span className="flex-1 truncate">{it.label}</span>
              <span className="font-mono text-[9px] text-text-faint">{it.time}</span>
            </div>
          ))}
        </div>
        <div className="border-t border-border-subtle pt-2">
          <div className="flex items-center gap-2 rounded px-2 py-1 text-[11px] text-text-muted">
            <Zap className="h-3 w-3 text-primary" />
            <span className="flex-1">Hızlı yakala</span>
            <span className="font-mono text-[9px] text-text-faint">⌘J</span>
          </div>
        </div>
      </div>
    </div>
  );
}

function StatBlock({ kicker, big, sub }: { kicker: string; big: string; sub: string }) {
  return (
    <section className="relative z-10 mx-auto max-w-3xl px-6 py-16 text-center sm:py-20">
      <div className="font-mono text-[11px] uppercase tracking-[0.2em] text-primary">{kicker}</div>
      <div className="mt-3 text-[40px] font-medium leading-[1.05] tracking-tight text-text sm:text-[56px]">
        {big}
      </div>
      <div className="mx-auto mt-4 max-w-md text-[14px] text-text-muted">{sub}</div>
    </section>
  );
}

/* ── Page ──────────────────────────────────────────────────────────── */

export default function WelcomePage() {
  return (
    <GuestGuard>
      <div className="relative min-h-screen overflow-hidden bg-canvas">
        {/* Decorative orbs */}
        <div
          aria-hidden
          className="pointer-events-none absolute -top-40 left-1/2 h-[500px] w-[800px] -translate-x-1/2 rounded-full opacity-30 blur-[120px]"
          style={{ background: "radial-gradient(circle, var(--color-violet) 0%, transparent 60%)" }}
        />
        <div
          aria-hidden
          className="pointer-events-none absolute right-[-200px] top-[400px] h-[400px] w-[400px] rounded-full opacity-20 blur-[100px]"
          style={{ background: "radial-gradient(circle, var(--color-info) 0%, transparent 60%)" }}
        />
        <div
          aria-hidden
          className="pointer-events-none absolute left-[-150px] top-[800px] h-[400px] w-[400px] rounded-full opacity-20 blur-[100px]"
          style={{ background: "radial-gradient(circle, var(--color-success) 0%, transparent 60%)" }}
        />

        <header className="relative z-10 mx-auto flex max-w-6xl items-center justify-between px-6 py-5">
          <Link href="/welcome" className="flex items-center gap-2 text-[14px] font-medium text-text">
            <span className="inline-block h-6 w-6 rounded-md bg-primary text-center text-[12px] font-semibold leading-6 text-primary-foreground">D</span>
            DevTrack
          </Link>
          <div className="flex items-center gap-3">
            <Link href="/login" className="text-[13px] text-text-secondary hover:text-text">Giriş yap</Link>
            <MiniThemeToggle />
          </div>
        </header>

        {/* HERO */}
        <section className="relative z-10 mx-auto max-w-4xl px-6 pt-16 pb-12 text-center sm:pt-24 sm:pb-20">
          <div className="mb-8 inline-flex items-center gap-2 rounded-full border border-border bg-surface-1/80 px-3.5 py-1.5 text-[12px] text-text-secondary backdrop-blur-sm">
            <Sparkles className="h-3 w-3 text-primary" />
            Bir kez başla, kaybetme.
          </div>
          <h1 className="text-[44px] font-medium leading-[1.02] tracking-tight text-text sm:text-[68px] md:text-[80px]">
            Başladığın <span className="italic text-text-secondary">her şey</span>
            <br />
            <span className="relative inline-block">
              tek yerde.
              <span aria-hidden className="absolute -bottom-2 left-0 right-0 h-2 -skew-x-6 rounded-sm bg-primary/30" />
            </span>
          </h1>
          <p className="mx-auto mt-8 max-w-xl text-[16px] leading-relaxed text-text-secondary sm:text-[18px]">
            Paralel projeler, parça parça eğitimler, ortada kalan fikirler. DevTrack hepsini birlikte tutar — bir hafta sonra geri döndüğünde nereye geldiğini görürsün.
          </p>
          <div className="mt-10 flex flex-wrap items-center justify-center gap-3">
            <Link href="/register" className={cn(buttonVariants({ variant: "default", size: "lg" }), "h-11 px-5 text-[14px]")}>
              Ücretsiz başla <ArrowRight className="h-4 w-4" />
            </Link>
            <Link href="/login" className={cn(buttonVariants({ variant: "secondary", size: "lg" }), "h-11 px-5 text-[14px]")}>
              Giriş yap
            </Link>
          </div>
          <p className="mt-4 text-[12px] text-text-faint">
            Kişisel kullanım için · 30 saniyede hazır
          </p>
        </section>

        {/* HERO PREVIEW ROW */}
        <section className="relative z-10 mx-auto mb-24 max-w-5xl px-6">
          <div className="relative">
            <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
              <div className="md:translate-y-6"><SidebarMock /></div>
              <div><ResumeMockCard /></div>
              <div className="md:translate-y-10"><QuickCaptureMock /></div>
            </div>
            {/* Soft fade-out at bottom */}
            <div
              aria-hidden
              className="pointer-events-none absolute -inset-x-12 bottom-[-50px] h-24"
              style={{ background: "linear-gradient(to bottom, transparent, var(--color-canvas))" }}
            />
          </div>
        </section>

        {/* STAT 1 — bridges hero preview to first feature */}
        <StatBlock
          kicker="Resume Mode"
          big="5 saniye."
          sub="Bir hafta sonra geri döndüğünde son kaldığın yeri bulmak için yeterli."
        />

        {/* FEATURE 1 — Resume Mode */}
        <section className="relative z-10 mx-auto max-w-6xl px-6 pb-20">
          <div className="overflow-hidden rounded-2xl border border-border bg-surface-1 p-8 shadow-card sm:p-12">
            <div className="grid grid-cols-1 items-center gap-10 md:grid-cols-2">
              <div>
                <div className="mb-4 inline-flex items-center gap-2 rounded-full bg-primary/10 px-3 py-1 text-[11px] font-medium text-primary">
                  <Compass className="h-3 w-3" /> Resume Mode
                </div>
                <h2 className="text-[28px] font-medium leading-tight tracking-tight text-text sm:text-[36px]">
                  Bir hafta sonra geri dön. <br />
                  <span className="text-text-secondary">5 saniyede yerini bul.</span>
                </h2>
                <p className="mt-4 text-[14px] leading-relaxed text-text-muted">
                  Son worklog'ların, açık adımların, "şu an nerede kaldım" notların — hepsi tek ekranda. "Devam et" butonuna basıp kaldığın yerden başla.
                </p>
                <ul className="mt-6 space-y-2.5 text-[13px] text-text-secondary">
                  {["Bileşene göre durum notları", "Açık adımlar + öncelik", "Son kararlar + alternatifler"].map((t) => (
                    <li key={t} className="flex items-start gap-2">
                      <div className="mt-0.5 flex h-4 w-4 items-center justify-center rounded-full bg-primary/15 text-primary">
                        <CheckSquare className="h-2.5 w-2.5" />
                      </div>
                      {t}
                    </li>
                  ))}
                </ul>
              </div>
              <div className="md:pl-4">
                <ResumeMockCard />
              </div>
            </div>
          </div>
        </section>

        {/* STAT 2 */}
        <StatBlock
          kicker="Hızlı yakala"
          big="⌘J · ⌘↵"
          sub="İki kısayol, akış kesilmesin. Yaz, türünü seç, gönder."
        />

        {/* FEATURE 2 — Hızlı yakala */}
        <section className="relative z-10 mx-auto max-w-6xl px-6 pb-20">
          <div className="overflow-hidden rounded-2xl border border-border bg-gradient-to-br from-success-soft via-surface-1 to-surface-1 p-8 shadow-card sm:p-12">
            <div className="grid grid-cols-1 items-center gap-10 md:grid-cols-2">
              <div className="md:order-2">
                <div className="mb-4 inline-flex items-center gap-2 rounded-full bg-success-soft px-3 py-1 text-[11px] font-medium text-success">
                  <Zap className="h-3 w-3" /> ⌘J ile yakala
                </div>
                <h2 className="text-[28px] font-medium leading-tight tracking-tight text-text sm:text-[36px]">
                  Aklındakini kaybetme. <br />
                  <span className="text-text-secondary">Yaz, gönder, devam et.</span>
                </h2>
                <p className="mt-4 text-[14px] leading-relaxed text-text-muted">
                  Bir fikir, bir worklog, bir adım — ⌘J ile pop-up açılır, yazıp ⌘↵ ile gönderirsin. Türünü ve sahibini bir tıkla seç. Akış kesilmesin.
                </p>
                <div className="mt-6 grid grid-cols-3 gap-2 text-[12px]">
                  {[
                    { icon: Lightbulb, label: "Fikir" },
                    { icon: CheckSquare, label: "Sonraki adım" },
                    { icon: FolderKanban, label: "Worklog" },
                  ].map((t) => (
                    <div key={t.label} className="flex items-center gap-2 rounded-md border border-border-subtle bg-surface-1 px-3 py-2 text-text-secondary">
                      <t.icon className="h-3.5 w-3.5 text-text-muted" /> {t.label}
                    </div>
                  ))}
                </div>
              </div>
              <div className="md:order-1">
                <QuickCaptureMock />
              </div>
            </div>
          </div>
        </section>

        {/* STAT 3 */}
        <StatBlock
          kicker="Hepsi bağlı"
          big="Bir tek pencere."
          sub="Projeler, eğitimler, worklog'lar, kararlar, fikirler — aynı zihin haritası."
        />

        {/* FEATURE 3 — Sidebar / hayat dolup taşan */}
        <section className="relative z-10 mx-auto max-w-6xl px-6 pb-24">
          <div className="overflow-hidden rounded-2xl border border-border bg-gradient-to-br from-info-soft via-surface-1 to-surface-1 p-8 shadow-card sm:p-12">
            <div className="grid grid-cols-1 items-center gap-10 md:grid-cols-2">
              <div>
                <div className="mb-4 inline-flex items-center gap-2 rounded-full bg-info-soft px-3 py-1 text-[11px] font-medium text-info">
                  <Link2 className="h-3 w-3" /> Hepsi bağlı
                </div>
                <h2 className="text-[28px] font-medium leading-tight tracking-tight text-text sm:text-[36px]">
                  Projeler, eğitimler, <br />
                  <span className="text-text-secondary">aynı zihin haritasında.</span>
                </h2>
                <p className="mt-4 text-[14px] leading-relaxed text-text-muted">
                  Her proje altında bileşenler. Her eğitim altında modüller. Her yerde worklog'lar, kararlar, kaynaklar, fikirler. Sidebar'da son aktivite — "AOS · 2h", "DevTrack · now".
                </p>
                <div className="mt-6 flex flex-wrap gap-2 text-[11px] text-text-muted">
                  {["GitHub commit takibi", "Reminder service", "Tag sistemi", "Soft-delete"].map((t) => (
                    <span key={t} className="rounded-md border border-border-subtle bg-surface-1 px-2.5 py-1">
                      {t}
                    </span>
                  ))}
                </div>
              </div>
              <div className="md:pl-4">
                <SidebarMock />
              </div>
            </div>
          </div>
        </section>

        {/* FINAL CTA */}
        <section className="relative z-10 mx-auto max-w-3xl px-6 pb-24">
          <div className="overflow-hidden rounded-2xl border border-border bg-gradient-to-br from-primary/15 via-surface-1 to-surface-1 p-10 text-center shadow-card ring-1 ring-primary/20 sm:p-14">
            <h2 className="text-[32px] font-medium leading-tight tracking-tight text-text sm:text-[44px]">
              Hadi başlayalım.
            </h2>
            <p className="mx-auto mt-3 max-w-md text-[14px] text-text-secondary">
              Bir hesap aç, ilk projeyi ekle, ⌘J'le ilk fikrini yakala. Tek oturum yeterli.
            </p>
            <div className="mt-8 flex flex-wrap items-center justify-center gap-3">
              <Link href="/register" className={cn(buttonVariants({ variant: "default", size: "lg" }), "h-11 px-5 text-[14px]")}>
                Ücretsiz başla <ArrowRight className="h-4 w-4" />
              </Link>
              <Link href="/login" className={cn(buttonVariants({ variant: "ghost", size: "lg" }), "h-11 px-5 text-[14px]")}>
                Zaten hesabım var
              </Link>
            </div>
          </div>
        </section>

        <footer className="relative z-10 border-t border-border-subtle px-6 py-6 text-center text-[12px] text-text-faint">
          DevTrack · kişisel proje + öğrenme takipçisi
        </footer>
      </div>
    </GuestGuard>
  );
}
