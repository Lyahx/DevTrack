using DevTrack.Domain.Enums;

namespace DevTrack.Domain.Common;

public record OwnerReference(OwnerType Type, int Id)
{
    public (int? ProjectId, int? ComponentId, int? LearningTrackId, int? LearningModuleId) ToColumns() => Type switch
    {
        OwnerType.Project => (Id, null, null, null),
        OwnerType.Component => (null, Id, null, null),
        OwnerType.LearningTrack => (null, null, Id, null),
        OwnerType.LearningModule => (null, null, null, Id),
        _ => throw new ArgumentOutOfRangeException(nameof(Type))
    };

    public static OwnerReference FromColumns(int? projectId, int? componentId, int? learningTrackId, int? learningModuleId)
    {
        if (componentId.HasValue) return new OwnerReference(OwnerType.Component, componentId.Value);
        if (learningModuleId.HasValue) return new OwnerReference(OwnerType.LearningModule, learningModuleId.Value);
        if (projectId.HasValue) return new OwnerReference(OwnerType.Project, projectId.Value);
        if (learningTrackId.HasValue) return new OwnerReference(OwnerType.LearningTrack, learningTrackId.Value);
        throw new InvalidOperationException("All four owner FK columns are null — record is corrupt.");
    }
}
