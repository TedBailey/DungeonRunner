using System.Threading.Tasks;
using Example.MassTransit;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Example.Consumers;

public class DungeonCompletedConsumer : IConsumer<DungeonCompleted>
{
    private readonly ILogger<DungeonCompletedConsumer> _logger;

    public DungeonCompletedConsumer(
        ILogger<DungeonCompletedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DungeonCompleted> context)
    {
        // not currently doing anything!
    }
}