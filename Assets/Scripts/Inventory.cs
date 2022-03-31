using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;

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

    public Weapon mainHand = null;
    public Weapon offHand = null;

    [SerializeField]
    private int slotsLimit = 10;
    [SerializeField]
    private int slotsUsed = 0;

    public Inventory() {
        itemList = new List<Item>();
        //AddItem(ItemManager.armory.startingWeapons[0].Copy(), 3);
        //Debug.Log("Inventory Init");
    }

    public void ReplaceInventory (List<Item> newInv) {
        itemList = newInv;
        UpdateSlotsUsed();
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
        
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

        UpdateSlotsUsed();
        OnItemListChanged?.Invoke(this, EventArgs.Empty); //refresh UI
        Debug.Log("Invoked Inventory Change");
    }

    public bool AddIfHasSpace(Item item, int qty = 1) {
        if(CheckHasSpace(item,qty)) {
            AddItem(item, qty);
            return true;
        } else
            return false;
    }

    public List<Item> GetItemList() {
        return itemList;
    }

    public int GetSlotsLimit() { return slotsLimit; }
    public int GetSlotsUsed() { return slotsUsed; }
    public bool RemoveItem(Item item, int amount = 1) {
        foreach (Item i in itemList) {
            if (i.itemName == item.itemName) {
                if ((i.amount - amount )<0) {
                    return false;
                } else {
                    i.amount -= amount;
                    if (i.amount == 0) {
                        itemList.Remove(i);
                    } 
                    UpdateSlotsUsed();
                    OnItemListChanged?.Invoke(this, EventArgs.Empty); //refresh UI
                    return true;
                }
            }
        }
        return false;
    }
    public bool UseAmmo(ProjectileWeapon weapon) {

        string ammoName = weapon.ammoName;
        Ammo ammo = null;

        foreach (Item i in itemList)
        {
            if (i.itemName == ammoName)
            {
                ammo = (Ammo)i;
                if(!RemoveItem(ammo , 1)) {
                    return false;
                }

            }
        }

        if (ammo != null)
        {
            Debug.Log(ammo.itemName);
            Debug.Log("This much: " + ammo.amount);
        }


        OnItemListChanged?.Invoke(this, EventArgs.Empty); //refresh UI
        UpdateSlotsUsed();

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
        UpdateSlotsUsed();

        return consume != null;
        
    }

    public Item GetItem(Item item) {
        foreach (Item i in itemList){
            if (i.itemName == item.itemName)
                return i;
        }
        return null;
    }
    public void SwapStorage(Bag bag) {
        AddItem(storage);
        storage = bag;
    }

    public void SetStorage(Bag bag) {
        storage = bag;
    }

    public Bag GetStorage() {
        return storage;
    }

/// <summary>
/// 
/// </summary>
/// <param name="newArmor">testna</param>
    public void SwapArmor(Armor newArmor)
    {
        AddItem(armor);
        armor = newArmor;
    }
    public void SetArmor(Armor newArmor)
    {
        armor = newArmor;
    }

    public Armor GetArmor() {
        return armor;
    }

    public void SwapMainHand(Weapon wep) {
        AddItem(mainHand);
        mainHand = wep;
    }
    public void SetMainHand(Weapon wep) {
        mainHand = wep;
    }
    public Weapon GetMainHand() {
        return mainHand;
    }

    public void SwapOffHand(Weapon wep) {
        AddItem(offHand);
        offHand = wep;
    }
    public void SetOffHand(Weapon wep) {
        offHand = wep;
    }
    public Weapon GetOffHand() {
        return offHand;
    }


    public void SetSlotLimit(int slotsLimit) {
        this.slotsLimit = slotsLimit;
    }

    private void UpdateSlotsUsed() {
        Debug.Log("Updated slots used");
        int usedSlots = 0;
        foreach (Item item in itemList) {
            int count = item.amount;
            int counted = 0;
            do
            {
                int remaining = count - counted;
                if (item.amount > 1 && item.IsStackable())
                {
                    if (remaining > item.stackLimit)
                    {
                        counted += item.stackLimit;
                    }
                    else
                    {
                        counted += remaining;
                    }
                }
                else
                {
                    counted++;
                }
                usedSlots++;
            } while (counted < count);
        }
        slotsUsed = usedSlots;
    }

    private bool CheckHasSpace (Item item, int qty = 1) {
        
        Item invItem = GetItem(item);
        if(invItem == null) {
            int slotsNeeded = (int)Math.Ceiling((double)qty / item.stackLimit);
            if ((slotsNeeded + slotsUsed) > slotsLimit)
                return false;
            else
                return true;
        } 
        int slotsAlreadyUsed = (int)Math.Ceiling((double)invItem.amount / invItem.stackLimit);
        int slotsToBeUsed = (int)Math.Ceiling(((double)invItem.amount + (double)qty)/ invItem.stackLimit);
        int newSlotsUsed = slotsToBeUsed - slotsAlreadyUsed;

        if((newSlotsUsed+slotsUsed) > slotsLimit)
            return false;
        else
            return true;

    }

    public bool IsOverencumbered () {
        return slotsUsed > slotsLimit;
    }
}
