using System.Drawing;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItemSlot : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Transform currentTransform;
    private GameObject mainContent;
    private Vector3 currentPossition;
    public GameObject prebab;
    private GameObject mouse;

    [HideInInspector]
    public GameObject inventory;
    public Image image;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemCount;
    private ItemStack itemStack;

    private int totalChild;

    void Start()
    {
        currentTransform = gameObject.transform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        currentPossition = currentTransform.position;
        mainContent = currentTransform.parent.gameObject;
        totalChild = mainContent.transform.childCount;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform parentRect = mainContent.transform.parent.gameObject.GetComponent<RectTransform>();

        Vector3[] corners = new Vector3[4];
        parentRect.GetWorldCorners(corners);

        float leftBound = corners[0].x;
        float rightBound = corners[2].x;

        if (eventData.position.x >= leftBound && eventData.position.x <= rightBound)
        {
            currentTransform.position = new Vector3(currentTransform.position.x, eventData.position.y, currentTransform.position.z);

            for (int i = 0; i < totalChild; i++)
            {
                if (i != currentTransform.GetSiblingIndex())
                {
                    Transform otherTransform = mainContent.transform.GetChild(i);
                    int distance = (int)Vector3.Distance(currentTransform.position, otherTransform.position);
                    if (distance <= 10)
                    {
                        Vector3 otherTransformOldPosition = otherTransform.position;
                        otherTransform.position = new Vector3(otherTransform.position.x, currentPossition.y, otherTransform.position.z);
                        currentTransform.position = new Vector3(currentTransform.position.x, otherTransformOldPosition.y, currentTransform.position.z);
                        currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                        currentPossition = currentTransform.position;
                    }
                }
            }
        }
        else
        {
            gameObject.SetActive(false);
            mouse = Instantiate(prebab, Input.mousePosition, Quaternion.identity);
            mouse.transform.SetParent(inventory.transform.parent);

            DragMouseItem item = mouse.GetComponent<DragMouseItem>();

            item.itemList = mainContent.transform;
            item.itemDrag = gameObject;
            item.SetItemStack(itemStack);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        currentTransform.position = currentPossition;
    }

    public void SetItemStack(ItemStack itemStack)
    {
        this.itemStack = itemStack;
        image.sprite = itemStack.GetItem().GetSprite();
        itemName.text = itemStack.GetItem().GetName();
        itemCount.text = "x" + itemStack.GetCount().ToString();
    }

    public void deleteItem() //TODO doesn't work when dragged to a new position
    {
        Inventory.items.RemoveAt(currentTransform.GetSiblingIndex());
    }
}
