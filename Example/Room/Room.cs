using System;
using OneOf;

namespace Example.Room;

public enum RoomStatus
{
    Pending,
    Looting,
    Success,
    Failure,
}

public interface IRoom
{
    public Guid Id { get; init; }
    public string Name { get; init; }

    internal OneOf<RoomSuccess, RoomFailure, RoomInternalError> Loot();
}

public class BaseRoom : IRoom
{
    // Default constructor
    public BaseRoom()
    {
    }
    
    public BaseRoom(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
    
    public Guid Id { get; init; }
    public string Name { get; init; }
    
    public OneOf<RoomSuccess, RoomFailure, RoomInternalError> Loot()
    {
        return new RoomSuccess();
    }
}

public record RoomSuccess();
public record RoomFailure(string Reason);
public record RoomInternalError(Exception Exception);