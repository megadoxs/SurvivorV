using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildManager : MenuSwitcher, IDungeonDataSaver
{
    [SerializeField]
    private GameObject buildMenu;
    
    [SerializeField] 
    private TilePrefab spawnTile;

    [SerializeField] 
    private GameObject spawnTilePrefab;

    private Tilemap tilemapGroundTop;
    private Tilemap tilemapCollisonObject;
    private Tilemap tilemapGround;
    private Transform environment;
    public bool buildMode;

    private TileBase selectedTile;
    private Vector3Int? previewPosition;
    private Vector3Int? initialPosition;
    private List<ItemStack> cost = new();
    
    public static BuildManager instance { get; private set; }
    
    private Dictionary<Vector3Int, TilePrefab> placedBuildings = new();

    private void Awake()
    {
        placedBuildings.Clear(); //for Global objects not being reset
        instance = this;
        GameManager.instance.OnBeforeSceneUnload += (scene, scene1) => { if(scene.buildIndex == 1 && buildMode) CancelBuilding(); };
    }

    private void OnBuild()
    {
        if(WaveManager.isWaveActive || PauseMenu.isPaused || GameOver.gameOver)
            return;
        
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            VerifyObjects();
            if (buildMode)
                CancelBuilding();
            else {
                HideMapBuildings();
                ValidatePrices();
                buildMode = true;
                buildMenu.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
    }

    private void VerifyObjects()
    {
        if (tilemapGroundTop == null || tilemapCollisonObject == null || tilemapGround == null || environment == null)
        {
            tilemapGroundTop = GameObject.Find("GroundTop").GetComponent<Tilemap>();
            tilemapCollisonObject = GameObject.Find("CollisionObject").GetComponent<Tilemap>();
            tilemapGround = GameObject.Find("Ground").GetComponent<Tilemap>();
            environment = GameObject.Find("Environment").transform;
        }
    }

    public void MapBuild()
    {
        VerifyObjects();
        ValidatePrices();
        buildMode = true;
        buildMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        if (!buildMode)
            return;
        
        if(WaveManager.isWaveActive)
            CancelBuilding();

        if (tilemapGroundTop == null)
            return;
        
        var position = tilemapGroundTop.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedTile == null)
            {
                TileBase tileClicked = tilemapGroundTop.GetTile(position);
                if (tileClicked != null)
                {
                    selectedTile = tileClicked;
                    initialPosition = position;
                    previewPosition = position;
                }
            }
            else if (previewPosition != null && previewPosition == position && tilemapGround.GetTile(position) != null)
            {
                if (selectedTile == spawnTile.Rows[0].Tiles[0]) //TODO make selectedTile hold a tilePrefab instead of a tileBase
                {
                    if (initialPosition != null)
                    {
                        foreach (var spawners in FindObjectsByType<Spawner>(FindObjectsSortMode.None))
                        {
                            if (spawners.transform.position == tilemapGroundTop.CellToWorld(initialPosition.Value) + tilemapGroundTop.cellSize/2)
                                Destroy(spawners.gameObject);
                        }
                    }
                    Instantiate(spawnTilePrefab, tilemapGroundTop.CellToWorld(position) + tilemapGroundTop.cellSize/2, Quaternion.identity, environment.transform);
                }
                
                if(initialPosition != null)
                    placedBuildings.Remove(initialPosition.Value);
                
                tilemapGroundTop.SetTile(position, selectedTile);
                placedBuildings.Add(position, GetTilePrefab(selectedTile));

                if (cost.Count > 0)
                {
                    Inventory.instance.RemoveItems(cost.ToArray());
                    cost.Clear();
                }
                
                selectedTile = null;
                TrapManager.instance.RefreshTiles();
            }
        }

        if (selectedTile != null && tilemapGroundTop.GetTile(position) == null && tilemapGround.GetTile(position) != null)
        {
            if (previewPosition == null)
            {
                previewPosition = position;
                tilemapGroundTop.SetTile(previewPosition.Value, selectedTile);
            }
            else
            {
                tilemapGroundTop.SetTile(previewPosition.Value, null);
                tilemapGroundTop.SetTile(position, selectedTile);
                previewPosition = position;
            }
        }
    }

    private void ResetSelected()
    {
        if (selectedTile != null)
        {
            if(previewPosition != null)
                tilemapGroundTop.SetTile(previewPosition.Value, null);
            if (initialPosition != null)
                tilemapGroundTop.SetTile(initialPosition.Value, selectedTile);
            else if (cost.Count > 0)
                cost.Clear();
            
            selectedTile = null;
        }
    }

    public void CancelBuilding()
    {
        BuildingToolTip.instance.Hide();
        buildMode = false;
        buildMenu.SetActive(false);
        ResetSelected();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ValidatePrices()
    {
        var container = buildMenu.transform.GetChild(0);
        for (var i = 0; i < container.childCount; i++)
        {
            var child = container.GetChild(i);
            for (var j = 0; j < child.childCount; j++)
            {
                var button = child.GetChild(j).gameObject;
                var image = button.transform.GetChild(1).GetComponent<Image>();

                if (i is 0 or 1)
                {
                    if (!Inventory.instance.HasItems(button.GetComponent<BuildingButton>().building.cost))
                    {
                        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.8f);
                        button.GetComponent<Button>().onClick.RemoveAllListeners();
                    }
                    else continue;
                }
                
                if(Inventory.instance.HasItems(button.GetComponent<BuildingButton>().building.cost))
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
                else
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 0.8f);
            }
        }
    }

    private void HideMapBuildings()
    {
        var container = buildMenu.transform.GetChild(0);
        for (var i = 0; i < 2; i++)
        {
            var child = container.GetChild(i);
            for (var j = 0; j < child.childCount; j++)
            {
                var image = child.GetChild(j).gameObject.transform.GetChild(1).GetComponent<Image>();
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.8f);
            }
        }
    }

    public void SelectTilePrefab(BuildingButton button)
    {
        if(!Inventory.instance.HasItems(button.building.cost))
            return;
        
        if(selectedTile != null)
            ResetSelected();
        selectedTile = button.building.Rows[0].Tiles[0]; //TODO make this work with buildings that are more than one tile

        cost = button.building.cost.Select(stack => stack.Clone()).ToList();
        
        previewPosition = null;
        initialPosition = null;
    }

    public GameObject GetBuildMenu()
    {
        return buildMenu;
    }

    private TilePrefab GetTilePrefab(TileBase tile)//TEMP
    {
        foreach (var tilePrefab in Resources.LoadAll<TilePrefab>("Dungeon"))
        {
            if (tilePrefab.Rows.Length == 0)
                continue;
            
            if(tilePrefab.Rows[0].Tiles[0] == tile)
                return tilePrefab;
        }

        return null;
    }

    private void PlaceBuilding(TilePrefab tilePrefab, Vector3Int location)
    {
        VerifyObjects();
        if (tilePrefab.Rows[0].Tiles[0] == spawnTile.Rows[0].Tiles[0])
        {
            Instantiate(spawnTilePrefab, tilemapGroundTop.CellToWorld(location) + tilemapGroundTop.cellSize/2, Quaternion.identity, environment.transform);
        }
        placedBuildings.Add(location, tilePrefab);
                
        tilemapGroundTop.SetTile(location, tilePrefab.Rows[0].Tiles[0]);
    }

    public void LoadData(DungeonData data)
    {
        if (data.buildings.Count > 0)
        {
            placedBuildings.Clear();
            foreach (var building in data.buildings) PlaceBuilding(building.GetPrefab(), building.position);
        }
        else
        {
            PlaceBuilding(spawnTile, new Vector3Int(21, 4, 0));
        }
    }

    public void SaveData(ref DungeonData data)
    {
        data.buildings.Clear();

        foreach (var building in placedBuildings)
        {
            data.buildings.Add(new BuildingData(building.Value.name, building.Key));
        }
    }
}
