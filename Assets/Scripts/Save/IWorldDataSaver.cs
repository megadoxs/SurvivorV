using UnityEngine;

public interface IWorldDataSaver
{
    void LoadData(WorldData data);
    void SaveData(ref WorldData data);
}
