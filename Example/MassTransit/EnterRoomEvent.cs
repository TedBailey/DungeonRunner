using System;

namespace Example.MassTransit;

public class EnterRoomEvent : EnterRoom
{
    public EnterRoomEvent(Guid dungeonId, int roomNumber)
    {
        DungeonId = dungeonId;
        RoomNumber = roomNumber;
    }
    
    public Guid DungeonId { get; }
    public int RoomNumber { get; }
}