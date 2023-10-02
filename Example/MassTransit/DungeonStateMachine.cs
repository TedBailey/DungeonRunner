using System;
using System.Collections.Generic;
using Example.Room;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Example.MassTransit;

public class DungeonStateMachine : MassTransitStateMachine<DungeonState>
{
    public DungeonStateMachine(ILogger<DungeonStateMachine> logger)
    {
        InstanceState(x => x.State, Exploring!, Looting!, Attempted!);

        // Since DungeonId is a Guid we can use this for event correlation
        Event(() => BeginDungeon, x =>
            x.CorrelateById(context => context.Message.DungeonId));
        Event(() => EnterRoom, x =>
            x.CorrelateById(context => context.Message.DungeonId));
        Event(() => RoomComplete, x =>
            x.CorrelateById(context => context.Message.DungeonId));
        Event(() => DungeonComplete, x =>
            x.CorrelateById(context => context.Message.DungeonId));

        Request(() => StartRoom, s => { s.Timeout = TimeSpan.FromDays(1); });

        Initially(
            When(BeginDungeon)
                .Then(thenContext => thenContext.Saga.Rooms =
                    CreateRooms(RoomStatus.Pending, thenContext.Message.RoomsToExplore))
                .IfElse(context => context.Saga.AllRoomsComplete,
                    thenContext => thenContext.TransitionTo(Attempted),
                    elseContext => elseContext
                        .Request(StartRoom,
                            x => new StartRoomRequest(x.Saga.CorrelationId, x.Saga.TryGetNextPendingRoom.Id)))
                .TransitionTo(Exploring));
        
        During(Exploring,
            When(StartRoom.Completed)
                .IfElse(context => context.Saga.AllRoomsComplete,
                    thenContext => thenContext.TransitionTo(Attempted),
                    elseContext => elseContext
                        // TODO: updating status doesn't actually work here
                        .Then(x => x.Saga.SetRoomStatus(x.Saga.TryGetNextPendingRoom.Id, RoomStatus.Success))
                        .Request(StartRoom,
                            x => new StartRoomRequest(x.Saga.CorrelationId,
                                x.Saga.TryGetNextPendingRoom.Id))),
            When(StartRoom.Faulted)
                // TODO: Publish that a room has failed!
                .TransitionTo(Attempted));


        During(Looting,
            When(RoomComplete)
                .IfElse(context => context.Saga.AllRoomsComplete,
                    thenContext => thenContext
                        .TransitionTo(Attempted),
                    elseContext => elseContext
                        // TODO: Incorrect publish, check UML! :)
                        .Publish(context => new RoomSucceeded(
                            context.CorrelationId ?? throw new NullReferenceException("Yikes no correlationId?"),
                            context.Saga.State,
                            context.Saga.DungeoneerId))
                        .TransitionTo(Exploring)));


        // TODO: Look into activities
        // When the dungeon is deemed attempted, publish that we have completed the dungeon
        WhenEnter(Attempted,
            context => context.Publish(DungeonComplete));
        DuringAny(
            When(DungeonComplete)
                .Then(context =>
                {
                    logger.LogInformation("{correlationId} | Dungeon complete!", context.CorrelationId);
                })
                .Finalize());

        // Make sure to ignore events that do not match a dungeon's state
        During(Exploring,
            Ignore(BeginDungeon));
        During(Exploring,
            Ignore(EnterRoom));
        During(Attempted,
            Ignore(BeginDungeon));
    }

    // TODO: cleanup some of these events and their usages.
    public Event<IBeginDungeon> BeginDungeon { get; }
    public Event<EnterRoom> EnterRoom { get; }
    public Event<RoomComplete> RoomComplete { get; }
    public Event<DungeonComplete> DungeonComplete { get; }

    /// <summary>
    /// A dungeon is currently being explored and the saga is awaiting
    /// results from the requested route.
    /// </summary>
    public State Exploring { get; set; } = null!;

    /// <summary>
    ///  
    /// </summary>
    public State Looting { get; set; } = null!;

    /// <summary>
    /// A dungeon has been attempted, successfully or not. 
    /// </summary>
    public State Attempted { get; set; } = null!;


    public Request<DungeonState, StartRoomRequest, RoomStartedResponse> StartRoom { get; private set; }


    /// <summary>
    /// Creates a dictionary of rooms all with a given status.
    /// </summary>
    private Dictionary<RoomStatus, List<IRoom>> CreateRooms(RoomStatus roomStatus, List<IRoom> rooms)
    {
        var roomsWithStatus = new Dictionary<RoomStatus, List<IRoom>> { { roomStatus, rooms } };
        return roomsWithStatus;
    }
}

public interface IBeginDungeon
{
    Guid DungeonId { get; }
    List<IRoom> RoomsToExplore { get; }
}

public interface DungeonComplete
{
    Guid DungeonId { get; }
}

public interface EnterRoom
{
    Guid DungeonId { get; }
    IRoom Room { get; }
}

public interface RoomComplete
{
    Guid DungeonId { get; }
    RoomStatus RoomStatus { get; }
}

public record RoomSucceeded(Guid DungeonId, int RoomNumber, string DungeoneerId);

public record StartRoomRequest(Guid DungeonId, Guid RoomId);

public record RoomStartedResponse(Guid DungeonId, Guid RoomId);