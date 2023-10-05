using System;
using System.Collections.Generic;
using System.Linq;
using Example.Room;
using MassTransit;
using OneOf;

namespace Example.MassTransit;

public class DungeonState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string DungeoneerId { get; set; }
    public int State { get; set; }

    public bool AllRoomsComplete => Rooms.All(x => x.Value is (RoomStatus.Success or RoomStatus.Failure));

    public Guid GetNextPendingRoomId()
    {
        var pendingRooms = Rooms
            .Where(room => room.Value == RoomStatus.Pending)
            .ToList();

        return pendingRooms.First().Key;
    }
    
    public Dictionary<Guid, RoomStatus> Rooms { get; set; }

    public void SetRoomStatus(Guid roomId, RoomStatus status)
    {
        Rooms[roomId] = status;
    }
}