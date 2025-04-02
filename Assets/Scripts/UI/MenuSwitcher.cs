using UnityEngine;

public class MenuSwitcher : MonoBehaviour
{
    [SerializeField]
    private GameObject defaultMenu;
    
    private GameObject activeMenu;
    
    void Start()
    {
        activeMenu = defaultMenu;
    }

    public void ActivateMenu(GameObject menu)
    {
        if (!menu.activeSelf)
        {
            activeMenu.SetActive(false);
            activeMenu = menu;
            activeMenu.SetActive(true);
        }
    }

    public virtual void Reset()
    {
        ActivateMenu(defaultMenu);
    }
}
