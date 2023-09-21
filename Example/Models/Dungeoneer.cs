using OneOf;

namespace Example.Models;

public class Dungeoneer
{
    public int Health { get; private set; }
    public string Name { get; init; }
    public string Id { get; init; }

    protected OneOf<Alive, Unalived> AddHealth(int health)
    {
        Health += health;

        if (Health >= 0)
        {
            return new Alive();
        }
        
        Health = 0;
        return new Unalived();
    }
}

public record Alive();
public record Unalived();