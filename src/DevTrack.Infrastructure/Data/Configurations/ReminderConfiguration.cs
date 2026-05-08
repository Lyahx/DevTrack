using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Data.Configurations;

public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.ToTable("Reminders");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Message).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.Type).HasConversion<int>();
        builder.Property(x => x.Severity).HasConversion<int>();
        builder.Property(x => x.GeneratedAt).IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RelatedProject)
            .WithMany()
            .HasForeignKey(x => x.RelatedProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RelatedLearningTrack)
            .WithMany()
            .HasForeignKey(x => x.RelatedLearningTrackId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.RelatedProjectId);
        builder.HasIndex(x => x.RelatedLearningTrackId);
        builder.HasIndex(x => x.IsRead);
        builder.HasIndex(x => x.IsDismissed);
        builder.HasIndex(x => x.GeneratedAt);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
