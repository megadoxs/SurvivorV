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
        image.sprite = itemStack.GetItem().Icon;
        text.text = $"{itemStack.GetItem().Name}({itemStack.GetCount()})";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!playerInside && other.CompareTag("Player"))
        {
            GameObject.FindWithTag("Player").GetComponent<Inventory>().AddItem(itemStack);
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        playerInside = false;
    }
}
