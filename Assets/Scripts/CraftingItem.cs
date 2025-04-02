using UnityEngine;

public class CraftingItem : Item
{
    public CraftingItem(string name, string descripton) : base(name, descripton) { }

    public override int GetMaxStackSize()
    {
        return 100;
    }
}
