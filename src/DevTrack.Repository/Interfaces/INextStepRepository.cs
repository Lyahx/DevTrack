using DevTrack.Domain.DTOs.NextSteps;
using DevTrack.Domain.Entities;
using DevTrack.Repository.Common;

namespace DevTrack.Repository.Interfaces;

public interface INextStepRepository
{
    Task<NextStep?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default);
    Task<(IReadOnlyList<NextStep> Items, int Total)> ListAsync(int userId, NextStepListQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<NextStep>> ListByScopeAsync(int userId, OwnerScope scope, bool? openOnly, int? take, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<NextStep>> ListOpenAsync(int userId, CancellationToken ct = default);
    Task<int> CountOpenAsync(int userId, CancellationToken ct = default);
    Task<IReadOnlyList<NextStep>> ListHighPriorityOpenAsync(int userId, int take, CancellationToken ct = default);
    Task SoftDeleteByScopeAsync(OwnerScope scope, DateTime utcNow, CancellationToken ct = default);
    Task AddAsync(NextStep step, CancellationToken ct = default);
    void Update(NextStep step);
    Task SaveChangesAsync(CancellationToken ct = default);
}
