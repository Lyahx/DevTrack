import type { Metadata } from "next";
import { RemindersClient } from "@/components/reminders/RemindersClient";

export const metadata: Metadata = { title: "DevTrack — Hatırlatmalar" };

export default function RemindersPage() {
  return <RemindersClient />;
}
