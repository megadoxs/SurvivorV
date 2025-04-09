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
        BeforeSceneUnload(currentScene, scene);
        SceneManager.LoadScene(scene);
    }

    public void LoadGameScene(string scene)
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
        SceneManager.LoadScene(scene);
    }

    private void BeforeSceneUnload(Scene oldScene, string newScene) //saves old scene data
    {
        if (newScene != null && newScene.Equals("MainMenuScene"))
        {
            SavePlayer(oldScene.name);
            SceneManager.sceneLoaded -= OnLoadScene;
        }
        
        switch (oldScene.name)
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
        
        SavePlayer(SceneManager.GetActiveScene().name);
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
    
    private void SavePlayer(string sceneName)
    {
        foreach (var saver in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IPlayerDataSaver>())
        {
            saver.SaveData(ref playerData);
        }
        playerData.sceneName = sceneName;
        fileDataHandler.SavePlayerData(playerData);
    }

    public void NewGame(string saveName)
    {
        playerData = new PlayerData();
        dungeonData = new DungeonData();
        worldData = new WorldData();

        fileDataHandler = new FileDataHandler(saveName);
        LoadGameScene(playerData.sceneName);
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
            LoadGameScene(playerData.sceneName);
    }

    public void SaveGame() //save current scene + player 
    {
        BeforeSceneUnload(SceneManager.GetActiveScene(), null);
        SavePlayer(SceneManager.GetActiveScene().name);
    }

    public static List<string> GetAllSaves()
    {
        return new FileDataHandler().GetAllSaves();
    }
    
    public static string GetLatestSave()
    {
        return new FileDataHandler().GetLatestSave();
    }

    private void OnApplicationQuit()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0) 
            SaveGame();
    }
}
