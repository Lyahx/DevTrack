using DevTrack.Domain.Common;
using DevTrack.Domain.Enums;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class ActivityTrackingService : IActivityTrackingService
{
    private readonly IProjectRepository _projects;
    private readonly IComponentRepository _components;
    private readonly ILearningTrackRepository _tracks;
    private readonly ILearningModuleRepository _modules;

    public ActivityTrackingService(
        IProjectRepository projects,
        IComponentRepository components,
        ILearningTrackRepository tracks,
        ILearningModuleRepository modules)
    {
        _projects = projects;
        _components = components;
        _tracks = tracks;
        _modules = modules;
    }

    public async Task RecordActivityAsync(OwnerReference owner, int userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        switch (owner.Type)
        {
            case OwnerType.Project:
                await _projects.UpdateLastActivityAsync(owner.Id, now, ct);
                break;

            case OwnerType.Component:
                await _components.UpdateLastActivityAsync(owner.Id, now, ct);
                var parentProjectId = await _components.GetParentProjectIdAsync(owner.Id, userId, ct);
                if (parentProjectId.HasValue)
                    await _projects.UpdateLastActivityAsync(parentProjectId.Value, now, ct);
                break;

            case OwnerType.LearningTrack:
                await _tracks.UpdateLastActivityAsync(owner.Id, now, ct);
                break;

            case OwnerType.LearningModule:
                await _modules.UpdateLastActivityAsync(owner.Id, now, ct);
                var parentTrackId = await _modules.GetParentTrackIdAsync(owner.Id, userId, ct);
                if (parentTrackId.HasValue)
                    await _tracks.UpdateLastActivityAsync(parentTrackId.Value, now, ct);
                break;
        }
    }
}
