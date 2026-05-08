using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Data.Configurations;

public class LearningTrackConfiguration : IEntityTypeConfiguration<LearningTrack>
{
    public void Configure(EntityTypeBuilder<LearningTrack> builder)
    {
        builder.ToTable("LearningTracks");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Source).HasMaxLength(200);
        builder.Property(x => x.Status).HasConversion<int>();

        builder.HasOne(x => x.User)
            .WithMany(u => u.LearningTracks)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Status);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
