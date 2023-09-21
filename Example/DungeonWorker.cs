using System;
using System.Threading;
using System.Threading.Tasks;
using Example.Models;
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
            
            await _bus.Publish(new Contracts.DungeonRun
            {
                Dungeoneer = adventurer,
            }, cancellationToken);

            await Task.Delay(2000, cancellationToken);
        }
    }
}