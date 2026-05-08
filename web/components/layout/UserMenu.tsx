"use client";

import { Check, LogOut, Monitor, Moon, Settings, Sun } from "lucide-react";
import { useTheme } from "next-themes";
import { useRouter } from "next/navigation";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { buttonVariants } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { useAuthStore } from "@/store/auth";
import { cn } from "@/lib/utils";

const THEME_OPTIONS = [
  { value: "light", label: "Aydınlık", icon: Sun },
  { value: "dark", label: "Karanlık", icon: Moon },
  { value: "system", label: "Sistem", icon: Monitor },
] as const;

export function UserMenu() {
  const router = useRouter();
  const user = useAuthStore((s) => s.user);
  const logout = useAuthStore((s) => s.logout);
  const { theme, setTheme } = useTheme();

  if (!user) return null;

  const initials = user.username.slice(0, 2).toUpperCase();

  return (
    <DropdownMenu>
      <DropdownMenuTrigger
        type="button"
        aria-label="Kullanıcı menüsü"
        className={cn(buttonVariants({ variant: "ghost", size: "icon" }), "rounded-full")}
      >
        <Avatar className="h-7 w-7">
          <AvatarFallback className="text-[10px] font-medium">{initials}</AvatarFallback>
        </Avatar>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-56">
        <div className="flex flex-col px-2 py-1.5">
          <span className="text-sm font-medium">{user.username}</span>
          <span className="text-xs text-text-faint">{user.email}</span>
        </div>
        <DropdownMenuSeparator />
        <div className="px-2 py-1 text-[10px] font-semibold uppercase tracking-wider text-text-faint">Tema</div>
        {THEME_OPTIONS.map(({ value, label, icon: Icon }) => {
          const active = (theme ?? "system") === value;
          return (
            <DropdownMenuItem key={value} onClick={() => setTheme(value)}>
              <Icon className="mr-2 h-4 w-4" /> {label}
              {active ? <Check className="ml-auto h-3.5 w-3.5 text-text-muted" /> : null}
            </DropdownMenuItem>
          );
        })}
        <DropdownMenuSeparator />
        <DropdownMenuItem onClick={() => router.push("/settings")}>
          <Settings className="mr-2 h-4 w-4" /> Ayarlar
        </DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem
          onClick={() => {
            logout();
            router.replace("/login");
          }}
          variant="destructive"
        >
          <LogOut className="mr-2 h-4 w-4" /> Çıkış yap
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
