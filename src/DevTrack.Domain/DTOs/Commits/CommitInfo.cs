namespace DevTrack.Domain.DTOs.Commits;

public class CommitInfo
{
    public string Sha { get; set; } = string.Empty;
    public string ShortSha { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string MessageHeadline { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime AuthoredAt { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class CommitListResponse
{
    public bool RepoConfigured { get; set; }
    public bool RepoSupported { get; set; }
    public string? Provider { get; set; }
    public string? OwnerRepo { get; set; }
    public string? Error { get; set; }
    public IReadOnlyList<CommitInfo> Commits { get; set; } = Array.Empty<CommitInfo>();
}
