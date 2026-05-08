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

export function daysSinceLabel(days: number | null | undefined): string {
  if (days == null) return "Hiç dokunulmadı";
  if (days <= 0) return "Bugün";
  if (days === 1) return "1 gün önce";
  return `${days} gün önce`;
}
