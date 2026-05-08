using DevTrack.Domain.Enums;

namespace DevTrack.Domain.Entities;

public class Project : SoftDeletableEntity
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Goal { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public string? RepoUrl { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Component> Components { get; set; } = new List<Component>();
    public ICollection<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();
}
