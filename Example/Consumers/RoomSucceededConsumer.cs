using System.Threading.Tasks;
using Example.MassTransit;
using Example.Models;
using MassTransit;

namespace Example.Consumers;

public class RoomSucceededConsumer : IConsumer<RoomSucceeded>
{
    private readonly IDungeonService _dungeonService;

    public RoomSucceededConsumer(IDungeonService dungeonService)
    {
        _dungeonService = dungeonService;
    }
    
    public Task Consume(ConsumeContext<RoomSucceeded> context)
    {
        var dungeoneer = _dungeonService.GetDungeoneer(context.Message.DungeoneerId);

        if (dungeoneer is not Adventurer adventurer)
        {
            return Task.CompletedTask;
        }
        
        adventurer.AddGold(10);
        _dungeonService.UpdateDungeoneer(adventurer);
        
        // Go to the next room!
        context.Publish(new EnterRoomEvent(context.Message.DungeonId, context.Message.RoomNumber + 1));

        return Task.CompletedTask;
    }
}