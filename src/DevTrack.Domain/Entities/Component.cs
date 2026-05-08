using DevTrack.Domain.Enums;

namespace DevTrack.Domain.Entities;

public class Component : SoftDeletableEntity
{
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ComponentType Type { get; set; } = ComponentType.Other;
    public string? TechStack { get; set; }
    public string? LocalUrl { get; set; }
    public string? RepoPath { get; set; }
    public string? CurrentStatusNote { get; set; }
    public DateTime? LastActivityAt { get; set; }

    public Project Project { get; set; } = null!;
    public ICollection<ComponentTag> ComponentTags { get; set; } = new List<ComponentTag>();
}
