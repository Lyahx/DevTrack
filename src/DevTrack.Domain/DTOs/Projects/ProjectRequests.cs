using DevTrack.Domain.Enums;

namespace DevTrack.Domain.DTOs.Projects;

public class ProjectCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Goal { get; set; }
    public string? RepoUrl { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
}

public class ProjectUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Goal { get; set; }
    public string? RepoUrl { get; set; }
}

public class ProjectStatusUpdateRequest
{
    public ProjectStatus Status { get; set; }
}

public class ProjectListQuery
{
    public ProjectStatus? Status { get; set; }
    public int? TagId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool IncludeDeleted { get; set; } = false;
}
