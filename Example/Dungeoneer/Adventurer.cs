using System;

namespace Example.Dungeoneer;

public class Adventurer : Dungeoneer
{
    private int Gold;
    
    public Adventurer()
    {
        Name = $"Adventurer - {Guid.NewGuid()}";
        Id = Guid.NewGuid().ToString();
        Gold = 0;
        _ = AddHealth(20);
    }

    public void AddGold(int gold) => Gold += gold;
}