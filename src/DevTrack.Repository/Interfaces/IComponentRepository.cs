using DevTrack.Domain.Entities;

namespace DevTrack.Repository.Interfaces;

public interface IComponentRepository
{
    Task<Component?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default);
    Task<IReadOnlyList<Component>> ListByProjectAsync(int projectId, int userId, bool includeDeleted, CancellationToken ct = default);
    Task AddAsync(Component component, CancellationToken ct = default);
    void Update(Component component);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task UpdateLastActivityAsync(int id, DateTime utcNow, CancellationToken ct = default);
    Task<int?> GetParentProjectIdAsync(int componentId, int userId, CancellationToken ct = default);
    Task<IReadOnlyList<int>> GetComponentIdsForProjectAsync(int projectId, int userId, bool includeDeleted, CancellationToken ct = default);
    Task SoftDeleteByProjectAsync(int projectId, DateTime utcNow, CancellationToken ct = default);
}
