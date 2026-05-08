export type CommitInfo = {
  sha: string;
  shortSha: string;
  message: string;
  messageHeadline: string;
  authorName: string;
  authoredAt: string;
  url: string;
};

export type CommitListResponse = {
  repoConfigured: boolean;
  repoSupported: boolean;
  provider: string | null;
  ownerRepo: string | null;
  error: string | null;
  commits: CommitInfo[];
};
