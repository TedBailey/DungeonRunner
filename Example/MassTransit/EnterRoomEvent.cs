using System;
using Example.Room;

namespace Example.MassTransit;

public class EnterRoomEvent : EnterRoom
{
    public EnterRoomEvent(Guid dungeonId, int roomNumber)
    {
        DungeonId = dungeonId;
        RoomNumber = roomNumber;
    }
    
    public Guid DungeonId { get; }
    public IRoom Room { get; }
    public int RoomNumber { get; }
}