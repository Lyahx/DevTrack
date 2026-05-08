import { Badge } from "@/components/ui/badge";
import { OWNER_TYPE_LABELS } from "@/types/enums";
import type { OwnerReference } from "@/types/owner";

export function OwnerBadge({ owner, name }: { owner: OwnerReference; name?: string }) {
  return (
    <Badge variant="outline" className="font-normal">
      <span className="text-muted-foreground">{OWNER_TYPE_LABELS[owner.type]}</span>
      {name ? <span className="ml-1.5">{name}</span> : <span className="ml-1.5">#{owner.id}</span>}
    </Badge>
  );
}
