using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Data/Wave")]
public class Wave : ScriptableObject
{
    public WaveEntity[] entities;
}

[Serializable]
public class WaveEntity
{
    public EntitySO entity;
    public int count;
}