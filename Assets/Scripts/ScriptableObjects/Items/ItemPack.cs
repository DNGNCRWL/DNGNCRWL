using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPack", menuName = "Item/Item Pack", order = 1)]
public class ItemPack : Item
{
    public Item[] items;

    public ItemPack Set(Item[] items, bool broken, int value)
    {
        this.itemName = items.Length.ToString() + " Item Pack";
        this.items = items;
        this.broken = broken;
        this.value = value;

        return this;
    }

    public override string GetExplicitString()
    {
        List<Item> asList = new List<Item>(items);
        return base.GetExplicitString() + ": " + ItemManager.ItemListToExplicitString(new List<Item>(items));
    }

    public override Item Copy()
    {
        Item[] copy = new Item[items.Length];
        for (int i = 0; i < items.Length; i++)
            copy[i] = items[i].Copy();

        return ScriptableObject.CreateInstance<ItemPack>().Set(copy, broken, value);
    }
}
