using System.Threading;
using System.Threading.Tasks;
using Example.Dungeoneer;
using Example.MassTransit;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace Example;

public class DungeonWorker : BackgroundService
{
    private readonly IBus _bus;
    private readonly IDungeonService _dungeonService;

    public DungeonWorker(
        IBus bus,
        IDungeonService dungeonService
        )
    {
        _bus = bus;
        _dungeonService = dungeonService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var adventurer = new Adventurer();
            _dungeonService.AddDungeoneer(adventurer);

            // TODO: Trouble getting this plugged together
            await _bus.Publish(new BeginDungeon());
            
            // TODO: Think about how a Dungeoneer plays into published BeginDungeons
            await _bus.Publish(new Contracts.DungeonRun
            {
                Dungeoneer = adventurer,
            }, cancellationToken);

            await Task.Delay(10000, cancellationToken);
        }
    }
}