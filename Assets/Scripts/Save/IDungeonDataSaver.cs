using UnityEngine;

public interface IDungeonDataSaver
{
    void LoadData(DungeonData data);
    void SaveData(ref DungeonData data);
}
