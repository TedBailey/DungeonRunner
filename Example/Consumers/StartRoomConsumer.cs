using System;
using System.Threading.Tasks;
using Example.MassTransit;
using Example.Room;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Example.Consumers;

public class StartRoomConsumer : IConsumer<StartRoomRequest>
{
    private readonly ILogger<StartRoomConsumer> _logger;
    private readonly IRoomService _roomService;
    private readonly IBus _bus;

    public StartRoomConsumer(
        ILogger<StartRoomConsumer> logger,
        IRoomService roomService,
        IBus bus)
    {
        _logger = logger;
        _roomService = roomService;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<StartRoomRequest> context)
    {
        var room = _roomService.TryGetRoom(context.Message.RoomId);
        if (room is null)
        {
            throw new Exception(
                $"Something has gone wrong, the room {context.Message.RoomId} has not been registered.");
        }
        
        _logger.LogInformation("Start room has been requested, Dungeon: {DungeonId}, Room: {RoomName} {RoomId}",
            context.Message.DungeonId, room.Name, context.Message.RoomId);
        
        //await context.RespondAsync(new RoomStartedResponse(context.Message.DungeonId, context.Message.RoomId));
        
        // TODO: Publish begin looting state
        //await _bus.Publish<RoomStarted>();

        await Task.Delay(1000);
        
        await context.RespondAsync(new RoomCompletedResponse(context.Message.DungeonId, context.Message.RoomId));
    }
}