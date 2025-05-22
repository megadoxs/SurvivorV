using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager instance { get; private set; }
    
    private PlayerData playerData;
    private DungeonData dungeonData;
    private WorldData worldData;

    private FileDataHandler fileDataHandler;
    private void Awake()
    {
        instance = this;
        SceneManager.sceneLoaded += OnLoadScene;
        GameManager.instance.OnBeforeSceneUnload += BeforeSceneUnload;
    }

    private void OnDestroy() //TODO this should NOT be needed, it should never be destroyed or awaken more than once, something is wrong with my global object
    {
        SceneManager.sceneLoaded -= OnLoadScene;
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

    private void BeforeSceneUnload(Scene oldScene, Scene newScene) //saves old scene data
    {
        if (oldScene.buildIndex != 0)
            SavePlayer();
        SaveScene();
    }
    
    private void SaveScene()
    {
        switch (SceneManager.GetActiveScene().name)
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
        
        SavePlayer();
        switch (scene.name)
        {
            case "DungeonScene":
            {
                foreach (var camera in FindObjectsByType<Camera>(FindObjectsSortMode.None))
                {
                    camera.backgroundColor = new Color(37/255f, 19/255f, 26/255f); //TODO fix, color is not identical as the tile one
                }
                foreach (var saver in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDungeonDataSaver>())
                {
                    saver.LoadData(dungeonData);
                }
                break;
            }
            case "WorldScene":
            {
                foreach (var camera in FindObjectsByType<Camera>(FindObjectsSortMode.None))
                {
                    camera.backgroundColor = new Color(80/255f, 155/255f, 102/255f);
                }
                foreach (var saver in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IWorldDataSaver>())
                {
                    saver.LoadData(worldData);
                }
                break;
            }
        }
    }
    
    private void SavePlayer()
    {
        foreach (var saver in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IPlayerDataSaver>())
        {
            saver.SaveData(ref playerData);
        }
        playerData.sceneName = SceneManager.GetActiveScene().name;
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

    //TODO check if this can be made private
    public void SaveGame() //save current scene + player 
    {
        SaveScene();
        SavePlayer();
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
