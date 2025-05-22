using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour, IDungeonDataSaver
{
    [SerializeField]
    private TMP_Text timeText;
    
    public static bool isWaveActive;

    private float time;
    
    private int wave = -1;
    private bool waveSpawnDone;
    private List<GameObject> monsters = new();
    
    private GameObject buttonList;
    private GameObject dungeonHeart;
    
    [SerializeField]
    private Wave[] waves;
    
    private bool loaded;
    private Coroutine waveSpawnCoroutine;
    private int monsterSpawned;

    private void Awake()
    {
        GameManager.instance.OnBeforeSceneUnload += (scene, scene1) => { if(scene.buildIndex == 1 && !waveSpawnDone && loaded && waveSpawnCoroutine != null) StopCoroutine(waveSpawnCoroutine); };
    }
    
    
    private void Start()
    {
        time = 100;
    }

    private void Update()
    {
        if (!loaded)
            return;
        
        if (!isWaveActive && SceneManager.GetActiveScene().buildIndex != 0)
        {
            if(wave > waves.Length - 1)
                return;
            
            if (time > 0)
            {
                time -= Time.deltaTime;
                var min = Math.Max(Mathf.FloorToInt(time / 60), 0);
                var sec = Math.Max(Mathf.FloorToInt(time % 60), 0);
                timeText.text = $"{min:00}:{sec:00}";   
            }
            else if (time <= 0)
            {
                time = 100;
                isWaveActive = true;
                if (SceneManager.GetActiveScene().buildIndex == 1) //TODO need to spawn the wave even if the player is not in the scene, need the scene to load in background
                {
                    if(dungeonHeart == null)
                        dungeonHeart = GameObject.Find("DungeonHeart");
                    if(buttonList == null)
                        buttonList = GameObject.Find("MapMarkers").transform.GetChild(1).gameObject;;
                    
                    wave++;
                    waveSpawnDone = false;
                    buttonList.SetActive(false);
                    TextManager.instance.ShowAnnouncement("Spawning Wave: " + (wave + 1), 5);
                    waveSpawnCoroutine = StartCoroutine(SpawnWave());
                }
                // else
                // {
                //     SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
                // }
            }
        }
        else if (isWaveActive && SceneManager.GetActiveScene().buildIndex == 1)
        {
            monsters.RemoveAll(monster => monster == null);

            if (monsters.Count == 0 && waveSpawnDone)
            {
                if(buttonList == null)
                    buttonList = GameObject.Find("MapMarkers").transform.GetChild(1).gameObject;;
                
                TextManager.instance.ShowAnnouncement("Wave " + (wave + 1) + " Cleared", 5);
                isWaveActive = false;
                buttonList.SetActive(true);
            }
        }
    }
    
    private IEnumerator SpawnWave()
    {
        var spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
        var index = 0;
        
        foreach (var wave in waves[wave].entities)
        {
            for (var i = 0; i < wave.count; i++)
            {
                var monster = Instantiate(wave.entity.entityPrefab, spawners[index].transform.position, Quaternion.identity);
                if (dungeonHeart != null)
                    monster.GetComponent<AIDestinationSetter>().target = dungeonHeart.GetComponent<CircleCollider2D>().transform;  
                monster.GetComponent<Entity>().SetEntityStats(wave.entity);
                monsters.Add(monster);
                monsterSpawned++;
                index = (index + 1) % spawners.Length;
                
                yield return new WaitForSeconds(1);
            }
        }

        waveSpawnDone = true;
    }

    private IEnumerator FinishSpawnWave(int monsterCount)
    {
        Spawner[] spawners = null; //TODO maybe I should store them in a list that isn't remade every execution
        while (spawners == null || spawners.Length == 0) //TODO do I really need that?
        {
            spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
            yield return null;
        }
        var index = 0;
        
        var cumulative = 0;
        var waveIndex = -1;
        var monsterIndex = -1;

        for (var i = 0; i < waves.Length; i++)
        {
            var waveCount = waves[i].entities.Sum(e => e.count);
            if (monsterCount < cumulative + waveCount)
            {
                waveIndex = i;
                monsterIndex = monsterCount - cumulative;
                break;
            }
            cumulative += waveCount;
        }

        
        monsterSpawned = monsterCount;
        foreach (var wave in waves[waveIndex].entities) //TODO if there is more than one type of monster, this won't work. It needs to know which monster to spawn
        {
            for (var i = monsterIndex; i < wave.count; i++)
            {
                var monster = Instantiate(wave.entity.entityPrefab, spawners[index].transform.position, Quaternion.identity);
                if (dungeonHeart != null)
                    monster.GetComponent<AIDestinationSetter>().target = dungeonHeart.GetComponent<CircleCollider2D>().transform;  
                monster.GetComponent<Entity>().SetEntityStats(wave.entity);
                monsters.Add(monster);
                monsterSpawned++;
                index = (index + 1) % spawners.Length;
                
                yield return new WaitForSeconds(1);
            }
        }

        waveSpawnDone = true;
    }

    private void RespawnMonster(EntityData entity)
    {
        var monster = Instantiate(entity.GetEntity().entityPrefab, entity.position, Quaternion.identity);
        if (dungeonHeart != null)
            monster.GetComponent<AIDestinationSetter>().target = dungeonHeart.GetComponent<CircleCollider2D>().transform;
        monster.GetComponent<Entity>().LoadEntity(entity);
        monsters.Add(monster);
    }

    public void LoadData(DungeonData data) //TODO would be so much simpler if I give the wave to FinishSpawnWave
    {
        if (!loaded)
            time = data.waveTimer;
        
        wave = data.waveCount - 1;

        if (wave != -1 && data.monsterSpawned < waves[wave].entities.Sum(e => e.count))
        {
            isWaveActive = true;
            waveSpawnCoroutine = StartCoroutine(FinishSpawnWave(data.monsterSpawned));
        }
        else 
            waveSpawnDone = true;
        
        if (data.entities.Count > 0)
        {
            foreach (var entity in data.entities) RespawnMonster(entity);
            isWaveActive = true;
        }
        loaded = true;
    }

    public void SaveData(ref DungeonData data)//TODO time needs to be saved in the world as well as it keep going down
    {
        data.waveTimer = time;   
        data.waveCount = wave + 1;
        data.monsterSpawned = monsterSpawned;

        data.entities = new List<EntityData>();
        foreach (var monster in monsters)
        {
            var entity = monster.GetComponent<Entity>();
            data.entities.Add(new EntityData(entity.GetEntity(), monster.transform.position, entity.GetHealth()));
        }
    }
}
