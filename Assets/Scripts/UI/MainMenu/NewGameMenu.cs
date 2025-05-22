using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewGameMenu : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    public void NewGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        var input = inputField.text;
        
        if (input.Length == 0)
            input = "New Game";
        
        Fade.state = true;
        StartCoroutine(NewGameCoroutine(input));
    }
    
    private IEnumerator NewGameCoroutine(string save)
    {
        yield return new WaitUntil(() => Fade.Done());
        Resources.FindObjectsOfTypeAll<PlayerController>().First().gameObject.SetActive(true);
        DataManager.instance.NewGame(save);
    }

    public void Cancel()
    {
        MainMenu.instance.Back();
    }
}
