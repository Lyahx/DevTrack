namespace DevTrack.Domain.Entities;

public class Worklog : BaseOwnedEntity
{
    public string WhatIDid { get; set; } = string.Empty;
    public string? WhatsLeft { get; set; }
    public DateTime LoggedAt { get; set; }
}
