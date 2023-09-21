using System;
using MassTransit;

namespace Example.MassTransit;

public class DungeonState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string DungeoneerId { get; set; }
    public int CurrentRoomNumber { get; set; }
    public int NumberOfRooms { get; set; }

    public bool AllRoomsComplete => CurrentRoomNumber >= NumberOfRooms;
}