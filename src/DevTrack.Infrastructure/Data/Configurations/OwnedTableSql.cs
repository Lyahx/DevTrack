namespace DevTrack.Infrastructure.Data.Configurations;

internal static class OwnedTableSql
{
    // PostgreSQL identifier quoting: PascalCase columns need to be wrapped in double quotes.
    public const string OwnerExactlyOne =
        "(CASE WHEN \"ProjectId\" IS NULL THEN 0 ELSE 1 END" +
        " + CASE WHEN \"ComponentId\" IS NULL THEN 0 ELSE 1 END" +
        " + CASE WHEN \"LearningTrackId\" IS NULL THEN 0 ELSE 1 END" +
        " + CASE WHEN \"LearningModuleId\" IS NULL THEN 0 ELSE 1 END) = 1";
}
