using DevTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevTrack.Infrastructure.Data.Configurations;

public class WorklogConfiguration : IEntityTypeConfiguration<Worklog>
{
    public void Configure(EntityTypeBuilder<Worklog> builder)
    {
        builder.ToTable("Worklogs", t => t.HasCheckConstraint(
            "CK_Worklogs_Owner_ExactlyOne",
            OwnedTableSql.OwnerExactlyOne));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WhatIDid).IsRequired().HasMaxLength(4000);
        builder.Property(x => x.WhatsLeft).HasMaxLength(4000);
        builder.Property(x => x.LoggedAt).IsRequired();

        OwnedEntityConfigurationHelper.ConfigureOwnership(builder);
        OwnedEntityConfigurationHelper.ConfigureUser(builder);

        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.ComponentId);
        builder.HasIndex(x => x.LearningTrackId);
        builder.HasIndex(x => x.LearningModuleId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.LoggedAt);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
