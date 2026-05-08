namespace DevTrack.Service.Interfaces;

public interface IDevSeedService
{
    Task<DevSeedResult> SeedAsync(int userId, CancellationToken ct = default);
    Task<int> WipeAsync(int userId, CancellationToken ct = default);
}

public class DevSeedResult
{
    public int Projects { get; set; }
    public int Components { get; set; }
    public int LearningTracks { get; set; }
    public int LearningModules { get; set; }
    public int Tags { get; set; }
    public int Worklogs { get; set; }
    public int NextSteps { get; set; }
    public int Ideas { get; set; }
    public int Resources { get; set; }
    public int Reminders { get; set; }
}
