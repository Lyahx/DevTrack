namespace DevTrack.Domain.DTOs.Tags;

public class TagCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
}

public class TagUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
}

public class TagResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
