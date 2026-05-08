using DevTrack.Domain.DTOs.Ideas;
using DevTrack.Domain.Entities;
using DevTrack.Repository.Common;

namespace DevTrack.Repository.Interfaces;

public interface IIdeaRepository
{
    Task<Idea?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default);
    Task<(IReadOnlyList<Idea> Items, int Total)> ListAsync(int userId, IdeaListQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<Idea>> ListByScopeAsync(int userId, OwnerScope scope, bool? unconvertedOnly, int? take, bool includeDeleted, CancellationToken ct = default);
    Task SoftDeleteByScopeAsync(OwnerScope scope, DateTime utcNow, CancellationToken ct = default);
    Task AddAsync(Idea idea, CancellationToken ct = default);
    void Update(Idea idea);
    Task SaveChangesAsync(CancellationToken ct = default);
}
