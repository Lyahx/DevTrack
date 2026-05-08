using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Data.Configurations;

public class DecisionConfiguration : IEntityTypeConfiguration<Decision>
{
    public void Configure(EntityTypeBuilder<Decision> builder)
    {
        builder.ToTable("Decisions", t => t.HasCheckConstraint(
            "CK_Decisions_Owner_ExactlyOne",
            OwnedTableSql.OwnerExactlyOne));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Reasoning).IsRequired().HasMaxLength(4000);
        builder.Property(x => x.Alternatives).HasMaxLength(4000);
        builder.Property(x => x.DecidedAt).IsRequired();

        OwnedEntityConfigurationHelper.ConfigureOwnership(builder);
        OwnedEntityConfigurationHelper.ConfigureUser(builder);

        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.ComponentId);
        builder.HasIndex(x => x.LearningTrackId);
        builder.HasIndex(x => x.LearningModuleId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.DecidedAt);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
