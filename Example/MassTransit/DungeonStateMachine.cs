using System;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Example.MassTransit;

public class DungeonStateMachine : MassTransitStateMachine<DungeonState>
{
    public DungeonStateMachine(ILogger<DungeonStateMachine> logger)
    {
        InstanceState(x => x.CurrentRoomNumber);

        // Since DungeonId is a Guid we can use this for event correlation
        Event(() => BeginDungeon, x => 
            x.CorrelateById(context => context.Message.DungeonId));
        
        Initially(
            When(BeginDungeon)
                .If(context => context.Saga.AllRoomsComplete,
                    thenContext => thenContext.TransitionTo(Attempted))
                .TransitionTo(Exploring));
        
        During(Exploring,
            When(EnterRoom)
                .Publish(RoomComplete)
                .TransitionTo(Exploring));
        
        // Ensure when all have been completed, finish the dungeon
        DuringAny(
            When(RoomComplete)
                .IfElse(context => context.Saga.AllRoomsComplete,
                    thenContext => thenContext
                        .TransitionTo(Attempted),
                    elseContext => elseContext
                        // TODO: Incorrect publish, check UML! :)
                        .Publish(context => new RoomSucceeded(
                            context.CorrelationId ?? throw new NullReferenceException("Yikes no correlationId?"),
                            context.Saga.CurrentRoomNumber,
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
    
    public Event<BeginDungeon> BeginDungeon { get; private set; }
    public Event<EnterRoom> EnterRoom { get; private set; }
    public Event<RoomComplete> RoomComplete { get; private set; }
    public Event<DungeonComplete> DungeonComplete { get; private set; }
    
    /// <summary>
    /// A dungeon is currently being explored and the saga is awaiting
    /// results from the requested route.
    /// </summary>
    public State Exploring => null!;

    /// <summary>
    ///  
    /// </summary>
    public State Looting => null!;
    
    /// <summary>
    /// A dungeon has been attempted, successfully or not. 
    /// </summary>
    public State Attempted => null!;
}

public interface BeginDungeon
{
    Guid DungeonId { get; }
}

public interface DungeonComplete
{
    Guid DungeonId { get; }
}

public interface EnterRoom
{
    Guid DungeonId { get; }
}

public interface RoomComplete
{
    Guid DungeonId { get; }
}

public record RoomSucceeded(Guid DungeonId, int RoomNumber, string DungeoneerId);

