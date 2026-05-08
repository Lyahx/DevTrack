import type { OwnerReference } from "@/types/owner";

export type Scope =
  | { kind: "project"; id: number }
  | { kind: "component"; id: number }
  | { kind: "track"; id: number }
  | { kind: "module"; id: number };

export function scopeToOwner(scope: Scope): OwnerReference {
  switch (scope.kind) {
    case "project":
      return { type: "Project", id: scope.id };
    case "component":
      return { type: "Component", id: scope.id };
    case "track":
      return { type: "LearningTrack", id: scope.id };
    case "module":
      return { type: "LearningModule", id: scope.id };
  }
}

export function scopeKey(scope: Scope) {
  return [scope.kind, scope.id] as const;
}
