using System.Collections.Generic;

public static class Items
{
    public static readonly Dictionary<string, Item> itemDictionary = new();
    public static readonly Item Wood = Register(new CraftingItem("Wood", "Dropped from a three"));
    public static readonly Item Bone = Register(new CraftingItem("Bone", "A skeleton's bone"));

    private static Item Register(Item item)
    {
        itemDictionary.Add(item.GetName(), item);
        return item;
    }

    public static Item GetItem(string name)
    {
        return itemDictionary[name];
    }
}
