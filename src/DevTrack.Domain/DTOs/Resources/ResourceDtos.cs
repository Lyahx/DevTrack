using DevTrack.Domain.Common;
using DevTrack.Domain.Enums;

namespace DevTrack.Domain.DTOs.Resources;

public class ResourceCreateRequest
{
    public OwnerReference Owner { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public ResourceType Type { get; set; }
    public string? Notes { get; set; }
}

public class ResourceUpdateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public ResourceType Type { get; set; }
    public string? Notes { get; set; }
}

public class ResourceResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public OwnerReference Owner { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public ResourceType Type { get; set; }
    public string? Notes { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class ResourceListQuery
{
    public int? ProjectId { get; set; }
    public int? ComponentId { get; set; }
    public int? LearningTrackId { get; set; }
    public int? LearningModuleId { get; set; }
    public ResourceType? Type { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool IncludeDeleted { get; set; } = false;
}
