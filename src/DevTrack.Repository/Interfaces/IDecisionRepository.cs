using DevTrack.Domain.DTOs.Decisions;
using DevTrack.Domain.Entities;
using DevTrack.Repository.Common;

namespace DevTrack.Repository.Interfaces;

public interface IDecisionRepository
{
    Task<Decision?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default);
    Task<(IReadOnlyList<Decision> Items, int Total)> ListAsync(int userId, DecisionListQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<Decision>> ListByScopeAsync(int userId, OwnerScope scope, int? take, bool includeDeleted, CancellationToken ct = default);
    Task SoftDeleteByScopeAsync(OwnerScope scope, DateTime utcNow, CancellationToken ct = default);
    Task AddAsync(Decision decision, CancellationToken ct = default);
    void Update(Decision decision);
    Task SaveChangesAsync(CancellationToken ct = default);
}
