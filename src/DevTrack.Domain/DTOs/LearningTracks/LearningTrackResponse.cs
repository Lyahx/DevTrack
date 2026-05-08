using DevTrack.Domain.DTOs.Tags;
using DevTrack.Domain.Enums;

namespace DevTrack.Domain.DTOs.LearningTracks;

public class LearningTrackResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Source { get; set; }
    public string? AiChatUrl { get; set; }
    public LearningTrackStatus Status { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public IReadOnlyList<TagResponse> Tags { get; set; } = Array.Empty<TagResponse>();
}
