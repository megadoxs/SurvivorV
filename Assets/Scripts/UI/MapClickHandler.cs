using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapClickHandler : MonoBehaviour, IPointerClickHandler
{
    public Camera miniMapCam;
    private GraphicRaycaster graphicRaycaster;

    private void OnEnable()
    {
        var mapMarkers = GameObject.Find("MapMarkers");
        
        mapMarkers.GetComponent<Canvas>().worldCamera = miniMapCam;
        graphicRaycaster = mapMarkers.GetComponent<GraphicRaycaster>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, eventData.pressPosition, eventData.pressEventCamera, out var cursor))
        {
            Texture texture = GetComponent<RawImage>().texture;
            Rect rect = GetComponent<RawImage>().rectTransform.rect;

            float coordX = Mathf.Clamp(0, (((cursor.x - rect.x) * texture.width) / rect.width), texture.width);
            float coordY = Mathf.Clamp(0, (((cursor.y - rect.y) * texture.height) / rect.height), texture.height);

            float calX = coordX / texture.width;
            float calY = coordY / texture.height;

       
            cursor = new Vector2(calX, calY);
            
            CastRayToWorld(cursor);
        }
    }
    
    private void CastRayToWorld(Vector2 vec)
    {
        var screenPos = new Vector2(vec.x * miniMapCam.pixelWidth, vec.y * miniMapCam.pixelHeight);

        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = screenPos };
        
        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            Button button = result.gameObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke();
            }
        }
    }
}
