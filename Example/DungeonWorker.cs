using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Example.Dungeoneer;
using Example.MassTransit;
using Example.Room;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Example;

public class DungeonWorker : BackgroundService
{
    private readonly ILogger<DungeonWorker> _logger;
    private readonly IBus _bus;
    private readonly IDungeoneerService _dungeoneerService;
    private readonly IRoomService _roomService;

    public DungeonWorker(
        ILogger<DungeonWorker> logger,
        IBus bus,
        IDungeoneerService dungeoneerService,
        IRoomService roomService
    )
    {
        _logger = logger;
        _bus = bus;
        _dungeoneerService = dungeoneerService;
        _roomService = roomService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var adventurer = new Adventurer();
            _dungeoneerService.AddDungeoneer(adventurer);

            var testRooms = new List<IRoom>
            {
                new BaseRoom("Basement"),
                new BaseRoom("Kitchen"),
                new BaseRoom("Dining room"),
            };

            _roomService.AddRooms(testRooms);

            var message = new { RoomsToExplore = testRooms, DungeonId = Guid.NewGuid() };
            _logger.LogInformation("Publishing {messageType} {dungeonId}", typeof(BeginDungeon), message.DungeonId);
            await _bus.Publish<BeginDungeon>(message, cancellationToken);

            // TODO: Think about how a Dungeoneer plays into published BeginDungeons
            /*await _bus.Publish(new Contracts.DungeonRun
            {
                Dungeoneer = adventurer,
            }, cancellationToken);*/

            await Task.Delay(1000, cancellationToken);
        }
    }
}