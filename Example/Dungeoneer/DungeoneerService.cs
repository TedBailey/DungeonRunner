using System;
using System.Collections.Generic;

namespace Example.Dungeoneer;

public interface IDungeoneerService
{
    void AddDungeoneer(Example.Dungeoneer.Dungeoneer dungeoneer);
    Example.Dungeoneer.Dungeoneer GetDungeoneer(string id);
    Example.Dungeoneer.Dungeoneer UpdateDungeoneer(Example.Dungeoneer.Dungeoneer dungeoneer);
}

public class DungeoneerService : IDungeoneerService
{
    private readonly Dictionary<string, Example.Dungeoneer.Dungeoneer> _dungeoneers = new();

    public void AddDungeoneer(Example.Dungeoneer.Dungeoneer dungeoneer)
    {
        _dungeoneers.Add(dungeoneer.Id, dungeoneer);
    }
    
    public void RemoveDungeoneer(string id)
    {
        var removeSuccessful = _dungeoneers.Remove(id);
        
        if (removeSuccessful is not true)
        {
            throw new InvalidOperationException($"Could not remove dungeoneer with id: {id}");
        }
    }

    public Example.Dungeoneer.Dungeoneer GetDungeoneer(string id)
    {
        if (_dungeoneers.TryGetValue(id, out var dungeoneer))
        {
            return dungeoneer;
        }

        throw new InvalidOperationException($"Could not find dungeoneer with id: {id}");
    }
    
    public Example.Dungeoneer.Dungeoneer UpdateDungeoneer(Example.Dungeoneer.Dungeoneer dungeoneer)
    {
        if (_dungeoneers.TryGetValue(dungeoneer.Id, out var found))
        {
            _dungeoneers[dungeoneer.Id] = found;
            return dungeoneer;
        }

        throw new InvalidOperationException($"Could not update dungeoneer with id: {dungeoneer.Id}");
    }
}