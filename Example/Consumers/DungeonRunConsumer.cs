using System.Threading.Tasks;
using Example.Dungeoneer;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Example.Consumers;

public class DungeonRunConsumer : IConsumer<Contracts.DungeonRun>
{
    private readonly ILogger<DungeonRunConsumer> _logger;
    private readonly IDungeoneerService _dungeoneerService;

    public DungeonRunConsumer(
        ILogger<DungeonRunConsumer> logger,
        IDungeoneerService dungeoneerService
        )
    {
        _logger = logger;
        _dungeoneerService = dungeoneerService;
    }
    
    public Task Consume(ConsumeContext<Contracts.DungeonRun> context)
    {
        var dungeoneer = _dungeoneerService.GetDungeoneer(context.Message.Dungeoneer.Id);
        
        _logger.LogInformation("{correlationId} | Received Dungeoneer: {name}, Health {health}",
            context.CorrelationId,
            dungeoneer.Name,
            dungeoneer.Health);

        return Task.CompletedTask;
    }
}