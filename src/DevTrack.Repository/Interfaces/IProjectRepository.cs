using DevTrack.Domain.Entities;
using DevTrack.Domain.Enums;

namespace DevTrack.Repository.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int id, int userId, bool includeDeleted, CancellationToken ct = default);
    Task<(IReadOnlyList<Project> Items, int Total)> ListAsync(
        int userId,
        ProjectStatus? status,
        int? tagId,
        int page,
        int pageSize,
        bool includeDeleted,
        CancellationToken ct = default);
    Task AddAsync(Project project, CancellationToken ct = default);
    void Update(Project project);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task UpdateLastActivityAsync(int id, DateTime utcNow, CancellationToken ct = default);
    Task<IReadOnlyList<Project>> ListActiveForUserAsync(int userId, CancellationToken ct = default);
}
