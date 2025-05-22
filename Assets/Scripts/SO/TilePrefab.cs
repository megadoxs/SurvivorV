using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TilePrefab", menuName = "2D/Tiles/Tile Prefab")]
public class TilePrefab : ScriptableObject
{
    public String description;
    public Sprite sprite;
    public TilePrefabType Type;
    public Direction[] Directions;
    public TileRow[] Rows;
    public Vector3Int[] AnchorPoints;
    public ItemStack[] cost;
}

[System.Serializable]
public class TileRow
{
    public TileBase[] Tiles;
}

[System.Serializable]
public enum Tilemaps //TODO remove from here
{
    Ground,
    Wall,
    Decoration,
    Map,
}
