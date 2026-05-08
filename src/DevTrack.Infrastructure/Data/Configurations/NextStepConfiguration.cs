using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Data.Configurations;

public class NextStepConfiguration : IEntityTypeConfiguration<NextStep>
{
    public void Configure(EntityTypeBuilder<NextStep> builder)
    {
        builder.ToTable("NextSteps", t => t.HasCheckConstraint(
            "CK_NextSteps_Owner_ExactlyOne",
            OwnedTableSql.OwnerExactlyOne));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.Priority).HasConversion<int>();

        OwnedEntityConfigurationHelper.ConfigureOwnership(builder);
        OwnedEntityConfigurationHelper.ConfigureUser(builder);

        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.ComponentId);
        builder.HasIndex(x => x.LearningTrackId);
        builder.HasIndex(x => x.LearningModuleId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.IsCompleted);
        builder.HasIndex(x => x.Priority);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
