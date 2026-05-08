using DevTrack.Domain.Enums;

namespace DevTrack.Domain.DTOs.Components;

public class ComponentCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public ComponentType Type { get; set; } = ComponentType.Other;
    public string? TechStack { get; set; }
    public string? LocalUrl { get; set; }
    public string? RepoPath { get; set; }
    public string? CurrentStatusNote { get; set; }
}

public class ComponentUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public ComponentType Type { get; set; }
    public string? TechStack { get; set; }
    public string? LocalUrl { get; set; }
    public string? RepoPath { get; set; }
    public string? CurrentStatusNote { get; set; }
}

public class ComponentStatusNoteRequest
{
    public string? CurrentStatusNote { get; set; }
}
