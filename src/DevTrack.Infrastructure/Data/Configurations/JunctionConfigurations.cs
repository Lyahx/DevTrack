using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Data.Configurations;

public class ProjectTagConfiguration : IEntityTypeConfiguration<ProjectTag>
{
    public void Configure(EntityTypeBuilder<ProjectTag> builder)
    {
        builder.ToTable("ProjectTags");
        builder.HasKey(x => new { x.ProjectId, x.TagId });

        builder.HasOne(x => x.Project)
            .WithMany(p => p.ProjectTags)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany(t => t.ProjectTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ComponentTagConfiguration : IEntityTypeConfiguration<ComponentTag>
{
    public void Configure(EntityTypeBuilder<ComponentTag> builder)
    {
        builder.ToTable("ComponentTags");
        builder.HasKey(x => new { x.ComponentId, x.TagId });

        builder.HasOne(x => x.Component)
            .WithMany(c => c.ComponentTags)
            .HasForeignKey(x => x.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany(t => t.ComponentTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class LearningTrackTagConfiguration : IEntityTypeConfiguration<LearningTrackTag>
{
    public void Configure(EntityTypeBuilder<LearningTrackTag> builder)
    {
        builder.ToTable("LearningTrackTags");
        builder.HasKey(x => new { x.LearningTrackId, x.TagId });

        builder.HasOne(x => x.LearningTrack)
            .WithMany(t => t.LearningTrackTags)
            .HasForeignKey(x => x.LearningTrackId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany(t => t.LearningTrackTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
