using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using DevTrack.Domain.DTOs.Commits;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DevTrack.Service.Implementations;

public class GitHubCommitService : ICommitService
{
    public const string HttpClientName = "github";

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
    };

    private readonly IProjectRepository _projects;
    private readonly ICurrentUser _currentUser;
    private readonly IHttpClientFactory _httpFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GitHubCommitService> _logger;

    public GitHubCommitService(
        IProjectRepository projects,
        ICurrentUser currentUser,
        IHttpClientFactory httpFactory,
        IMemoryCache cache,
        ILogger<GitHubCommitService> logger)
    {
        _projects = projects;
        _currentUser = currentUser;
        _httpFactory = httpFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task<CommitListResponse> GetForProjectAsync(int projectId, int take, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var project = await _projects.GetByIdAsync(projectId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Project not found.");

        if (string.IsNullOrWhiteSpace(project.RepoUrl))
            return new CommitListResponse { RepoConfigured = false, RepoSupported = false };

        var parsed = TryParseGitHub(project.RepoUrl);
        if (parsed is null)
        {
            return new CommitListResponse
            {
                RepoConfigured = true,
                RepoSupported = false,
                Error = "Sadece github.com URL'leri destekleniyor.",
            };
        }

        var (owner, repo) = parsed.Value;
        var cacheKey = $"commits:{owner}/{repo}:{take}";
        if (_cache.TryGetValue<CommitListResponse>(cacheKey, out var cached) && cached is not null)
            return cached;

        var client = _httpFactory.CreateClient(HttpClientName);
        var url = $"https://api.github.com/repos/{owner}/{repo}/commits?per_page={take}";
        try
        {
            using var resp = await client.GetAsync(url, ct);
            if (!resp.IsSuccessStatusCode)
            {
                var msg = resp.StatusCode switch
                {
                    HttpStatusCode.NotFound => "Repo bulunamadı veya private.",
                    HttpStatusCode.Forbidden => "GitHub rate limit aşıldı (anonymous: saatte 60).",
                    _ => $"GitHub yanıtı: {(int)resp.StatusCode}",
                };
                _logger.LogWarning("GitHub commit fetch failed for {OwnerRepo}: {Code}", $"{owner}/{repo}", resp.StatusCode);
                var failure = new CommitListResponse
                {
                    RepoConfigured = true,
                    RepoSupported = true,
                    Provider = "github",
                    OwnerRepo = $"{owner}/{repo}",
                    Error = msg,
                };
                // Cache the failure briefly so we don't hammer the API
                _cache.Set(cacheKey, failure, TimeSpan.FromMinutes(1));
                return failure;
            }

            var stream = await resp.Content.ReadAsStreamAsync(ct);
            var raw = await JsonSerializer.DeserializeAsync<List<GitHubRawCommit>>(stream, JsonOpts, ct) ?? new();
            var commits = raw
                .Where(r => !string.IsNullOrEmpty(r.Sha))
                .Select(r =>
                {
                    var msg = r.Commit?.Message ?? string.Empty;
                    var headline = msg.Split('\n', 2)[0].Trim();
                    return new CommitInfo
                    {
                        Sha = r.Sha!,
                        ShortSha = r.Sha!.Length >= 7 ? r.Sha[..7] : r.Sha,
                        Message = msg,
                        MessageHeadline = headline,
                        AuthorName = r.Commit?.Author?.Name ?? "—",
                        AuthoredAt = r.Commit?.Author?.Date ?? DateTime.UtcNow,
                        Url = r.HtmlUrl ?? string.Empty,
                    };
                })
                .ToList();

            var result = new CommitListResponse
            {
                RepoConfigured = true,
                RepoSupported = true,
                Provider = "github",
                OwnerRepo = $"{owner}/{repo}",
                Commits = commits,
            };
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GitHub commit fetch threw for {OwnerRepo}", $"{owner}/{repo}");
            return new CommitListResponse
            {
                RepoConfigured = true,
                RepoSupported = true,
                Provider = "github",
                OwnerRepo = $"{owner}/{repo}",
                Error = "GitHub'a ulaşılamadı.",
            };
        }
    }

    private static (string Owner, string Repo)? TryParseGitHub(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return null;
        if (!uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase)) return null;
        var parts = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) return null;
        var owner = parts[0];
        var repo = parts[1].EndsWith(".git", StringComparison.OrdinalIgnoreCase) ? parts[1][..^4] : parts[1];
        if (string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(repo)) return null;
        return (owner, repo);
    }

    private sealed class GitHubRawCommit
    {
        [JsonPropertyName("sha")] public string? Sha { get; set; }
        [JsonPropertyName("html_url")] public string? HtmlUrl { get; set; }
        [JsonPropertyName("commit")] public GitHubRawCommitDetails? Commit { get; set; }
    }

    private sealed class GitHubRawCommitDetails
    {
        [JsonPropertyName("message")] public string? Message { get; set; }
        [JsonPropertyName("author")] public GitHubRawAuthor? Author { get; set; }
    }

    private sealed class GitHubRawAuthor
    {
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("date")] public DateTime Date { get; set; }
    }
}
