using System;
using TMPro;
using UnityEngine;

public class SaveSlot : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void setName(string name)
    {
        text.text = name;
    }
    
    public void LoadSave()
    {
        MainMenu.instance.LoadGame(SaveMenu.instance.saves[gameObject.transform.GetSiblingIndex()]);
    }
}
