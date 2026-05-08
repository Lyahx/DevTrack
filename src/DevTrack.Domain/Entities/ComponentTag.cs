namespace DevTrack.Domain.Entities;

public class ComponentTag
{
    public int ComponentId { get; set; }
    public int TagId { get; set; }

    public Component Component { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
