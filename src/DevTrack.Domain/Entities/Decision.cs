namespace DevTrack.Domain.Entities;

public class Decision : BaseOwnedEntity
{
    public string Title { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public string? Alternatives { get; set; }
    public DateTime DecidedAt { get; set; }
}
