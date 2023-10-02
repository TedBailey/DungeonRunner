using System;

namespace Example.Contracts;

public record DungeonRun
{
    public DateTime RunStartDateTime { get; } = DateTime.Now;
    public Dungeoneer.Dungeoneer Dungeoneer { get; init; }
    public int NumberOfRooms { get; set; }
}