using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TilePrefab building;

    private void OnValidate()
    {
        var image = GetComponentInChildren<Image>();
        if (building != null && image != null)
        {
            image.sprite = building.sprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BuildingToolTip.instance.Show(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BuildingToolTip.instance.Hide();
    }
}
