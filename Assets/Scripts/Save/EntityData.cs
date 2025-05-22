using System;
using UnityEngine;

[Serializable]
public class EntityData
{
    public string type;
    public Vector3 position;
    public float health;

    public EntityData(EntitySO entity, Vector3 position, float health)
    {
        type = entity.name;
        this.position = position;
        this.health = health;
    }

    public EntitySO GetEntity()
    {
        return Resources.Load<EntitySO>("Monsters/" + type);
    }
}
