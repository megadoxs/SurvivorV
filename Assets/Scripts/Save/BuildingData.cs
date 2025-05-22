using System;
using UnityEngine;

[Serializable]
public class BuildingData
{
    public string type;
    public Vector3Int position;
    
    public BuildingData(string type, Vector3Int position)
    {
        this.type = type;
        this.position = position;
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