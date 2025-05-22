using System;
using System.Collections;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    
    [SerializeField] 
    private GameObject mainMenu;
    [SerializeField]
    private GameObject resumeButton;
    
    private GameObject activeMenu;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (DataManager.GetLatestSave() == null)
            resumeButton.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Back();
    }

    public void Back()
    {
        if (activeMenu != null)
        {
            activeMenu.SetActive(false);
            activeMenu = null;
            mainMenu.SetActive(true);
        }
    }

    public void LoadLatestGame()
    {
        Fade.state = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(LoadGameCoroutine(DataManager.GetLatestSave()));
    }

    public void LoadGame(string save)
    {
        Fade.state = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(LoadGameCoroutine(save));
    }

    public void ActivateMenu(GameObject menu)
    {
        menu.SetActive(true);
        mainMenu.SetActive(false);
        activeMenu = menu;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadGameCoroutine(string save)
    {
        yield return new WaitUntil(() => Fade.Done());
        Resources.FindObjectsOfTypeAll<PlayerController>().First().gameObject.SetActive(true);
        DataManager.instance.LoadGame(save);
    }
}
