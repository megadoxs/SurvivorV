using System;
using System.Collections.Generic;

[Serializable]
public class DungeonData
{
    public float dungeonHeartHealth;
    public float dungeonHeartMaxHealth;
    
    public float waveTimer;
    public int waveCount;
    public int monsterSpawned;
    
    public List<EntityData> entities;
    public List<BuildingData> buildings;
    public List<DungeonExtension>extensions;
    
    public DungeonData()
    {
        dungeonHeartHealth = 100;
        dungeonHeartMaxHealth = 100;
        waveTimer = 100;
        waveCount = 0;
        entities = new List<EntityData>();
        buildings = new List<BuildingData>();
        extensions = new List<DungeonExtension>();
    }
}
