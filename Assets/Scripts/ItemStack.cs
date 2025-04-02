using UnityEngine;

public class ItemStack
{
    private readonly Item item;
    private int count;

    public ItemStack(Item item): this(item, 0){}

    public ItemStack(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }

    public void SetCount(int count) //TODO care about max stack size
    {
        this.count = count;
    }

    public void addCount(int count) //TODO only keep this or addstacksize
    {
        this.count += count;
    }

    public virtual int AddStackSize(int count)
    {
        if (this.count < item.GetMaxStackSize())
        {
            int left = this.count + count - item.GetMaxStackSize();
            this.count += count - left;
            return left;
        }
        return count;
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
