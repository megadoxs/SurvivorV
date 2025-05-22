using System;
using UnityEngine;

[Serializable]
public class ItemStack
{
    [SerializeField]
    private Item item;
    [SerializeField]
    private int count;

    public ItemStack(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }

    public void SetCount(int count)
    {
        this.count = Mathf.Min(count, item.MaxStackSize);
    }

    public int AddStackSize(int count)
    {
        var toAdd = Mathf.Min(count, Mathf.Max(0, item.MaxStackSize - this.count));
        this.count += toAdd;
        return count - toAdd;
    }
    
    public int RemoveStackSize(int count)
    {
        var toRemove = Mathf.Min(count, this.count);
        this.count -= toRemove;
        return count - toRemove;
    }

    public int GetCount()
    {
        return count;
    }

    public Item GetItem()
    {
        return item;
    }

    public ItemStack Clone()
    {
        return new ItemStack(item, count);
    }
}
