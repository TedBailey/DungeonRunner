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

    internal OneOf<RoomSuccess, RoomFailure, RoomInternalError> Loot();
}

public class BaseRoom : IRoom
{
    public BaseRoom()
    {
        Id = Guid.NewGuid();
    }
    
    public Guid Id { get; init; }
    
    public OneOf<RoomSuccess, RoomFailure, RoomInternalError> Loot()
    {
        return new RoomSuccess();
    }
}

public record RoomSuccess();
public record RoomFailure(string Reason);
public record RoomInternalError(Exception Exception);