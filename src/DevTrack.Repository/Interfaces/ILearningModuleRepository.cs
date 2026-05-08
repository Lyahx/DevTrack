using DevTrack.Domain.Entities;

namespace DevTrack.Repository.Interfaces;

public interface ILearningModuleRepository
{
    Task<LearningModule?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<LearningModule>> ListByTrackAsync(int trackId, int userId, bool includeDeleted, CancellationToken ct = default);
    Task AddAsync(LearningModule module, CancellationToken ct = default);
    void Update(LearningModule module);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task UpdateLastActivityAsync(int id, DateTime utcNow, CancellationToken ct = default);
    Task<int?> GetParentTrackIdAsync(int moduleId, int userId, CancellationToken ct = default);
    Task<IReadOnlyList<int>> GetModuleIdsForTrackAsync(int trackId, int userId, bool includeDeleted, CancellationToken ct = default);
    Task<(int Total, int Completed)> GetModuleProgressAsync(int trackId, int userId, CancellationToken ct = default);
    Task SoftDeleteByTrackAsync(int trackId, DateTime utcNow, CancellationToken ct = default);
}
