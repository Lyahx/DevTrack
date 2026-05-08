using DevTrack.Domain.Common;

namespace DevTrack.Service.Interfaces;

public interface IOwnerValidator
{
    Task EnsureOwnedByUserAsync(OwnerReference owner, int userId, CancellationToken ct = default);
}
