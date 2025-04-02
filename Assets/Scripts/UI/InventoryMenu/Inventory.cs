using System.Collections.Generic;
using UnityEngine;

public class Inventory : MenuSwitcher, IPlayerDataSaver
{
    public GameObject inventoryMenu;
    public int inventorySize = 20;

    [Space] [Header("Items")] public GameObject inventoryPannel;
    public GameObject item;
    public static List<ItemStack> items = new();
    private List<GameObject> slots = new();

    private void OnInventory()
    {
        if (!PauseMenu.isPaused)
        {
            if (inventoryMenu.activeSelf)
            {
                inventoryMenu.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Reset();
            }
            else
            {
                inventoryMenu.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                RefreshInventory();
            }
        }
    }

    public void RefreshInventory()
    {
        foreach (var item in slots) Destroy(item);
        slots.Clear();

        foreach (var i in items)
        {
            var newPanel = Instantiate(item, inventoryPannel.transform);
            var slot = newPanel.GetComponent<DragItemSlot>();
            slot.SetItemStack(i);
            slot.inventory = inventoryMenu;
            slots.Add(newPanel);
        }
    }

    public void AddItem(ItemStack item)
    {
        foreach (var i in items)
            if (i.GetItem().GetName() == item.GetItem().GetName())
                item.SetCount(i.AddStackSize(item.GetCount()));

        while (items.Count < inventorySize && item.GetCount() > 0)
        {
            items.Add(item.Clone());
            item.SetCount(0);
        }

        if (inventoryMenu.activeSelf)
            RefreshInventory();
    }

    public void AddItems(List<ItemStack> items)
    {
        foreach (var item in items) AddItem(item);
    }

    public void LoadData(PlayerData data)
    {
        AddItems(ItemStackData.GetItemStacks(data.items));
    }

    public void SaveData(ref PlayerData data)
    {
        data.items = ItemStackData.SaveItemStacks(items);
    }
}