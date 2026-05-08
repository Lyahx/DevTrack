namespace DevTrack.Repository.Common;

public class OwnerScope
{
    public IReadOnlyCollection<int> ProjectIds { get; init; } = Array.Empty<int>();
    public IReadOnlyCollection<int> ComponentIds { get; init; } = Array.Empty<int>();
    public IReadOnlyCollection<int> LearningTrackIds { get; init; } = Array.Empty<int>();
    public IReadOnlyCollection<int> LearningModuleIds { get; init; } = Array.Empty<int>();

    public bool IsEmpty =>
        ProjectIds.Count + ComponentIds.Count + LearningTrackIds.Count + LearningModuleIds.Count == 0;

    public static OwnerScope ForProject(int projectId, IReadOnlyCollection<int>? componentIds = null) => new()
    {
        ProjectIds = new[] { projectId },
        ComponentIds = componentIds ?? Array.Empty<int>()
    };

    public static OwnerScope ForComponent(int componentId) => new()
    {
        ComponentIds = new[] { componentId }
    };

    public static OwnerScope ForLearningTrack(int trackId, IReadOnlyCollection<int>? moduleIds = null) => new()
    {
        LearningTrackIds = new[] { trackId },
        LearningModuleIds = moduleIds ?? Array.Empty<int>()
    };

    public static OwnerScope ForLearningModule(int moduleId) => new()
    {
        LearningModuleIds = new[] { moduleId }
    };
}
