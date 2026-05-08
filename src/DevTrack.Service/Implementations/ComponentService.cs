using AutoMapper;
using DevTrack.Domain.DTOs.Components;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class ComponentService : IComponentService
{
    private readonly IComponentRepository _components;
    private readonly IProjectRepository _projects;
    private readonly IWorklogRepository _worklogs;
    private readonly IDecisionRepository _decisions;
    private readonly INextStepRepository _steps;
    private readonly IIdeaRepository _ideas;
    private readonly IResourceRepository _resources;
    private readonly ITagRepository _tags;
    private readonly ITransactionFactory _tx;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public ComponentService(
        IComponentRepository components,
        IProjectRepository projects,
        IWorklogRepository worklogs,
        IDecisionRepository decisions,
        INextStepRepository steps,
        IIdeaRepository ideas,
        IResourceRepository resources,
        ITagRepository tags,
        ITransactionFactory tx,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _components = components;
        _projects = projects;
        _worklogs = worklogs;
        _decisions = decisions;
        _steps = steps;
        _ideas = ideas;
        _resources = resources;
        _tags = tags;
        _tx = tx;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ComponentResponse>> ListByProjectAsync(int projectId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        await EnsureProjectAsync(projectId, userId, ct);
        var items = await _components.ListByProjectAsync(projectId, userId, includeDeleted, ct);
        return items.Select(c => _mapper.Map<ComponentResponse>(c)).ToList();
    }

    public async Task<ComponentResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var component = await _components.GetByIdAsync(id, userId, includeDeleted, ct)
            ?? throw new NotFoundException("Component not found.");
        return _mapper.Map<ComponentResponse>(component);
    }

    public async Task<ComponentResponse> CreateAsync(int projectId, ComponentCreateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        await EnsureProjectAsync(projectId, userId, ct);

        var component = _mapper.Map<Component>(request);
        component.ProjectId = projectId;
        await _components.AddAsync(component, ct);
        await _components.SaveChangesAsync(ct);
        return _mapper.Map<ComponentResponse>(component);
    }

    public async Task<ComponentResponse> UpdateAsync(int id, ComponentUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var component = await _components.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Component not found.");

        component.Name = request.Name;
        component.Type = request.Type;
        component.TechStack = request.TechStack;
        component.LocalUrl = request.LocalUrl;
        component.RepoPath = request.RepoPath;
        component.CurrentStatusNote = request.CurrentStatusNote;

        await _components.SaveChangesAsync(ct);
        return _mapper.Map<ComponentResponse>(component);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var component = await _components.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Component not found.");

        var scope = OwnerScope.ForComponent(id);
        var now = DateTime.UtcNow;

        await using var transaction = await _tx.BeginAsync(ct);
        try
        {
            await _worklogs.SoftDeleteByScopeAsync(scope, now, ct);
            await _decisions.SoftDeleteByScopeAsync(scope, now, ct);
            await _steps.SoftDeleteByScopeAsync(scope, now, ct);
            await _ideas.SoftDeleteByScopeAsync(scope, now, ct);
            await _resources.SoftDeleteByScopeAsync(scope, now, ct);

            component.IsDeleted = true;
            component.DeletedAt = now;
            await _components.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<ComponentResponse> UpdateStatusNoteAsync(int id, ComponentStatusNoteRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var component = await _components.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Component not found.");

        component.CurrentStatusNote = request.CurrentStatusNote;
        await _components.SaveChangesAsync(ct);
        return _mapper.Map<ComponentResponse>(component);
    }

    public async Task AttachTagAsync(int componentId, int tagId, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var component = await _components.GetByIdAsync(componentId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Component not found.");
        var tag = await _tags.GetByIdAsync(tagId, userId, ct)
            ?? throw new NotFoundException("Tag not found.");

        if (await _tags.ComponentTagExistsAsync(component.Id, tag.Id, ct)) return;

        await _tags.AddComponentTagAsync(new ComponentTag { ComponentId = component.Id, TagId = tag.Id }, ct);
        await _tags.SaveChangesAsync(ct);
    }

    public async Task DetachTagAsync(int componentId, int tagId, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var component = await _components.GetByIdAsync(componentId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Component not found.");

        await _tags.RemoveComponentTagAsync(component.Id, tagId, ct);
        await _tags.SaveChangesAsync(ct);
    }

    private async Task EnsureProjectAsync(int projectId, int userId, CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(projectId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Project not found.");
        _ = project;
    }
}
