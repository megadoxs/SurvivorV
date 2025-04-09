using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MenuSwitcher //TODO use menuSwitcher if possible to remove code duplication maybe extend it?
{
    public GameObject pauseMenu;
    public GameObject gameMenu;
    public GameObject settingsMenu;
    
    public static bool isPaused;

    public void OnPauseGame()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        isPaused = true;
    }

    public void ResumeGame() //TODO also makes the cursor invisible if the inventory is open
    {
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        isPaused = false;
        gameMenu.SetActive(true);
        settingsMenu.SetActive(false);
        Reset();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        gameMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void MainMenu()
    {
        Fade.state = true;
        StartCoroutine(LoadMainMenu());
    }
    
    private IEnumerator LoadMainMenu()
    {
        yield return new WaitUntil(() => Fade.Done());
        GameDataManager.instance.LoadScene("MainMenuScene");
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
        gameMenu.SetActive(true);
        settingsMenu.SetActive(false);
        Reset();
        GameObject.FindWithTag("Player").SetActive(false);
        Fade.state = false;
    }
}
