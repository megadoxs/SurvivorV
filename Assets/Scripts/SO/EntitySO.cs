using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "Data/Entity")]
public class EntitySO : ScriptableObject
{
    public GameObject entityPrefab;
    
    public HostilityType hostility;
    
    public float maxHealth;
    public float detectionRange;
    public float speed;
    public float damage;
    public float attackRange;
    public ItemStack[] drops;
}
