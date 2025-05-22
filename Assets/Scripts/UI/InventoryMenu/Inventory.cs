using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MenuSwitcher, IPlayerDataSaver
{
    public GameObject inventoryMenu;
    public int inventorySize = 20;

    [Space] [Header("Items")] public GameObject inventoryPannel;
    public GameObject item;
    public List<ItemStack> items = new();
    private List<GameObject> slots = new();
    
    public static Inventory instance { get; private set; }
    
    private void Awake()
    {
        instance = this;
    }

    private void OnInventory()
    {
        if (!PauseMenu.isPaused && GetComponent<PlayerController>().alive && !GameOver.gameOver)
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

    private void RefreshInventory()
    {
        foreach (var item in slots) Destroy(item);
        slots.Clear();
        items.RemoveAll(i => i.GetCount() == 0);
        
        for (var index = 0; index < items.Count; index++)
        {
            var newPanel = Instantiate(item, inventoryPannel.transform);
            var slot = newPanel.GetComponent<DragItemSlot>();

            slot.SetItemStack(items[index]);
            slot.SetItemStackIndex(index);
            slot.inventory = inventoryMenu;

            slots.Add(newPanel);
        }
    }

    public void AddItem(ItemStack item)
    {
        foreach (var i in items)
        {
            if(item.GetCount() == 0)
                break;
        
            if (i.GetItem() == item.GetItem())
                item.SetCount(i.AddStackSize(item.GetCount()));   
        }

        while (items.Count < inventorySize && item.GetCount() > 0)
        {
            items.Add(item.Clone());
            item.SetCount(0);
        }

        if (inventoryMenu.activeSelf)
            RefreshInventory();
        
        BuildManager.instance.ValidatePrices();
    }

    public void AddItems(List<ItemStack> items)
    {
        foreach (var item in items) AddItem(item);
    }

    public bool HasItems(ItemStack[] items)
    {
        foreach (var required in items)
        {
            var totalCount = this.items
                .Where(i => i.GetItem() == required.GetItem())
                .Sum(i => i.GetCount());

            if (totalCount < required.GetCount())
                return false;
        }

        return true;
    }

    public int GetItemCount(Item item)
    {
        return items.Where(i => i.GetItem() == item).Sum(i => i.GetCount());;
    }

    public void RemoveItems(ItemStack[] items)
    {
        foreach (var item in items) RemoveItem(item);
    }
    
    public void RemoveItem(ItemStack item)
    {
        foreach (var i in items)
        {
            if(item.GetCount() == 0)
                break;
            
            if (i.GetItem() == item.GetItem())
                item.SetCount(i.RemoveStackSize(item.GetCount()));
        }

        if (inventoryMenu.activeSelf)
            RefreshInventory();
        
        BuildManager.instance.ValidatePrices();
    }

    public void LoadData(PlayerData data)
    {
        foreach (var item in slots) Destroy(item);
        slots.Clear();
        items.Clear();
        AddItems(ItemStackData.GetItemStacks(data.items));
    }

    public void SaveData(ref PlayerData data)
    {
        data.items = ItemStackData.SaveItemStacks(items);
    }
}