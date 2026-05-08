import type { Metadata } from "next";
import { SettingsClient } from "@/components/settings/SettingsClient";

export const metadata: Metadata = { title: "DevTrack — Ayarlar" };

export default function SettingsPage() {
  return <SettingsClient />;
}
