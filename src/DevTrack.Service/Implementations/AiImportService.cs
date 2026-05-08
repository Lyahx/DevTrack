using DevTrack.Domain.Common;
using DevTrack.Domain.DTOs.AiImport;
using DevTrack.Domain.Entities;
using DevTrack.Domain.Enums;
using DevTrack.Domain.Exceptions;
using DevTrack.Repository.Common;
using DevTrack.Repository.Interfaces;
using DevTrack.Service.Common;
using DevTrack.Service.Interfaces;

namespace DevTrack.Service.Implementations;

public class AiImportService : IAiImportService
{
    private readonly ILearningTrackRepository _tracks;
    private readonly IWorklogRepository _worklogs;
    private readonly IDecisionRepository _decisions;
    private readonly INextStepRepository _nextSteps;
    private readonly IIdeaRepository _ideas;
    private readonly IResourceRepository _resources;
    private readonly ITransactionFactory _tx;
    private readonly IActivityTrackingService _activity;
    private readonly ICurrentUser _currentUser;

    public AiImportService(
        ILearningTrackRepository tracks,
        IWorklogRepository worklogs,
        IDecisionRepository decisions,
        INextStepRepository nextSteps,
        IIdeaRepository ideas,
        IResourceRepository resources,
        ITransactionFactory tx,
        IActivityTrackingService activity,
        ICurrentUser currentUser)
    {
        _tracks = tracks;
        _worklogs = worklogs;
        _decisions = decisions;
        _nextSteps = nextSteps;
        _ideas = ideas;
        _resources = resources;
        _tx = tx;
        _activity = activity;
        _currentUser = currentUser;
    }

    public async Task<AiImportApplyResult> ApplyToLearningTrackAsync(int trackId, AiImportApplyRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.RequireUserId();
        var track = await _tracks.GetByIdAsync(trackId, userId, includeDeleted: false, ct)
            ?? throw new NotFoundException("Learning track not found.");

        var owner = new OwnerReference(OwnerType.LearningTrack, track.Id);
        var now = DateTime.UtcNow;
        var result = new AiImportApplyResult();

        await using var tx = await _tx.BeginAsync(ct);
        try
        {
            foreach (var w in request.Worklogs.Where(x => !string.IsNullOrWhiteSpace(x.WhatIDid)))
            {
                var entity = new Worklog
                {
                    UserId = userId,
                    WhatIDid = Truncate(w.WhatIDid, 4000),
                    WhatsLeft = string.IsNullOrWhiteSpace(w.WhatsLeft) ? null : Truncate(w.WhatsLeft!, 2000),
                    LoggedAt = now,
                };
                entity.ApplyOwner(owner);
                await _worklogs.AddAsync(entity, ct);
                result.WorklogsCreated++;
            }

            foreach (var d in request.Decisions.Where(x => !string.IsNullOrWhiteSpace(x.Title)))
            {
                var entity = new Decision
                {
                    UserId = userId,
                    Title = Truncate(d.Title, 200),
                    Reasoning = Truncate(d.Reasoning ?? string.Empty, 4000),
                    Alternatives = string.IsNullOrWhiteSpace(d.Alternatives) ? null : Truncate(d.Alternatives!, 2000),
                    DecidedAt = now,
                };
                entity.ApplyOwner(owner);
                await _decisions.AddAsync(entity, ct);
                result.DecisionsCreated++;
            }

            foreach (var s in request.NextSteps.Where(x => !string.IsNullOrWhiteSpace(x.Description)))
            {
                var entity = new NextStep
                {
                    UserId = userId,
                    Description = Truncate(s.Description, 1000),
                    Priority = s.Priority,
                };
                entity.ApplyOwner(owner);
                await _nextSteps.AddAsync(entity, ct);
                result.NextStepsCreated++;
            }

            foreach (var i in request.Ideas.Where(x => !string.IsNullOrWhiteSpace(x.Content)))
            {
                var entity = new Idea
                {
                    UserId = userId,
                    Content = Truncate(i.Content, 2000),
                    CapturedAt = now,
                };
                entity.ApplyOwner(owner);
                await _ideas.AddAsync(entity, ct);
                result.IdeasCreated++;
            }

            foreach (var r in request.Resources.Where(x => !string.IsNullOrWhiteSpace(x.Url) && !string.IsNullOrWhiteSpace(x.Title)))
            {
                var entity = new Resource
                {
                    UserId = userId,
                    Title = Truncate(r.Title, 200),
                    Url = Truncate(r.Url, 1000),
                    Type = r.Type,
                    Notes = string.IsNullOrWhiteSpace(r.Notes) ? null : Truncate(r.Notes!, 1000),
                    AddedAt = now,
                };
                entity.ApplyOwner(owner);
                await _resources.AddAsync(entity, ct);
                result.ResourcesCreated++;
            }

            await _worklogs.SaveChangesAsync(ct);
            await _activity.RecordActivityAsync(owner, userId, ct);

            await tx.CommitAsync(ct);
            return result;
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    private static string Truncate(string value, int max) =>
        value.Length <= max ? value : value[..max];
}
