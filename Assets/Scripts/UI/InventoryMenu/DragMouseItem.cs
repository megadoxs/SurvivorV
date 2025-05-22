using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragMouseItem : MonoBehaviour //TODO rename to MouseItemSlot
{
    public GameObject prefab;
    [HideInInspector]
    public Transform itemList;
    [HideInInspector]
    public GameObject itemDrag;

    public Image image;
    public TextMeshProUGUI itemCount;
    private ItemStack itemStack;

    private void Update() {
        Vector3 mousePosition = Input.mousePosition;
        transform.position = mousePosition;

        if (Input.GetMouseButtonUp(0))
        {
            GameObject pickupItem = Instantiate(prefab, GameObject.FindWithTag("Player").transform.position, Quaternion.identity);
            pickupItem.GetComponent<PickupItem>().SetItemStack(itemStack);
            pickupItem.GetComponent<PickupItem>().playerInside = true;
            itemDrag.GetComponent<DragItemSlot>().DeleteItem();
            Destroy(itemDrag);
            Destroy(gameObject);
        }

        RectTransform parentRect = itemList.transform.parent.gameObject.GetComponent<RectTransform>();

        Vector3[] corners = new Vector3[4];
        parentRect.GetWorldCorners(corners);

        float leftBound = corners[0].x;
        float rightBound = corners[2].x;

        if (mousePosition.x >= leftBound && mousePosition.x <= rightBound)
        {
            itemDrag.SetActive(true);
            Destroy(gameObject);
        }
    }

    public void SetItemStack(ItemStack itemStack) {
        this.itemStack = itemStack;
        image.sprite = itemStack.GetItem().Icon;
        itemCount.text = itemStack.GetCount().ToString();
    }
}
