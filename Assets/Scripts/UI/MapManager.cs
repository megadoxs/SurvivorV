using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapManager : MonoBehaviour
{
    [SerializeField] 
    private GameObject miniMap; 
    [SerializeField] 
    private GameObject largeMap;

    [SerializeField] 
    private RenderTexture miniMapTexture;
    [SerializeField] 
    private RenderTexture largeMapTexture;

    public static bool MapOpen;
    
    private bool miniMapToggle = true;
    private Transform mainCamera;
    private Vector3 dragOrigin;
    private bool isDragging;
    
    public static MapManager instance { get; private set; }
    
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (MapOpen)
        {
            Camera camera = GameObject.Find("Map Camera").GetComponent<Camera>();
            
            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = camera.ScreenToWorldPoint(Input.mousePosition);
                isDragging = true;
            } 
            else if(Input.GetMouseButton(0) && isDragging)
            {
                Vector3 currentPos = camera.ScreenToWorldPoint(Input.mousePosition);
                Vector3 difference = dragOrigin - currentPos;

                camera.transform.position += difference;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
            
            // Handle zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                float newSize = camera.orthographicSize - scroll * 2;
                camera.orthographicSize = Mathf.Clamp(newSize, 2, 10);
            }
        }
    }

    private void OnMap()
    {
        if (PauseMenu.isPaused || GameOver.gameOver)
            return;
        
        var camera = GameObject.Find("Map Camera").GetComponent<Camera>();

        MapOpen = !MapOpen;
        
        if (MapOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            camera.orthographicSize = 10;
            camera.targetTexture = largeMapTexture;
            mainCamera = camera.transform.parent;
            camera.transform.SetParent(mainCamera.transform.parent);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            camera.transform.SetParent(mainCamera);
            camera.transform.position = mainCamera.transform.position;
            camera.orthographicSize = 7;
            camera.targetTexture = miniMapTexture;
        }
        
        largeMap.SetActive(MapOpen);
        if(miniMapToggle)
            miniMap.SetActive(!MapOpen);
    }

    public void CloseMap()
    {
        MapOpen = false;
        
        var camera = GameObject.Find("Map Camera").GetComponent<Camera>();
        
        camera.transform.SetParent(mainCamera);
        camera.transform.position = mainCamera.transform.position;
        camera.orthographicSize = 7;
        camera.targetTexture = miniMapTexture;
        
        largeMap.SetActive(false);
        if(miniMapToggle)
            miniMap.SetActive(true);
    }

    private void OnToggleMinimap()
    {
        if (!MapOpen)
        {
            miniMapToggle = !miniMapToggle;
            miniMap.SetActive(miniMapToggle);
        }
    }
}
