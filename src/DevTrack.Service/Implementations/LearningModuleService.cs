using AutoMapper;
using DevTrack.Domain.DTOs.LearningTracks;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Enums;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class LearningModuleService : ILearningModuleService
{
    private readonly ILearningModuleRepository _modules;
    private readonly ILearningTrackRepository _tracks;
    private readonly IWorklogRepository _worklogs;
    private readonly INextStepRepository _steps;
    private readonly IIdeaRepository _ideas;
    private readonly IResourceRepository _resources;
    private readonly ITransactionFactory _tx;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public LearningModuleService(
        ILearningModuleRepository modules,
        ILearningTrackRepository tracks,
        IWorklogRepository worklogs,
        INextStepRepository steps,
        IIdeaRepository ideas,
        IResourceRepository resources,
        ITransactionFactory tx,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _modules = modules;
        _tracks = tracks;
        _worklogs = worklogs;
        _steps = steps;
        _ideas = ideas;
        _resources = resources;
        _tx = tx;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<LearningModuleResponse>> ListByTrackAsync(int trackId, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        await EnsureTrackAsync(trackId, userId, ct);
        var items = await _modules.ListByTrackAsync(trackId, userId, includeDeleted, ct);
        return items.Select(m => _mapper.Map<LearningModuleResponse>(m)).ToList();
    }

    public async Task<LearningModuleResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var module = await _modules.GetByIdAsync(id, userId, includeDeleted, ct)
            ?? throw new NotFoundException("Module not found.");
        return _mapper.Map<LearningModuleResponse>(module);
    }

    public async Task<LearningModuleResponse> CreateAsync(int trackId, LearningModuleCreateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        await EnsureTrackAsync(trackId, userId, ct);

        var module = _mapper.Map<LearningModule>(request);
        module.LearningTrackId = trackId;
        if (module.Status == LearningModuleStatus.InProgress) module.StartedAt = DateTime.UtcNow;
        if (module.Status == LearningModuleStatus.Completed) { module.StartedAt ??= DateTime.UtcNow; module.CompletedAt = DateTime.UtcNow; }

        await _modules.AddAsync(module, ct);
        await _modules.SaveChangesAsync(ct);
        return _mapper.Map<LearningModuleResponse>(module);
    }

    public async Task<LearningModuleResponse> UpdateAsync(int id, LearningModuleUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var module = await _modules.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Module not found.");

        module.Name = request.Name;
        module.Order = request.Order;

        await _modules.SaveChangesAsync(ct);
        return _mapper.Map<LearningModuleResponse>(module);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var module = await _modules.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Module not found.");

        var scope = OwnerScope.ForLearningModule(id);
        var now = DateTime.UtcNow;

        await using var transaction = await _tx.BeginAsync(ct);
        try
        {
            await _worklogs.SoftDeleteByScopeAsync(scope, now, ct);
            await _steps.SoftDeleteByScopeAsync(scope, now, ct);
            await _ideas.SoftDeleteByScopeAsync(scope, now, ct);
            await _resources.SoftDeleteByScopeAsync(scope, now, ct);

            module.IsDeleted = true;
            module.DeletedAt = now;
            await _modules.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<LearningModuleResponse> UpdateStatusAsync(int id, LearningModuleStatusUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var module = await _modules.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Module not found.");

        module.Status = request.Status;
        switch (request.Status)
        {
            case LearningModuleStatus.NotStarted:
                module.StartedAt = null;
                module.CompletedAt = null;
                break;
            case LearningModuleStatus.InProgress:
                module.StartedAt ??= DateTime.UtcNow;
                module.CompletedAt = null;
                break;
            case LearningModuleStatus.Completed:
                module.StartedAt ??= DateTime.UtcNow;
                module.CompletedAt = module.CompletedAt ?? DateTime.UtcNow;
                break;
        }

        await _modules.SaveChangesAsync(ct);
        return _mapper.Map<LearningModuleResponse>(module);
    }

    public async Task<LearningModuleResponse> UpdateOrderAsync(int id, LearningModuleOrderUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var module = await _modules.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Module not found.");

        module.Order = request.Order;
        await _modules.SaveChangesAsync(ct);
        return _mapper.Map<LearningModuleResponse>(module);
    }

    private async Task EnsureTrackAsync(int trackId, int userId, CancellationToken ct)
    {
        var track = await _tracks.GetByIdAsync(trackId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Learning track not found.");
        _ = track;
    }
}
