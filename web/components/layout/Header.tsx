"use client";

import Link from "next/link";
import { CommandTrigger } from "./CommandTrigger";
import { ReminderBell } from "./ReminderBell";
import { UserMenu } from "./UserMenu";

export function Header() {
  return (
    <header className="sticky top-0 z-40 flex h-14 items-center gap-3 border-b bg-background/80 px-4 backdrop-blur md:px-6">
      <Link href="/" className="flex items-center gap-2 font-semibold">
        <span className="inline-block h-6 w-6 rounded-md bg-primary text-primary-foreground text-xs font-bold leading-6 text-center">D</span>
        <span>DevTrack</span>
      </Link>
      <div className="mx-auto flex w-full max-w-2xl items-center justify-center">
        <CommandTrigger />
      </div>
      <div className="flex items-center gap-1">
        <ReminderBell />
        <UserMenu />
      </div>
    </header>
  );
}
