using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Data.Configurations;

public class ComponentConfiguration : IEntityTypeConfiguration<Component>
{
    public void Configure(EntityTypeBuilder<Component> builder)
    {
        builder.ToTable("Components");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.TechStack).HasMaxLength(500);
        builder.Property(x => x.LocalUrl).HasMaxLength(500);
        builder.Property(x => x.RepoPath).HasMaxLength(500);
        builder.Property(x => x.CurrentStatusNote).HasMaxLength(2000);
        builder.Property(x => x.Type).HasConversion<int>();

        builder.HasOne(x => x.Project)
            .WithMany(p => p.Components)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.Type);

        builder.HasQueryFilter(x => !x.IsDeleted && !x.Project.IsDeleted);
    }
}
