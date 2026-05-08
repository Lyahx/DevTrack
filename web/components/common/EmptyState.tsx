import type { ReactNode } from "react";

export function EmptyState({
  icon,
  title,
  description,
  action,
}: {
  icon?: ReactNode;
  title: string;
  description?: string;
  action?: ReactNode;
}) {
  return (
    <div className="flex flex-col items-center justify-center gap-2 rounded-md border border-border-subtle bg-surface-1 px-6 py-10 text-center shadow-soft">
      {icon ? (
        <div className="mb-1 flex h-11 w-11 items-center justify-center rounded-full bg-surface-2 text-text-muted [&_svg]:size-5">
          {icon}
        </div>
      ) : null}
      <h3 className="text-[14px] font-medium text-text">{title}</h3>
      {description ? <p className="max-w-sm text-[12.5px] text-text-muted">{description}</p> : null}
      {action ? <div className="mt-2">{action}</div> : null}
    </div>
  );
}
