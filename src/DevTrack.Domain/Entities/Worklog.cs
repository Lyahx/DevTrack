namespace DevTrack.Domain.Entities;

public class Worklog : BaseOwnedEntity
{
    public string WhatIDid { get; set; } = string.Empty;
    public string? WhatsLeft { get; set; }
    // Optional reasoning — used when this worklog is also a "decision":
    // explains why the user chose this approach. Carries the columns of the
    // former Decision entity, which has been collapsed into Worklog.
    public string? Reasoning { get; set; }
    public string? Alternatives { get; set; }
    public DateTime LoggedAt { get; set; }
}
