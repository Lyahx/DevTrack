"use client";

import Link from "next/link";
import { CommandTrigger } from "./CommandTrigger";
import { ReminderBell } from "./ReminderBell";
import { UserMenu } from "./UserMenu";

export function Header() {
  return (
    <header className="sticky top-0 z-40 flex h-12 items-center gap-3 border-b border-border bg-surface-1 px-4 md:px-5">
      <Link href="/" className="flex items-center gap-2 text-[13px] font-medium text-text">
        <span className="inline-block h-5 w-5 rounded-sm bg-primary text-center text-[11px] font-semibold leading-5 text-primary-foreground">D</span>
        <span>DevTrack</span>
      </Link>
      <div className="flex flex-1 justify-center">
        <CommandTrigger />
      </div>
      <div className="flex items-center gap-1">
        <ReminderBell />
        <UserMenu />
      </div>
    </header>
  );
}
