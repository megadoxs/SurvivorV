using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileOverride", menuName = "2D/Tiles/Tile Override")]
public class TileOverride : ScriptableObject
{
    public TileBase Tile;
    public TileBase[] OverrideTiles;
}
