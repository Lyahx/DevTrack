import { Badge } from "@/components/ui/badge";
import type { TagResponse } from "@/types/tag";

export function TagChips({ tags, max }: { tags: TagResponse[]; max?: number }) {
  if (tags.length === 0) return null;
  const visible = max ? tags.slice(0, max) : tags;
  const rest = max ? tags.length - visible.length : 0;
  return (
    <div className="flex flex-wrap items-center gap-1">
      {visible.map((t) => (
        <Badge
          key={t.id}
          variant="outline"
          style={t.color ? { borderColor: t.color, color: t.color } : undefined}
        >
          {t.name}
        </Badge>
      ))}
      {rest > 0 ? (
        <Badge variant="ghost">+{rest}</Badge>
      ) : null}
    </div>
  );
}
