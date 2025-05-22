using System;
using TMPro;
using UnityEngine;

public class DungeonHeart : MonoBehaviour, IDungeonDataSaver
{
    private float health;
    private float maxHealth;

    private GameObject healthBar;

    public static DungeonHeart instance { get; private set; }
    
    private void Awake()
    {
        instance = this;
    }

    public void Damage(float damage)
    {
        health -= damage;
        GameObject.Find("DungeonCoreHealth").GetComponentInChildren<TMP_Text>().text = health + "/" + maxHealth;
        if (health <= 0)
        {
            Destroy(gameObject);
            GameOver.instance.ShowGameOverMenu();
        }
    }

    public void Heal(float heal)
    {
        health += heal;
    }

    public void Upgrade(float health)
    {
        this.health += health; 
        maxHealth += health;
    }

    public void LoadData(DungeonData data)
    {
        health = data.dungeonHeartHealth;
        maxHealth = data.dungeonHeartMaxHealth;
    }

    public void SaveData(ref DungeonData data)
    {
        data.dungeonHeartHealth = health;
        data.dungeonHeartMaxHealth = maxHealth;
    }
}
