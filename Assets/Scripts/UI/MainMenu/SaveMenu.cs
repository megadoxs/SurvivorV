using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveMenu : MonoBehaviour
{
    public static SaveMenu instance;
    
    [SerializeField] 
    private GameObject savePrefab;

    [SerializeField] 
    private GameObject saveList;

    public List<string> saves;
    private List<GameObject> saveSlots;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        saves = new List<string>();
        if(saveSlots != null)
            saveSlots.ForEach(self => Destroy(self));
        saveSlots = new List<GameObject>();
        
        foreach (var save in GameDataManager.instance.GetAllSaves())
        {
            saves.Add(save);
            var saveObject = Instantiate(savePrefab, saveList.transform);
            saveObject.GetComponent<SaveSlot>().setName(save);
            saveSlots.Add(saveObject);
        }
    }
}
