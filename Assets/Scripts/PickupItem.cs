using TMPro;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public SpriteRenderer image;
    public TextMeshPro text;
    private ItemStack itemStack;
    public bool playerInside;

    public void SetItemStack(ItemStack itemStack)
    {
        this.itemStack = itemStack;
        image.sprite = itemStack.GetItem().GetSprite();
        text.text = $"{itemStack.GetItem().GetName()}({itemStack.GetCount()})";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!playerInside)
        {
            Inventory.items.Add(itemStack);
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        playerInside = false;
    }
}
