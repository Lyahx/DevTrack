namespace DevTrack.Domain.Entities;

public class Idea : BaseOwnedEntity
{
    public string Content { get; set; } = string.Empty;
    public bool IsConvertedToNextStep { get; set; }
    public int? ConvertedNextStepId { get; set; }
    public DateTime CapturedAt { get; set; }

    public NextStep? ConvertedNextStep { get; set; }
}
