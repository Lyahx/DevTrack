using DevTrack.Domain.Common;

namespace DevTrack.Domain.DTOs.Decisions;

public class DecisionCreateRequest
{
    public OwnerReference Owner { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public string? Alternatives { get; set; }
    public DateTime? DecidedAt { get; set; }
}

public class DecisionUpdateRequest
{
    public string Title { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public string? Alternatives { get; set; }
    public DateTime? DecidedAt { get; set; }
}

public class DecisionResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public OwnerReference Owner { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public string? Alternatives { get; set; }
    public DateTime DecidedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class DecisionListQuery
{
    public int? ProjectId { get; set; }
    public int? ComponentId { get; set; }
    public int? LearningTrackId { get; set; }
    public int? LearningModuleId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool IncludeDeleted { get; set; } = false;
}
