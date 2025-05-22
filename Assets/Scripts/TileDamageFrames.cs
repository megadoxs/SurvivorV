using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class TileDamageFrames //TODO Should be made a scriptable Object, also add damage
{
    public TileBase Tile;
    public Sprite[] Frames;
}