using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance { get; private set; }
    
    private PlayerData playerData;
    private DungeonData dungeonData;
    private WorldData worldData;

    private FileDataHandler fileDataHandler;
    private void Awake()
    {
        instance = this;
        SceneManager.sceneLoaded += OnLoadScene;
    }

    public void LoadScene(string scene) //to ensure data is saved, loadScene should be called here
    {
        var currentScene = SceneManager.GetActiveScene();
        BeforeSceneUnload(currentScene);
        SceneManager.LoadScene(scene);
    }

    private void BeforeSceneUnload(Scene scene) //saves old scene data
    {
        if (scene.buildIndex == 0)
        {
            foreach (var saver in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IPlayerDataSaver>())
            {
                saver.LoadData(playerData);
            }
            Collider2D player = GameObject.FindWithTag("Player").GetComponent<Collider2D>();
            CinemachineCamera virtualCam = FindFirstObjectByType<CinemachineCamera>();
            virtualCam.PreviousStateIsValid = false;
            virtualCam.transform.position = player.transform.position;
            Fade.state = false;
            return;
        }
        
        switch (scene.name)
        {
            case "DungeonScene":
            {
                foreach (var saver in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDungeonDataSaver>())
                {
                    saver.SaveData(ref dungeonData);
                }
                fileDataHandler.SaveDungeonData(dungeonData);
                break;
            }
            case "WorldScene":
            {
                foreach (var saver in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IWorldDataSaver>())
                {
                    saver.SaveData(ref worldData);
                }
                fileDataHandler.SaveWorldData(worldData);
                break;
            }
        }
    }

    private void OnLoadScene(Scene scene, LoadSceneMode mode) //loads scene data + save player data
    {
        if (scene.buildIndex == 0)
            return;
        
        foreach (var saver in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IPlayerDataSaver>())
        {
            saver.SaveData(ref playerData);
        }
        playerData.sceneName = SceneManager.GetActiveScene().name;
        fileDataHandler.SavePlayerData(playerData);

        switch (scene.name)
        {
            case "DungeonScene":
            {
                foreach (var saver in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDungeonDataSaver>())
                {
                    saver.LoadData(dungeonData);
                }
                break;
            }
            case "WorldScene":
            {
                foreach (var saver in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IWorldDataSaver>())
                {
                    saver.LoadData(worldData);
                }
                break;
            }
        }
    }

    public void NewGame(string saveName)
    {
        playerData = new PlayerData();
        dungeonData = new DungeonData();
        worldData = new WorldData();

        fileDataHandler = new FileDataHandler(saveName);
        LoadScene(playerData.sceneName);
    }

    public void LoadGame(string saveName)
    {
        fileDataHandler = new FileDataHandler(saveName);
        
        playerData = fileDataHandler.LoadPlayerData();
        dungeonData = fileDataHandler.LoadDungeonData();
        worldData = fileDataHandler.LoadWorldData();
        
        if(playerData == null) //temp
            NewGame(saveName);
        else
            LoadScene(playerData.sceneName);
    }

    public void SaveGame() //save current scene + player 
    {
        BeforeSceneUnload(SceneManager.GetActiveScene());
        foreach (var saver in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IPlayerDataSaver>())
        {
            saver.SaveData(ref playerData);
        }
        playerData.sceneName = SceneManager.GetActiveScene().name;
        fileDataHandler.SavePlayerData(playerData);
    }

    public List<string> GetAllSaves()
    {
        return new FileDataHandler().GetAllSaves();
    }

    private void OnApplicationQuit()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0)
         SaveGame();
    }

    public string GetLatestSave()
    {
        return new FileDataHandler().GetLatestSave();
    }
}
