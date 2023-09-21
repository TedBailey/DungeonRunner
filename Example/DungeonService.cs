using System;
using System.Collections.Generic;
using Example.Models;

namespace Example;

public interface IDungeonService
{
    void AddDungeoneer(Dungeoneer dungeoneer);
    Dungeoneer GetDungeoneer(string id);
    Dungeoneer UpdateDungeoneer(Dungeoneer dungeoneer);
}

public class DungeonService : IDungeonService
{
    private readonly Dictionary<string, Dungeoneer> _dungeoneers = new();

    public void AddDungeoneer(Dungeoneer dungeoneer)
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

    public Dungeoneer GetDungeoneer(string id)
    {
        if (_dungeoneers.TryGetValue(id, out var dungeoneer))
        {
            return dungeoneer;
        }

        throw new InvalidOperationException($"Could not find dungeoneer with id: {id}");
    }
    
    public Dungeoneer UpdateDungeoneer(Dungeoneer dungeoneer)
    {
        if (_dungeoneers.TryGetValue(dungeoneer.Id, out var found))
        {
            _dungeoneers[dungeoneer.Id] = found;
            return dungeoneer;
        }

        throw new InvalidOperationException($"Could not update dungeoneer with id: {dungeoneer.Id}");
    }
}