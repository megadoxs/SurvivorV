using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingToolTip : MonoBehaviour
{
    [SerializeField] 
    private Image image;
    [SerializeField] 
    private new TMP_Text name;
    [SerializeField] 
    private TMP_Text description;
    [SerializeField] 
    private GameObject cost;
    
    [Space]
    [SerializeField] 
    private GameObject itemCostPrefab;
    
    public static BuildingToolTip instance;
    
    private void Awake()
    {
        instance = this;
        Hide();
    }

    public void Show(GameObject buildingButton)
    {
        var buttonPosition = buildingButton.transform.position;
        
        var rt = GetComponent<RectTransform>();
        var width = rt.rect.width * rt.lossyScale.x;
        
        var screenRightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        
        var rightPosition = buttonPosition + new Vector3(width, 0, 0);
        if (Camera.main.ScreenToWorldPoint(new Vector3(rightPosition.x + width / 2, 0)).x < screenRightEdge)
            transform.position = rightPosition;
        else
            transform.position = buttonPosition - new Vector3(width, 0, 0);
        
        var building = buildingButton.GetComponent<BuildingButton>().building;
        
        image.sprite = building.sprite;
        image.type = Image.Type.Simple;
        image.preserveAspect = true;
        name.text = building.name;
        description.text = building.description;

        for (var i = cost.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(cost.transform.GetChild(i).gameObject);
        }
        
        foreach (var item in building.cost)
        {
            var itemCost = Instantiate(itemCostPrefab, cost.transform);
            itemCost.GetComponentInChildren<Image>().sprite = item.GetItem().Icon;
            itemCost.GetComponentInChildren<TMP_Text>().text = Inventory.instance.GetItemCount(item.GetItem()) + "/" + item.GetCount() + " " + item.GetItem().Name;
        }
        

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
