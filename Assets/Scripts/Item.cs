using UnityEngine;

public abstract class Item
{
    private string name;
    private string description;

    public Item(string name, string description)
    {
        this.name = name;
        this.description = description;
    }

    public virtual string GetName()
    {
        return name;
    }
    public virtual string GetDescription() {
        return description; 
    }
    public virtual Sprite GetSprite()
    {
        return Resources.Load<Sprite>("UI/Item Images/" + name.ToLower().Replace(" ", "_"));
    }

    public abstract int GetMaxStackSize();
}
