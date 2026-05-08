using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Data.Configurations;

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("Resources", t => t.HasCheckConstraint(
            "CK_Resources_Owner_ExactlyOne",
            OwnedTableSql.OwnerExactlyOne));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Url).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.Property(x => x.Type).HasConversion<int>();
        builder.Property(x => x.AddedAt).IsRequired();

        OwnedEntityConfigurationHelper.ConfigureOwnership(builder);
        OwnedEntityConfigurationHelper.ConfigureUser(builder);

        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.ComponentId);
        builder.HasIndex(x => x.LearningTrackId);
        builder.HasIndex(x => x.LearningModuleId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Type);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
