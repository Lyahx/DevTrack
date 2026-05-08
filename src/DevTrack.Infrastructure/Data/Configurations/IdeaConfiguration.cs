using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Data.Configurations;

public class IdeaConfiguration : IEntityTypeConfiguration<Idea>
{
    public void Configure(EntityTypeBuilder<Idea> builder)
    {
        builder.ToTable("Ideas", t => t.HasCheckConstraint(
            "CK_Ideas_Owner_ExactlyOne",
            OwnedTableSql.OwnerExactlyOne));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.CapturedAt).IsRequired();

        OwnedEntityConfigurationHelper.ConfigureOwnership(builder);
        OwnedEntityConfigurationHelper.ConfigureUser(builder);

        builder.HasOne(x => x.ConvertedNextStep)
            .WithMany()
            .HasForeignKey(x => x.ConvertedNextStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.ComponentId);
        builder.HasIndex(x => x.LearningTrackId);
        builder.HasIndex(x => x.LearningModuleId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.IsConvertedToNextStep);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
