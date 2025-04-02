using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

public class FileDataHandler {
    private string path;
    private static string playerDataPath = "PlayerData.json";
    private static string dungeonDataPath = "DungeonData.json";
    private static string worldDataPath = "WorldData.json";

    public FileDataHandler(string saveName)
    {
        path = Path.Combine(Application.persistentDataPath, saveName);
    }

    public FileDataHandler() { }

    
    private T LoadData<T>(string dataPath) where T : class
    {
        var fullPath = Path.Combine(path, dataPath);

        if (!File.Exists(fullPath))
        {
            return null;
        }

        using var stream = new FileStream(fullPath, FileMode.Open);
        using var reader = new StreamReader(stream);
        return JsonUtility.FromJson<T>(reader.ReadToEnd());
    }

    private void SaveData<T>(T data, string dataPath)
    {
        var fullPath = Path.Combine(path, dataPath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        
        using var stream = new FileStream(fullPath, FileMode.Create);
        using var writer = new StreamWriter(stream);
        writer.Write(JsonUtility.ToJson(data, true));
        writer.Close();
        stream.Close();
    }
    
    public PlayerData LoadPlayerData() => LoadData<PlayerData>(playerDataPath);
    public void SavePlayerData(PlayerData data) => SaveData(data, playerDataPath);
    public DungeonData LoadDungeonData() => LoadData<DungeonData>(dungeonDataPath);
    public void SaveDungeonData(DungeonData data) => SaveData(data, dungeonDataPath);
    public WorldData LoadWorldData() => LoadData<WorldData>(worldDataPath);
    public void SaveWorldData(WorldData data) => SaveData(data, worldDataPath);

    public List<string> GetAllSaves()
    {
        List<string> saves = new();
        saves.AddRange(new DirectoryInfo(Application.persistentDataPath).GetDirectories().OrderByDescending(directory => directory.LastAccessTime).Select(directory => directory.Name));
        return saves;
    }

    public string GetLatestSave()
    {
        return GetAllSaves().FirstOrDefault();
    }
    
}
