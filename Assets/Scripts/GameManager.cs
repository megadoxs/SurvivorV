using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour //TODO new game doesn't work because it never forgets the GlobalObject which aren't reseted
{
    public static GameManager instance { get; private set; }
    
    public event Action<Scene, Scene> OnBeforeSceneUnload;

    private void Awake()
    {
        instance = this;
    }

    //TODO not sure both are needed
    public void LoadScene(string sceneName)
    {
        BeforeSceneUnload(SceneManager.GetSceneByName(sceneName));
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneIndex)
    {
        BeforeSceneUnload(SceneManager.GetSceneByBuildIndex(sceneIndex));
        SceneManager.LoadScene(sceneIndex);
    }

    private void BeforeSceneUnload(Scene scene) //TODO scene is always null
    {
        OnBeforeSceneUnload?.Invoke(SceneManager.GetActiveScene(), scene);
    }
}
