using DevTrack.Domain.Entities;
using DevTrack.Domain.Enums;

namespace DevTrack.Repository.Interfaces;

public interface ILearningTrackRepository
{
    Task<LearningTrack?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default);
    Task<(IReadOnlyList<LearningTrack> Items, int Total)> ListAsync(
        int userId,
        LearningTrackStatus? status,
        int? tagId,
        int page,
        int pageSize,
        bool includeDeleted,
        CancellationToken ct = default);
    Task AddAsync(LearningTrack track, CancellationToken ct = default);
    void Update(LearningTrack track);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task UpdateLastActivityAsync(int id, DateTime utcNow, CancellationToken ct = default);
    Task<IReadOnlyList<LearningTrack>> ListActiveForUserAsync(int userId, CancellationToken ct = default);
}
