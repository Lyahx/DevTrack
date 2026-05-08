using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Data.Configurations;

internal static class OwnedEntityConfigurationHelper
{
    public static void ConfigureOwnership<T>(EntityTypeBuilder<T> builder) where T : BaseOwnedEntity
    {
        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Component)
            .WithMany()
            .HasForeignKey(x => x.ComponentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LearningTrack)
            .WithMany()
            .HasForeignKey(x => x.LearningTrackId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LearningModule)
            .WithMany()
            .HasForeignKey(x => x.LearningModuleId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public static void ConfigureUser<T>(EntityTypeBuilder<T> builder) where T : BaseOwnedEntity
    {
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
