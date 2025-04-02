using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string sceneName;

    public Vector3 position;
    // public float health;
    public List<ItemStackData> items;
    //save stats

    public PlayerData() //don't convert, serializable won't work.
    {
        sceneName = "DungeonScene";
        position = Vector3.zero;
        items = new List<ItemStackData>();
    }
}
