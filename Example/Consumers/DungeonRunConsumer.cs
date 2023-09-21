using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Example.Consumers;

public class DungeonRunConsumer : IConsumer<Contracts.DungeonRun>
{
    private readonly ILogger<DungeonRunConsumer> _logger;
    private readonly IDungeonService _dungeonService;

    public DungeonRunConsumer(
        ILogger<DungeonRunConsumer> logger,
        IDungeonService dungeonService
        )
    {
        _logger = logger;
        _dungeonService = dungeonService;
    }
    
    public Task Consume(ConsumeContext<Contracts.DungeonRun> context)
    {
        var dungeoneer = _dungeonService.GetDungeoneer(context.Message.Dungeoneer.Id);
        
        _logger.LogInformation("{correlationId} | Received Dungeoneer: {name}, Health {health}",
            context.CorrelationId,
            dungeoneer.Name,
            dungeoneer.Health);

        return Task.CompletedTask;
    }
}