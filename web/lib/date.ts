import { format, formatDistanceToNow, parseISO } from "date-fns";
import { tr } from "date-fns/locale";

function toDate(value: string | Date | null | undefined): Date | null {
  if (!value) return null;
  if (value instanceof Date) return value;
  try {
    return parseISO(value);
  } catch {
    return null;
  }
}

export function formatRelative(value: string | Date | null | undefined): string {
  const d = toDate(value);
  if (!d) return "—";
  return formatDistanceToNow(d, { addSuffix: true, locale: tr });
}

export function formatDate(value: string | Date | null | undefined): string {
  const d = toDate(value);
  if (!d) return "—";
  return format(d, "d MMM yyyy", { locale: tr });
}

export function formatDateTime(value: string | Date | null | undefined): string {
  const d = toDate(value);
  if (!d) return "—";
  return format(d, "d MMM yyyy, HH:mm", { locale: tr });
}

export function formatRelativeShort(value: string | Date | null | undefined): string {
  const d = toDate(value);
  if (!d) return "—";
  const diffMs = Date.now() - d.getTime();
  const diffMin = Math.floor(diffMs / 60000);
  if (diffMin < 1) return "now";
  if (diffMin < 60) return `${diffMin}m`;
  const diffHour = Math.floor(diffMin / 60);
  if (diffHour < 24) return `${diffHour}h`;
  const diffDay = Math.floor(diffHour / 24);
  if (diffDay < 30) return `${diffDay}d`;
  const diffMonth = Math.floor(diffDay / 30);
  if (diffMonth < 12) return `${diffMonth}mo`;
  return `${Math.floor(diffMonth / 12)}y`;
}

export function daysSinceLabel(days: number | null | undefined): string {
  if (days == null) return "Hiç dokunulmadı";
  if (days <= 0) return "Bugün";
  if (days === 1) return "1 gün önce";
  return `${days} gün önce`;
}
