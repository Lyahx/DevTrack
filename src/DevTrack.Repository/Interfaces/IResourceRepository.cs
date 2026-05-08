using DevTrack.Domain.DTOs.Resources;
using DevTrack.Domain.Entities;
using DevTrack.Repository.Common;

namespace DevTrack.Repository.Interfaces;

public interface IResourceRepository
{
    Task<Resource?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default);
    Task<(IReadOnlyList<Resource> Items, int Total)> ListAsync(int userId, ResourceListQuery query, CancellationToken ct = default);
    Task<IReadOnlyList<Resource>> ListByScopeAsync(int userId, OwnerScope scope, bool includeDeleted, CancellationToken ct = default);
    Task SoftDeleteByScopeAsync(OwnerScope scope, DateTime utcNow, CancellationToken ct = default);
    Task AddAsync(Resource resource, CancellationToken ct = default);
    void Update(Resource resource);
    Task SaveChangesAsync(CancellationToken ct = default);
}
