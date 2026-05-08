"use client";

import { useQuery } from "@tanstack/react-query";
import { ExternalLink, GitCommit, GitBranch } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { commitsApi } from "@/lib/api/commits";
import { formatRelative } from "@/lib/date";

export function CommitList({ projectId, hasRepo }: { projectId: number; hasRepo: boolean }) {
  const data = useQuery({
    queryKey: ["commits", projectId],
    queryFn: () => commitsApi.forProject(projectId, 10),
    enabled: hasRepo,
    staleTime: 5 * 60_000,
  });

  if (!hasRepo) {
    return (
      <Card>
        <CardHeader className="pb-2">
          <CardTitle className="flex items-center gap-2 text-base">
            <GitCommit className="h-4 w-4" /> Son commit&apos;ler
          </CardTitle>
        </CardHeader>
        <CardContent className="text-sm text-muted-foreground">
          Bu projeye repo URL&apos;si bağlanmamış. Düzenle ekranından bir GitHub repo linki ekleyince son
          10 commit burada listelenir.
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader className="pb-2">
        <div className="flex items-center justify-between">
          <CardTitle className="flex items-center gap-2 text-base">
            <GitCommit className="h-4 w-4" /> Son commit&apos;ler
          </CardTitle>
          {data.data?.ownerRepo ? (
            <span className="inline-flex items-center gap-1 text-xs text-muted-foreground">
              <GitBranch className="h-3 w-3" /> {data.data.ownerRepo}
            </span>
          ) : null}
        </div>
      </CardHeader>
      <CardContent>
        {data.isLoading ? (
          <div className="space-y-2">
            {[0, 1, 2].map((i) => <Skeleton key={i} className="h-10 w-full" />)}
          </div>
        ) : data.data?.error ? (
          <p className="rounded-md bg-amber-100/50 p-2 text-xs text-amber-900 dark:bg-amber-950/30 dark:text-amber-300">
            {data.data.error}
          </p>
        ) : !data.data?.commits.length ? (
          <p className="text-sm text-muted-foreground">Commit bulunamadı.</p>
        ) : (
          <ul className="divide-y">
            {data.data.commits.map((c) => (
              <li key={c.sha} className="flex items-start gap-3 py-2">
                <a
                  href={c.url}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="mt-0.5 font-mono text-xs text-muted-foreground hover:text-foreground"
                  title={c.sha}
                >
                  {c.shortSha}
                </a>
                <div className="min-w-0 flex-1">
                  <p className="truncate text-sm">{c.messageHeadline}</p>
                  <p className="text-xs text-muted-foreground">
                    {c.authorName} · {formatRelative(c.authoredAt)}
                  </p>
                </div>
                <a
                  href={c.url}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-muted-foreground hover:text-foreground"
                  aria-label="GitHub'da aç"
                >
                  <ExternalLink className="h-3.5 w-3.5" />
                </a>
              </li>
            ))}
          </ul>
        )}
      </CardContent>
    </Card>
  );
}
