using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DevTrack.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DevTrackDbContext>
{
    public DevTrackDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("DEVTRACK_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=devtrack;Username=devtrack;Password=devtrack";

        var options = new DbContextOptionsBuilder<DevTrackDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new DevTrackDbContext(options);
    }
}
