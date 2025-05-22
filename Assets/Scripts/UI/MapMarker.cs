using System;
using UnityEngine;
using UnityEngine.UI;

public class MapMarker : MonoBehaviour
{
    public Vector3Int anchor;
    public TilePrefabType type;
    public Direction direction;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            DungeonManager.instance.ShowBuildingMenu(anchor, type, direction, gameObject);
            MapManager.instance.CloseMap();
        });
    }
}
