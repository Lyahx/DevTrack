"use client";

import { useQuery } from "@tanstack/react-query";
import { Bell } from "lucide-react";
import Link from "next/link";
import { buttonVariants } from "@/components/ui/button";
import { remindersApi } from "@/lib/api/reminders";
import { useAuthStore } from "@/store/auth";
import { cn } from "@/lib/utils";

export function ReminderBell() {
  const status = useAuthStore((s) => s.status);
  const { data } = useQuery({
    queryKey: ["reminders", "unread"],
    queryFn: () => remindersApi.unread(),
    enabled: status === "authenticated",
    refetchInterval: 60_000,
  });

  const count = data?.length ?? 0;

  return (
    <Link
      href="/reminders"
      aria-label="Hatırlatmalar"
      className={cn(buttonVariants({ variant: "ghost", size: "icon" }), "relative")}
    >
      <Bell className="h-4 w-4" />
      {count > 0 ? (
        <span className="absolute -right-0.5 -top-0.5 inline-flex h-4 min-w-4 items-center justify-center rounded-full bg-rose-500 px-1 text-[10px] font-semibold text-white">
          {count > 9 ? "9+" : count}
        </span>
      ) : null}
    </Link>
  );
}
