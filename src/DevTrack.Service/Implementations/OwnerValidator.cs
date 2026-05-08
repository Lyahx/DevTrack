using DevTrack.Domain.Common;
using DevTrack.Domain.Enums;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class OwnerValidator : IOwnerValidator
{
    private readonly IProjectRepository _projects;
    private readonly IComponentRepository _components;
    private readonly ILearningTrackRepository _tracks;
    private readonly ILearningModuleRepository _modules;

    public OwnerValidator(
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

    public async Task EnsureOwnedByUserAsync(OwnerReference owner, int userId, CancellationToken ct = default)
    {
        switch (owner.Type)
        {
            case OwnerType.Project:
                if (await _projects.GetByIdAsync(owner.Id, userId, includeDeleted: false, ct) is null)
                    throw new NotFoundException("Owner project not found.");
                break;
            case OwnerType.Component:
                if (await _components.GetByIdAsync(owner.Id, userId, includeDeleted: false, ct) is null)
                    throw new NotFoundException("Owner component not found.");
                break;
            case OwnerType.LearningTrack:
                if (await _tracks.GetByIdAsync(owner.Id, userId, includeDeleted: false, ct) is null)
                    throw new NotFoundException("Owner learning track not found.");
                break;
            case OwnerType.LearningModule:
                if (await _modules.GetByIdAsync(owner.Id, userId, includeDeleted: false, ct) is null)
                    throw new NotFoundException("Owner learning module not found.");
                break;
            default:
                throw new ValidationFailedException(new Dictionary<string, string[]>
                {
                    { "Owner.Type", new[] { "Unknown owner type." } }
                });
        }
    }
}
