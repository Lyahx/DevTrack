using DevTrack.Domain.Enums;

namespace DevTrack.Domain.Entities;

public class Resource : BaseOwnedEntity
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public ResourceType Type { get; set; }
    public string? Notes { get; set; }
    public DateTime AddedAt { get; set; }
}
