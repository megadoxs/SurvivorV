using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TilemapTile", menuName = "2D/Tiles/Tilemap Tile")]
public class TilemapTile : ScriptableObject
{
    public Tilemaps Tilemap;
    public TileBase[] Tiles;
}
