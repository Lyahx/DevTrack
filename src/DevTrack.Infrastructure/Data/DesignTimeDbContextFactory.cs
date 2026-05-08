using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DevTrack.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DevTrackDbContext>
{
    public DevTrackDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("DEVTRACK_CONNECTION_STRING")
            ?? "Server=localhost,1433;Database=DevTrack;User Id=sa;Password=Your_password123;TrustServerCertificate=True;Encrypt=False";

        var options = new DbContextOptionsBuilder<DevTrackDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new DevTrackDbContext(options);
    }
}
