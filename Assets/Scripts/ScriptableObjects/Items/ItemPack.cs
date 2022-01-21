using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPack", menuName = "Item/Item Pack", order = 1)]
public class ItemPack : Item
{
    public Item[] items;

    public ItemPack Set(string description, bool broken, int value, Item[] items)
    {
        this.itemName = items.Length.ToString() + " Item Pack";
        this.description = description;
        this.broken = broken;
        this.value = value;

        this.items = items;

        return this;
    }

    public override string GetExplicitString()
    {
        List<Item> asList = new List<Item>(items);
        return base.GetExplicitString() + ": " + ItemManager.ItemListToExplicitString(new List<Item>(items));
    }

    public override Item Copy()
    {
        ItemPack copy = ScriptableObject.CreateInstance<ItemPack>();
        copy.CopyVariables(itemName, description, broken, value, actions);

        Item[] copyItems = new Item[items.Length];
        for (int i = 0; i < items.Length; i++)
            copyItems[i] = items[i].Copy();

        copy.items = copyItems;

        return copy;
    }
}
