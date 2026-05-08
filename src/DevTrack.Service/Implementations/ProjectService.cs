using AutoMapper;
using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.Projects;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Enums;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projects;
    private readonly IComponentRepository _components;
    private readonly IWorklogRepository _worklogs;
    private readonly INextStepRepository _steps;
    private readonly IIdeaRepository _ideas;
    private readonly IResourceRepository _resources;
    private readonly ITagRepository _tags;
    private readonly ITransactionFactory _tx;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public ProjectService(
        IProjectRepository projects,
        IComponentRepository components,
        IWorklogRepository worklogs,
        INextStepRepository steps,
        IIdeaRepository ideas,
        IResourceRepository resources,
        ITagRepository tags,
        ITransactionFactory tx,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _projects = projects;
        _components = components;
        _worklogs = worklogs;
        _steps = steps;
        _ideas = ideas;
        _resources = resources;
        _tags = tags;
        _tx = tx;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProjectResponse>> ListAsync(ProjectListQuery query, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 20 : query.PageSize;

        var (items, total) = await _projects.ListAsync(userId, query.Status, query.TagId, page, pageSize, query.IncludeDeleted, ct);

        return new PagedResult<ProjectResponse>
        {
            Items = items.Select(p => _mapper.Map<ProjectResponse>(p)).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<ProjectResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var project = await _projects.GetByIdAsync(id, userId, includeDeleted, ct)
            ?? throw new NotFoundException("Project not found.");
        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task<ProjectResponse> CreateAsync(ProjectCreateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var project = _mapper.Map<Project>(request);
        project.UserId = userId;
        await _projects.AddAsync(project, ct);
        await _projects.SaveChangesAsync(ct);
        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task<ProjectResponse> UpdateAsync(int id, ProjectUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var project = await _projects.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Project not found.");

        project.Name = request.Name;
        project.Description = request.Description;
        project.Goal = request.Goal;
        project.RepoUrl = request.RepoUrl;

        await _projects.SaveChangesAsync(ct);
        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var project = await _projects.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Project not found.");

        var componentIds = await _components.GetComponentIdsForProjectAsync(id, userId, includeDeleted: false, ct);
        var scope = OwnerScope.ForProject(id, componentIds);
        var now = DateTime.UtcNow;

        await using var transaction = await _tx.BeginAsync(ct);
        try
        {
            await _worklogs.SoftDeleteByScopeAsync(scope, now, ct);
            await _steps.SoftDeleteByScopeAsync(scope, now, ct);
            await _ideas.SoftDeleteByScopeAsync(scope, now, ct);
            await _resources.SoftDeleteByScopeAsync(scope, now, ct);
            await _components.SoftDeleteByProjectAsync(id, now, ct);

            project.IsDeleted = true;
            project.DeletedAt = now;
            await _projects.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<ProjectResponse> UpdateStatusAsync(int id, ProjectStatusUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var project = await _projects.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Project not found.");

        project.Status = request.Status;
        project.CompletedAt = request.Status == ProjectStatus.Completed
            ? (project.CompletedAt ?? DateTime.UtcNow)
            : null;

        await _projects.SaveChangesAsync(ct);
        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task AttachTagAsync(int projectId, int tagId, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var project = await _projects.GetByIdAsync(projectId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Project not found.");
        var tag = await _tags.GetByIdAsync(tagId, userId, ct)
            ?? throw new NotFoundException("Tag not found.");

        if (await _tags.ProjectTagExistsAsync(project.Id, tag.Id, ct)) return;

        await _tags.AddProjectTagAsync(new ProjectTag { ProjectId = project.Id, TagId = tag.Id }, ct);
        await _tags.SaveChangesAsync(ct);
    }

    public async Task DetachTagAsync(int projectId, int tagId, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var project = await _projects.GetByIdAsync(projectId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Project not found.");

        await _tags.RemoveProjectTagAsync(project.Id, tagId, ct);
        await _tags.SaveChangesAsync(ct);
    }
}
