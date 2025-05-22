using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverMenu;
    
    public static GameOver instance;
    public static bool gameOver;
    
    private void Awake()
    {
        instance = this;
    }
    
    public void ShowGameOverMenu()
    {
        gameOver = true;
        Time.timeScale = 0;
        gameOverMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
