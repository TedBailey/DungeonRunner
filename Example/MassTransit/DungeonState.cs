using System;
using System.Collections.Generic;
using System.Linq;
using Example.Room;
using MassTransit;

namespace Example.MassTransit;

public class DungeonState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string DungeoneerId { get; set; }
    public int State { get; set; }
    public int NumberOfRooms { get; set; }

    public bool AllRoomsComplete => Rooms.Any(x => x.Key is not (RoomStatus.Success or RoomStatus.Failure));

    public IRoom TryGetNextPendingRoom => Rooms[RoomStatus.Pending][0];

    public void SetRoomStatus(Guid roomId, RoomStatus status)
    {
        /*var room = Rooms.Single(x => x.Value.First(y => y.Id == roomId));
        Rooms[status].Remove(room);
        Rooms*/
    }

    // Rooms can have a status to them
    public Dictionary<RoomStatus, List<IRoom>> Rooms { get; set; }
}