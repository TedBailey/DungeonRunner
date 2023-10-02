using OneOf;

namespace Example.Room;

public class RoomService
{
    public OneOf<RoomSuccess, RoomFailure, RoomInternalError> BeginLooting(IRoom room)
    {
        return room.Loot();
    }
}