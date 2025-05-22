using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemStackData //TODO can probably be deleted now that ItemStack is serializable
{
    public string name;
    public int count;

    public ItemStackData(string name, int count)
    {
        this.name = name;
        this.count = count;
    }

    public ItemStack GetItemStack()
    {
        return new ItemStack(Resources.Load<Item>("Items/" + name.ToLower().Replace(" ", "_")), count);
    }

    public static List<ItemStack> GetItemStacks(List<ItemStackData> items)
    {
        List <ItemStack> itemStacks = new();
        foreach (var item in items)
        {
            itemStacks.Add(item.GetItemStack());
        }
        return itemStacks;
    }

    public static List<ItemStackData> SaveItemStacks(List<ItemStack> items)
    {
        List <ItemStackData> itemStacks = new();
        foreach (var item in items)
        {
            itemStacks.Add(new ItemStackData(item.GetItem().name, item.GetCount()));
        }
        return itemStacks;
    }
}
