using System;
using UnityEngine;

[Serializable]
public class DungeonExtension
{
    public String type;
    public Vector3Int position;
    public Direction direction;

    public DungeonExtension(String type, Vector3Int position, Direction direction)
    {
        this.type = type;
        this.position = position;
        this.direction = direction;
    }
    
    public TilePrefab GetPrefab()
    {
        TilePrefab[] allPrefabs = Resources.LoadAll<TilePrefab>("");
        foreach (var prefab in allPrefabs)
        {
            if (prefab.name == type)
                return prefab;
        }

        return null;
    }
}