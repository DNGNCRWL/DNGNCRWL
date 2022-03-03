using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class Inventory
{
    // [SerializeField]
    public List<Item> itemList;

    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;
    public event EventHandler OnItemListChanged;

    public Bag storage = null;

    public Armor armor = null;

    public Inventory() {
        itemList = new List<Item>();
        //AddItem(new Item {itemName = "Femur", sprite = Addressables.LoadAssetAsync<Sprite[]>("Assets/Images/Sprites/Weapons/Femur.png")});
        //AddItem(ItemManager.armory.startingWeapons[0].Copy(), 3);
        // AddItem(ItemManager.IM.STARTING_WEAPONS[1].Copy(), 2);
        // AddItem(ItemManager.IM.STARTING_WEAPONS[2].Copy());
        // AddItem(ItemManager.IM.STARTING_WEAPONS[3].Copy());
        // AddItem(ItemManager.IM.STARTING_WEAPONS[4].Copy());
        // AddItem(ItemManager.IM.STARTING_WEAPONS[5].Copy());
        // AddItem(ItemManager.IM.STARTING_WEAPONS[6].Copy());
        // AddItem(ItemManager.IM.STARTING_WEAPONS[7].Copy());
        // AddItem(ItemManager.IM.STARTING_WEAPONS[8].Copy());
        // AddItem(ItemManager.IM.STARTING_WEAPONS[9].Copy());
        Debug.Log("Inventory Init");
    }

    public void ReplaceInventory (List<Item> newInv) {
        itemList = newInv;
    }

    public void AddItem(Item item, int qty = 1) {
        if (item == null) return;

        bool present = false;
        foreach (Item invItem in itemList)
        {
            if (item.itemName == invItem.itemName)
            {
                present = true;
                invItem.amount += qty;
                break;
            }
        }

        if (!present){
            item.amount = qty;
            itemList.Add(item);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty); //refresh UI
    }

    public List<Item> GetItemList() {
        return itemList;
    }

    public bool UseAmmo(ProjectileWeapon weapon) {

        string ammoName = weapon.ammoName;
        Ammo ammo = null;

        foreach (Item i in itemList)
        {
            if (i.itemName == ammoName)
            {
                ammo = (Ammo)i;
                ammo.Consume();

            }
        }

        if (ammo != null)
        {
            Debug.Log(ammo.itemName);
            Debug.Log("This much: " + ammo.amount);
        }


        OnItemListChanged?.Invoke(this, EventArgs.Empty); //refresh UI

        return ammo!= null;
    }
    
    public bool UseConsumable(Consumable consumable)
    {

        string consumeName = consumable.itemName;
        Consumable consume = null;

        foreach (Item i in itemList)
        {
            if (i.itemName == consumeName)
            {
                consume = (Consumable)i;
                consume.Consume();

            }
        }

        if (consume != null)
        {
            Debug.Log(consume.itemName);
            Debug.Log("This much: " + consume.amount);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty); //refresh UI

        return consume != null;
        
    }

    public void ChangeStorage(Bag bag) {
        AddItem(storage);
        storage = bag;
    }

/// <summary>
/// 
/// </summary>
/// <param name="newArmor">testna</param>
    public void ChangeArmor(Armor newArmor)
    {
        AddItem(armor);
        armor = newArmor;
    }

}
