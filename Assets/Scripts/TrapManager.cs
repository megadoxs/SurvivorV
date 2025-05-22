using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrapManager : MonoBehaviour
{
    [SerializeField] 
    private TileDamageFrames[] traps;

    [SerializeField] 
    private Tilemap tilemap;
    
    public static TrapManager instance;
    
    private Dictionary<TileBase, List<Vector3Int>> tiles = new();
    private Dictionary<Vector3Int, HashSet<Entity>> damagedEntities = new();
    
    private void Awake()
    {
        instance = this;   
    }
    
    private void Start()
    {
        RefreshTiles();
    }


    void Update()
    {
        foreach (var trap in traps)
        {
            foreach (var trapPos in tiles[trap.Tile])
            {
                var currentSprite = tilemap.GetSprite(trapPos);

                if (trap.Frames.Contains(currentSprite))
                {
                    if (!damagedEntities.ContainsKey(trapPos))
                        damagedEntities[trapPos] = new HashSet<Entity>();

                    var hits = Physics2D.OverlapBoxAll(tilemap.GetCellCenterWorld(trapPos), tilemap.cellSize, 0f);
                    foreach (var hit in hits)
                    {
                        var entity = hit.GetComponent<Entity>();
                        if (entity != null && !damagedEntities[trapPos].Contains(entity))
                        {
                            entity.Damage(1);
                            damagedEntities[trapPos].Add(entity);
                        }
                    }
                }
                else
                {
                    if (damagedEntities.ContainsKey(trapPos))
                        damagedEntities[trapPos].Clear();
                }
            }
        }
    }

    public void RefreshTiles()
    {
        foreach (var trap in traps)
        {
            var foundTiles = new List<Vector3Int>();
        
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                var tile = tilemap.GetTile(pos);
                if (tile == trap.Tile)
                {
                    foundTiles.Add(pos);
                }
            }
        
            tiles[trap.Tile] = foundTiles;
        }
    }
}
