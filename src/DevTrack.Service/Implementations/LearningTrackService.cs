using AutoMapper;
using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.DTOs.LearningTracks;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Enums;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class LearningTrackService : ILearningTrackService
{
    private readonly ILearningTrackRepository _tracks;
    private readonly ILearningModuleRepository _modules;
    private readonly IWorklogRepository _worklogs;
    private readonly IDecisionRepository _decisions;
    private readonly INextStepRepository _steps;
    private readonly IIdeaRepository _ideas;
    private readonly IResourceRepository _resources;
    private readonly ITagRepository _tags;
    private readonly ITransactionFactory _tx;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public LearningTrackService(
        ILearningTrackRepository tracks,
        ILearningModuleRepository modules,
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
        _tracks = tracks;
        _modules = modules;
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

    public async Task<PagedResult<LearningTrackResponse>> ListAsync(LearningTrackListQuery query, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 20 : query.PageSize;

        var (items, total) = await _tracks.ListAsync(userId, query.Status, query.TagId, page, pageSize, query.IncludeDeleted, ct);

        return new PagedResult<LearningTrackResponse>
        {
            Items = items.Select(t => _mapper.Map<LearningTrackResponse>(t)).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<LearningTrackResponse> GetAsync(int id, bool includeDeleted, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var track = await _tracks.GetByIdAsync(id, userId, includeDeleted, ct)
            ?? throw new NotFoundException("Learning track not found.");
        return _mapper.Map<LearningTrackResponse>(track);
    }

    public async Task<LearningTrackResponse> CreateAsync(LearningTrackCreateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var track = _mapper.Map<LearningTrack>(request);
        track.UserId = userId;
        await _tracks.AddAsync(track, ct);
        await _tracks.SaveChangesAsync(ct);
        return _mapper.Map<LearningTrackResponse>(track);
    }

    public async Task<LearningTrackResponse> UpdateAsync(int id, LearningTrackUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var track = await _tracks.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Learning track not found.");

        track.Name = request.Name;
        track.Description = request.Description;
        track.Source = request.Source;

        await _tracks.SaveChangesAsync(ct);
        return _mapper.Map<LearningTrackResponse>(track);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var track = await _tracks.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Learning track not found.");

        var moduleIds = await _modules.GetModuleIdsForTrackAsync(id, userId, includeDeleted: false, ct);
        var scope = OwnerScope.ForLearningTrack(id, moduleIds);
        var now = DateTime.UtcNow;

        await using var transaction = await _tx.BeginAsync(ct);
        try
        {
            await _worklogs.SoftDeleteByScopeAsync(scope, now, ct);
            await _decisions.SoftDeleteByScopeAsync(scope, now, ct);
            await _steps.SoftDeleteByScopeAsync(scope, now, ct);
            await _ideas.SoftDeleteByScopeAsync(scope, now, ct);
            await _resources.SoftDeleteByScopeAsync(scope, now, ct);
            await _modules.SoftDeleteByTrackAsync(id, now, ct);

            track.IsDeleted = true;
            track.DeletedAt = now;
            await _tracks.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<LearningTrackResponse> UpdateStatusAsync(int id, LearningTrackStatusUpdateRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var track = await _tracks.GetByIdAsync(id, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Learning track not found.");

        track.Status = request.Status;
        track.CompletedAt = request.Status == LearningTrackStatus.Completed
            ? (track.CompletedAt ?? DateTime.UtcNow)
            : null;

        await _tracks.SaveChangesAsync(ct);
        return _mapper.Map<LearningTrackResponse>(track);
    }

    public async Task AttachTagAsync(int trackId, int tagId, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var track = await _tracks.GetByIdAsync(trackId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Learning track not found.");
        var tag = await _tags.GetByIdAsync(tagId, userId, ct)
            ?? throw new NotFoundException("Tag not found.");

        if (await _tags.LearningTrackTagExistsAsync(track.Id, tag.Id, ct)) return;

        await _tags.AddLearningTrackTagAsync(new LearningTrackTag { LearningTrackId = track.Id, TagId = tag.Id }, ct);
        await _tags.SaveChangesAsync(ct);
    }

    public async Task DetachTagAsync(int trackId, int tagId, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var track = await _tracks.GetByIdAsync(trackId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Learning track not found.");

        await _tags.RemoveLearningTrackTagAsync(track.Id, tagId, ct);
        await _tags.SaveChangesAsync(ct);
    }
}
