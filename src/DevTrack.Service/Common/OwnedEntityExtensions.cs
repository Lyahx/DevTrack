using DevTrack.Domain.Common;
using DevTrack.Domain.Entities;

namespace DevTrack.Service.Common;

internal static class OwnedEntityExtensions
{
    public static void ApplyOwner(this BaseOwnedEntity entity, OwnerReference owner)
    {
        var (projectId, componentId, learningTrackId, learningModuleId) = owner.ToColumns();
        entity.ProjectId = projectId;
        entity.ComponentId = componentId;
        entity.LearningTrackId = learningTrackId;
        entity.LearningModuleId = learningModuleId;
    }
}
