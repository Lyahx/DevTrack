using DevTrack.Domain.DTOs.Tags;
using DevTrack.Domain.Enums;

namespace DevTrack.Domain.DTOs.Components;

public class ComponentResponse
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ComponentType Type { get; set; }
    public string? TechStack { get; set; }
    public string? LocalUrl { get; set; }
    public string? RepoPath { get; set; }
    public string? CurrentStatusNote { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public IReadOnlyList<TagResponse> Tags { get; set; } = Array.Empty<TagResponse>();
}
