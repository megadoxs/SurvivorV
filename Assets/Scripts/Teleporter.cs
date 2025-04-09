using System.Collections;
using System.Linq;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Teleporter : MonoBehaviour
{
    [SerializeField]
    private SceneAsset scene;
    [Min(0)]
    public int index;
    
    private static bool teleported;
    private static int teleporter;
    private void Start()
    {
        gameObject.SetActive(!SceneManager.GetActiveScene().name.Equals(scene.name));
    }

    private void OnTriggerExit2D(Collider2D player)
    {
       if(player.CompareTag("Player"))
           teleported = false;
    }

    private void OnTriggerEnter2D(Collider2D player)
    {
            if (player.CompareTag("Player") && !teleported)
            {
                player.GetComponent<PlayerController>().LockMovement();
                teleporter = index;
                Fade.state = true;
                Time.timeScale = 0;
                StartCoroutine(Teleport());
            }
    }

    private IEnumerator Teleport()
    {
        //waits for fade in
        yield return new WaitUntil(() => Fade.Done());
        //loads new scene
        Time.timeScale = 1;
        GameDataManager.instance.LoadScene(scene.name);
        SceneManager.sceneLoaded += OnLoadScene;
        //start fade out
        Fade.state = false;
    }
    
    private void OnLoadScene(Scene scene, LoadSceneMode mode)
    {
        Collider2D player = GameObject.FindWithTag("Player").GetComponent<Collider2D>();
        player.transform.position = FindObjectsByType<Teleporter>(FindObjectsSortMode.None).Where(t => t.index == teleporter).ToArray().First().GetComponent<Collider2D>().bounds.center;
        CinemachineCamera virtualCam = FindFirstObjectByType<CinemachineCamera>();
        virtualCam.PreviousStateIsValid = false;
        virtualCam.transform.position = player.transform.position;
        player.GetComponent<PlayerController>().UnlockMovement();
        teleported = true;
        SceneManager.sceneLoaded -= OnLoadScene;
    }
}
