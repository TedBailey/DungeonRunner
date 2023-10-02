using System.Threading.Tasks;
using Example.MassTransit;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Example.Consumers;

public class StartRoomConsumer : IConsumer<StartRoomRequest>
{
    private readonly ILogger<StartRoomConsumer> _logger;

    public StartRoomConsumer(ILogger<StartRoomConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StartRoomRequest> context)
    {
        _logger.LogInformation("Start room has been requested, Dungeon: {DungeonId}, Room: {RoomId}",
            context.Message.DungeonId, context.Message.RoomId);

        await Task.Delay(1000);

        await context.RespondAsync(new RoomStartedResponse(context.Message.DungeonId, context.Message.RoomId));
    }
}