using System;
using System.Collections.Generic;
using System.Linq;
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

        Request(() => StartRoom, s => { s.Timeout = TimeSpan.Zero; });

        Initially(
            When(BeginDungeon)
                .Then(thenContext => thenContext.Saga.Rooms =
                    CreateRooms(RoomStatus.Pending, thenContext.Message.RoomsToExplore))
                .If(context => context.Saga.AllRoomsComplete,
                    thenContext => thenContext.TransitionTo(Attempted))
                .TransitionTo(Exploring));

        WhenEnter(Exploring,
            enterContext => enterContext
                .IfElse(context => !context.Saga.AllRoomsComplete,
                    thenContext => thenContext.Request(StartRoom,
                        x => new StartRoomRequest(x.Saga.CorrelationId, x.Saga.GetNextPendingRoomId()))
                        .TransitionTo(Looting),
                    elseContext => elseContext.TransitionTo(Attempted)));

        During(Looting,
            When(StartRoom.Completed)
                .Then(x => x.Saga.SetRoomStatus(x.Message.RoomId, RoomStatus.Success))
                .IfElse(context => context.Saga.AllRoomsComplete,
                    thenContext => thenContext.TransitionTo(Attempted),
                    elseContext => elseContext.TransitionTo(Exploring)),
            When(StartRoom.Faulted)
                // TODO: Publish that a room has failed!
                // Find completed room, set status, allow for WhenEnter(Attempted) to do the publishing!
                .TransitionTo(Attempted),
            When(StartRoom.TimeoutExpired)
                // TODO: Allow for WhenEnter(Attempted) to do the publishing!
                .TransitionTo(Attempted));

        // TODO: When finished, publish out the message result
        //  Success results, failed, faulted, timeout, etc
        WhenEnter(Attempted,
            enterContext => enterContext
                .Publish(x => new DungeonCompleted(x.Saga.CorrelationId))
                .Then(context =>
                {
                    logger.LogInformation("{correlationId} | Dungeon complete!", context.CorrelationId);
                })
                .Finalize());

        // Make sure to ignore events that do not match a dungeon's state
        During(Exploring,
            Ignore(BeginDungeon));
        During(Looting,
            Ignore(BeginDungeon));
        During(Attempted,
            Ignore(BeginDungeon));
    }

    public Event<BeginDungeon> BeginDungeon { get; }

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


    public Request<DungeonState, StartRoomRequest, RoomCompletedResponse> StartRoom { get; private set; }


    /// <summary>
    /// Creates a dictionary of rooms all with a given status.
    /// </summary>
    private static Dictionary<Guid, RoomStatus> CreateRooms(RoomStatus roomStatus, IEnumerable<IRoom> rooms)
    {
        return rooms.ToDictionary(room => room.Id, room => roomStatus);
    }
}

public interface BeginDungeon
{
    Guid DungeonId { get; }
    IEnumerable<BaseRoom> RoomsToExplore { get; }
}

public record BeginDungeonAccepted(Guid DungeonId);
public record RoomSucceeded(Guid DungeonId, int RoomNumber, string DungeoneerId);

public record StartRoomRequest(Guid DungeonId, Guid RoomId);

/// <summary>
/// The room has actually completed.
/// </summary>
public record RoomCompletedResponse(Guid DungeonId, Guid RoomId);

/// <summary>
/// The room has been started.
/// </summary>
public record RoomStartedResponse(Guid DungeonId, Guid RoomId);

public record DungeonCompleted(Guid DungeonId);