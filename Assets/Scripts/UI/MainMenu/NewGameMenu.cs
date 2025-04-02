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
        var input = inputField.text; //TODO should create a name for an empty input
        Fade.state = true;
        StartCoroutine(NewGameCoroutine(inputField.text));
    }
    
    private IEnumerator NewGameCoroutine(string save)
    {
        yield return new WaitUntil(() => Fade.Done());
        Resources.FindObjectsOfTypeAll<PlayerController>().First().gameObject.SetActive(true);
        GameDataManager.instance.NewGame(save);
    }

    public void Cancel()
    {
        MainMenu.instance.Back();
    }
}
