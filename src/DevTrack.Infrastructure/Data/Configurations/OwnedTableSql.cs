namespace DevTrack.Infrastructure.Data.Configurations;

internal static class OwnedTableSql
{
    public const string OwnerExactlyOne =
        "(CASE WHEN [ProjectId] IS NULL THEN 0 ELSE 1 END" +
        " + CASE WHEN [ComponentId] IS NULL THEN 0 ELSE 1 END" +
        " + CASE WHEN [LearningTrackId] IS NULL THEN 0 ELSE 1 END" +
        " + CASE WHEN [LearningModuleId] IS NULL THEN 0 ELSE 1 END) = 1";
}
