import { Badge } from "@/components/ui/badge";
import type { TagResponse } from "@/types/tag";

export function TagChips({ tags }: { tags: TagResponse[] }) {
  if (tags.length === 0) return null;
  return (
    <div className="flex flex-wrap gap-1">
      {tags.map((t) => (
        <Badge
          key={t.id}
          variant="outline"
          style={t.color ? { borderColor: t.color, color: t.color } : undefined}
          className="text-xs"
        >
          {t.name}
        </Badge>
      ))}
    </div>
  );
}
