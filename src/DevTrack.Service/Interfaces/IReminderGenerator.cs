namespace DevTrack.Service.Interfaces;

public interface IReminderGenerator
{
    Task<int> GenerateForAllUsersAsync(CancellationToken ct = default);
}
