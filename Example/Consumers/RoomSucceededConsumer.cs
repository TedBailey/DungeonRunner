using System.Threading.Tasks;
using Example.Dungeoneer;
using Example.MassTransit;
using MassTransit;

namespace Example.Consumers;

public class RoomSucceededConsumer : IConsumer<RoomSucceeded>
{
    private readonly IDungeoneerService _dungeoneerService;

    public RoomSucceededConsumer(IDungeoneerService dungeoneerService)
    {
        _dungeoneerService = dungeoneerService;
    }
    
    public Task Consume(ConsumeContext<RoomSucceeded> context)
    {
        var dungeoneer = _dungeoneerService.GetDungeoneer(context.Message.DungeoneerId);

        if (dungeoneer is not Adventurer adventurer)
        {
            return Task.CompletedTask;
        }
        
        adventurer.AddGold(10);
        _dungeoneerService.UpdateDungeoneer(adventurer);
        
        return Task.CompletedTask;
    }
}