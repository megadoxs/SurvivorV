using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Pathfinding;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class DungeonManager : MonoBehaviour, IDungeonDataSaver //TODO this system of adding groups of tiles to a tilemap is so bad, this can't be the best way to do it
{
    [Header("Dungeon Generation")]
    [SerializeField] 
    private List<TilePrefab> rooms;
    [SerializeField]
    private List<TilePrefab> hallways;
    
    [SerializeField] 
    private Tilemap tilemapCollisionObject;
    [SerializeField]
    private Tilemap tilemapGroundTop;
    [SerializeField]
    private Tilemap tilemapGround;
    [SerializeField]
    private Tilemap tilemapMap;
    
    [SerializeField]
    private AstarPath astarPath;
    
    [SerializeField] 
    private GameObject buttonPrefab;
    [SerializeField] 
    private GameObject buttonList;

    [SerializeField] 
    private GameObject buildingButtonPrefab;
    
    private Dictionary<TileBase, TileBase> mapTiles;
    private Dictionary<TileBase, Tilemap> tilesTilemaps;
    private Dictionary<TileBase, TileBase[]> tileOverrides;
    
    public static DungeonManager instance;

    private List<DungeonExtension> extensions = new();

    private void Awake()
    {
        instance = this;
        
        mapTiles = new Dictionary<TileBase, TileBase>();
    
        var MapTiles = Resources.LoadAll<MapTile>("Dungeon");

        foreach (var map in MapTiles)
        {
            foreach (var mapTile in map.Tiles)
            {
                if (!mapTiles.ContainsKey(mapTile))
                    mapTiles[mapTile] = map.Tile;
            }
        }
        
        tilesTilemaps = new Dictionary<TileBase, Tilemap>();
    
        var tilemapTile = Resources.LoadAll<TilemapTile>("Dungeon");

        foreach (var map in tilemapTile)
        {
            foreach (var tile in map.Tiles)
            {
                if (!tilesTilemaps.ContainsKey(tile))
                    tilesTilemaps[tile] = map.Tilemap switch
                    {
                        Tilemaps.Ground => tilemapGround,
                        Tilemaps.Wall => tilemapCollisionObject,
                        _ => null
                    };
            }
        }
        
        tileOverrides = new Dictionary<TileBase, TileBase[]>();
        
        var tileOverride = Resources.LoadAll<TileOverride>("Dungeon");
        foreach (var overrideTile in tileOverride)
        {
            if (!tileOverrides.ContainsKey(overrideTile.Tile))
                tileOverrides[overrideTile.Tile] = overrideTile.OverrideTiles;
        }
    }

    public void ShowBuildingMenu(Vector3Int anchor, TilePrefabType type, Direction direction, GameObject button) //TODO map is in the way
    {
        var buildingMenu = BuildManager.instance.GetBuildMenu();
        var container = buildingMenu.transform.GetChild(0);
        
        for (var i = 0; i < 2; i++)
        {
            for (var j = 0; j < container.GetChild(i).childCount; j++)
            {
                var building = container.GetChild(i).GetChild(j).gameObject;
            
                var sourceList = type == TilePrefabType.Room ? rooms : hallways;
                var tilePrefab = sourceList.Find(x => x.Directions.Contains(direction) && x == building.GetComponent<BuildingButton>().building);

                if (tilePrefab != null)
                {
                    building.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        GenerateDungeon(tilePrefab, anchor, direction);
                        Inventory.instance.RemoveItems(tilePrefab.cost.Select(stack => stack.Clone()).ToArray());
                        BuildManager.instance.CancelBuilding();
                        Destroy(button);
                    });
                    var image = building.transform.GetChild(1).GetComponent<Image>();
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
                }
                else
                {    
                    building.GetComponent<Button>().onClick.RemoveAllListeners();
                
                    var image = building.transform.GetChild(1).GetComponent<Image>();
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 0.8f);
                }
            }
        }
        BuildManager.instance.MapBuild();
    }

    private void GenerateDungeon(TilePrefab tilePrefab, Vector3Int anchor, Direction direction) //TODO if two structures have the same width next to each other, will cause issue
    {
        extensions.Add(new DungeonExtension(tilePrefab.name, anchor, direction));
        
        
        List<Vector3Int> grounds = new List<Vector3Int>();
        Dictionary<Vector3Int, TileBase> wallOverrides = new Dictionary<Vector3Int, TileBase>();
        var width = tilePrefab.Rows[0].Tiles.Length;
        var height = tilePrefab.Rows.Length;
        
        var position = anchor + direction switch
        {
            Direction.Up => new Vector3Int(-width/2, height - 1),
            Direction.Down => new Vector3Int(-width/2, 0),
            Direction.Left => new Vector3Int(-(width - 1), height/2),
            Direction.Right => new Vector3Int(0, height/2),
            _ => Vector3Int.zero
        };
        
        if (direction == Direction.Down)
        {
            for (var x = 0; x < width; x++)
            {
                if (tilemapGround.HasTile(position + new Vector3Int(x, 1)))
                { 
                    if(x == 0 || x == width - 1) 
                        wallOverrides.Add(position + new Vector3Int(x, 0), tileOverrides.GetValueOrDefault(tilePrefab.Rows[0].Tiles[x])[0]);
                    else
                        grounds.Add(position + new Vector3Int(x, 0));
                }
            }
        }
        else if (direction == Direction.Up)
        {
            for (var x = 0; x < width; x++)
            {
                if (tilemapGround.HasTile(position + new Vector3Int(x, -height)))
                {
                    if (x == 0 || x == width - 1)
                        wallOverrides.Add(position + new Vector3Int(x, -height + 1), tileOverrides.GetValueOrDefault(tilePrefab.Rows[height - 1].Tiles[x])[0]);
                    else
                        grounds.Add(position + new Vector3Int(x, -height + 1));
                }
                else if (tilemapCollisionObject.HasTile(position + new Vector3Int(x, -height)))
                    wallOverrides.Add(position + new Vector3Int(x, -height + 1), tileOverrides.GetValueOrDefault(tilePrefab.Rows[height - 1].Tiles[x])[x < width / 2 ? 0 : 1]);
            }
        }
        else if (direction == Direction.Left)
        {
            for (var y = 0; y < height; y++)
            {
                if (tilemapGround.HasTile(position + new Vector3Int(width, -y)))
                {
                    if (y == 0 || y == height - 1)
                        wallOverrides.Add(position + new Vector3Int(width - 1, -y), tileOverrides.GetValueOrDefault(tilePrefab.Rows[y].Tiles[width-1])[1]);
                    else
                        grounds.Add(position + new Vector3Int(width - 1, -y));
                }
                else if (tilemapCollisionObject.HasTile(position + new Vector3Int(width, -y)))
                    wallOverrides.Add(position + new Vector3Int(width - 1, -y), tileOverrides.GetValueOrDefault(tilePrefab.Rows[y].Tiles[width-1])[y < height / 2 ? 0 : 1]);
            }
        }
        else if (direction == Direction.Right)
        {
            for (var y = 0; y < height; y++)
            {
                if (tilemapGround.HasTile(position + new Vector3Int(-1, -y)))
                {
                    if (y == 0 || y == height - 1)
                        wallOverrides.Add(position + new Vector3Int(0, -y), tileOverrides.GetValueOrDefault(tilePrefab.Rows[y].Tiles[0])[1]);
                    else    
                        grounds.Add(position + new Vector3Int(0, -y));
                }
                else if (tilemapCollisionObject.HasTile(position + new Vector3Int(-1, -y)))
                    wallOverrides.Add(position + new Vector3Int(0, -y), tileOverrides.GetValueOrDefault(tilePrefab.Rows[y].Tiles[0])[y < height / 2 ? 0 : 1]);
            }
        }
        
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var tile = tilePrefab.Rows[y].Tiles[x];
                Tilemap tilemap = tilesTilemaps.GetValueOrDefault(tile);
                
                var tilePosition = position + new Vector3Int(x, -y);

                if (grounds.Contains(tilePosition))
                {
                    tile = tilePrefab.Rows[height/2].Tiles[width/2];
                    tilemapCollisionObject.SetTile(tilePosition, null);
                    tilemap = tilemapGround;
                }
                else if (wallOverrides.ContainsKey(tilePosition))
                {
                    tile = wallOverrides[tilePosition];
                }
                
                tilemap.SetTile(tilePosition, tile);
                tilemapMap.SetTile(tilePosition, mapTiles.GetValueOrDefault(tile));
            }
        }

        tilemapCollisionObject.GetComponent<TilemapCollider2D>().enabled = false;
        tilemapCollisionObject.GetComponent<TilemapCollider2D>().enabled = true;
        tilemapCollisionObject.CompressBounds();
        astarPath.graphs[0].active.data.gridGraph.SetDimensions(tilemapCollisionObject.size.x, tilemapCollisionObject.size.y, 0.16F);
        astarPath.graphs[0].active.data.gridGraph.center = tilemapCollisionObject.localBounds.center;
        astarPath.Scan();

        foreach (var point in tilePrefab.AnchorPoints)
        {
            Vector3Int buttonPosition = position + new Vector3Int(point.x, -point.y);
            var offset = 3;
            Direction buttonDirection;

            if (point.x == 0)
            {
                if (direction == Direction.Right)
                    continue;
                buttonPosition += new Vector3Int(-offset*3, 0);
                buttonDirection = Direction.Left;
            }
            else if (point.x == width - 1)
            {
                if (direction == Direction.Left)
                    continue;
                buttonPosition += new Vector3Int(offset*3, 0);
                buttonDirection = Direction.Right;
            }
            else if (point.y == 0)
            {
                if (direction == Direction.Down)
                    continue;
                buttonPosition += new Vector3Int(0, offset);
                buttonDirection = Direction.Up;
            }
            else if (point.y == height - 1)
            {
                if (direction == Direction.Up)
                    continue;
                buttonPosition += new Vector3Int(0, -offset);
                buttonDirection = Direction.Down;
            }
            else continue;
            
            var button = Instantiate(buttonPrefab, tilemapMap.CellToWorld(buttonPosition) + new Vector3(tilemapMap.cellSize.x/2, tilemapMap.cellSize.y/2), Quaternion.identity, buttonList.transform);
            var marker = button.GetComponent<MapMarker>();
            marker.direction = buttonDirection;
            marker.anchor = position + new Vector3Int(point.x, -point.y);
            marker.type = tilePrefab.Type switch
            {
                TilePrefabType.Hallway => TilePrefabType.Room,
                TilePrefabType.Room => TilePrefabType.Hallway,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public void LoadData(DungeonData data) //TODO buttons of the placed extension are is not deleted
    {
        if(data.extensions.Count > 0)
            Destroy(buttonList.transform.GetChild(0).gameObject);
        
        data.extensions.ForEach(x => GenerateDungeon(x.GetPrefab(), x.position, x.direction));
        
        for (int i = buttonList.transform.childCount - 1; i >= 0; i--) //test
        {
            var child = buttonList.transform.GetChild(i);
            var marker = child.GetComponent<MapMarker>();

            foreach (var extension in data.extensions)
            {
                if (extension == null)
                    continue;

                if (marker.anchor == extension.position)
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }
    }

    public void SaveData(ref DungeonData data)
    {
        data.extensions.Clear();
        foreach (var x in extensions)
            data.extensions.Add(x);
    }
}
