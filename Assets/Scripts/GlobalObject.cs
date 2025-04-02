using UnityEngine;

public class GlobalObject : MonoBehaviour
{
    private static GlobalObject instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Keep UI across scenes
        }
        else
        {
            Destroy(gameObject);  // Remove duplicates
        }
    }
}
