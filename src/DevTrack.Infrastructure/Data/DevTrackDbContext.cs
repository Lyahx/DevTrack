using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Infrastructure.Data;

public class DevTrackDbContext : DbContext
{
    public DevTrackDbContext(DbContextOptions<DevTrackDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Component> Components => Set<Component>();
    public DbSet<LearningTrack> LearningTracks => Set<LearningTrack>();
    public DbSet<LearningModule> LearningModules => Set<LearningModule>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ProjectTag> ProjectTags => Set<ProjectTag>();
    public DbSet<ComponentTag> ComponentTags => Set<ComponentTag>();
    public DbSet<LearningTrackTag> LearningTrackTags => Set<LearningTrackTag>();
    public DbSet<Worklog> Worklogs => Set<Worklog>();
    public DbSet<Decision> Decisions => Set<Decision>();
    public DbSet<NextStep> NextSteps => Set<NextStep>();
    public DbSet<Idea> Ideas => Set<Idea>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<Reminder> Reminders => Set<Reminder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DevTrackDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditAndSoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditAndSoftDelete();
        return base.SaveChanges();
    }

    private void ApplyAuditAndSoftDelete()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = null;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries<SoftDeletableEntity>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = now;
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}
