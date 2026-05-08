"use client";

import { AuthGuard } from "@/components/auth/AuthGuard";
import { CommandPalette } from "@/components/command-palette/CommandPalette";
import { QuickCaptureModal } from "@/components/quick-capture/QuickCaptureModal";
import { DetailPanel } from "./DetailPanel";
import { Header } from "./Header";
import { KeyboardShortcuts } from "./KeyboardShortcuts";
import { QuickCaptureFab } from "./QuickCaptureFab";
import { Sidebar } from "./Sidebar";

export function AppShell({ children }: { children: React.ReactNode }) {
  return (
    <AuthGuard>
      <div className="flex min-h-screen flex-col">
        <Header />
        <div className="flex flex-1 overflow-hidden">
          <Sidebar />
          <main className="min-w-0 flex-1 overflow-y-auto">
            <div className="mx-auto w-full max-w-6xl px-4 py-6 md:px-8">{children}</div>
          </main>
          <DetailPanel />
        </div>
        <QuickCaptureFab />
        <QuickCaptureModal />
        <CommandPalette />
        <KeyboardShortcuts />
      </div>
    </AuthGuard>
  );
}
