using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// [System.Serializable]
public class Inventory
{
    // [SerializeField]
    public List<Item> itemList;

    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;
    public event EventHandler OnItemListChanged;

    public Inventory() {
        itemList = new List<Item>();
        //AddItem(new Item {itemName = "Femur", sprite = Addressables.LoadAssetAsync<Sprite[]>("Assets/Images/Sprites/Weapons/Femur.png")});
        AddItem(ItemManager.IM.startingWeapons[0].Copy());
        AddItem(ItemManager.IM.startingWeapons[1].Copy());
        AddItem(ItemManager.IM.startingWeapons[2].Copy());
        AddItem(ItemManager.IM.startingWeapons[3].Copy());
        AddItem(ItemManager.IM.startingWeapons[4].Copy());
        AddItem(ItemManager.IM.startingWeapons[5].Copy());
        AddItem(ItemManager.IM.startingWeapons[6].Copy());
        AddItem(ItemManager.IM.startingWeapons[7].Copy());
        AddItem(ItemManager.IM.startingWeapons[8].Copy());
        AddItem(ItemManager.IM.startingWeapons[9].Copy());
        Debug.Log("Inventory Init");
    }

    public void ReplaceInventory (List<Item> newInv) {
        itemList = newInv;
    }

    public void AddItem(Item item) {
        itemList.Add(item);
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public List<Item> GetItemList() {
        return itemList;
    }
}
