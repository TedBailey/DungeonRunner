#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using OneOf;

namespace Example.Room;

public interface IRoomService
{
    public IRoom? TryGetRoom(Guid roomId);
    public IRoom AddRoom(IRoom room);
    public IEnumerable<IRoom> AddRooms(IEnumerable<IRoom> rooms);
    public OneOf<RoomSuccess, RoomFailure, RoomInternalError> BeginLooting(Guid roomId);
}

public class RoomService : IRoomService
{
    private readonly List<IRoom> _rooms = new();

    public IRoom? TryGetRoom(Guid roomId)
    {
        return _rooms.SingleOrDefault(x => x.Id == roomId);
    }

    public IRoom AddRoom(IRoom room)
    {
        _rooms.Add(room);
        return room;
    }

    public IEnumerable<IRoom> AddRooms(IEnumerable<IRoom> rooms)
    {
        var addRooms = rooms.ToList();
        _rooms.AddRange(addRooms);
        return addRooms;
    }

    public OneOf<RoomSuccess, RoomFailure, RoomInternalError> BeginLooting(Guid roomId)
    {
        var room = _rooms.SingleOrDefault(x => x.Id == roomId);
        return room?.Loot() ?? new RoomInternalError(new Exception($"Room {roomId} could not be found!"));
    }
}