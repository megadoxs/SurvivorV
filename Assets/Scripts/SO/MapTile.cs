using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "MapTile", menuName = "2D/Tiles/Map Tile")]
public class MapTile : ScriptableObject
{
    public TileBase Tile;
    public TileBase[] Tiles;
}
