using DevTrack.Domain.Common;

namespace DevTrack.Service.Interfaces;

public interface IActivityTrackingService
{
    Task RecordActivityAsync(OwnerReference owner, int userId, CancellationToken ct = default);
}
