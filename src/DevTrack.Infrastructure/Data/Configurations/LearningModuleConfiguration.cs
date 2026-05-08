using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Data.Configurations;

public class LearningModuleConfiguration : IEntityTypeConfiguration<LearningModule>
{
    public void Configure(EntityTypeBuilder<LearningModule> builder)
    {
        builder.ToTable("LearningModules");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Status).HasConversion<int>();

        builder.HasOne(x => x.LearningTrack)
            .WithMany(t => t.Modules)
            .HasForeignKey(x => x.LearningTrackId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.LearningTrackId);
        builder.HasIndex(x => new { x.LearningTrackId, x.Order });

        builder.HasQueryFilter(x => !x.IsDeleted && !x.LearningTrack.IsDeleted);
    }
}
