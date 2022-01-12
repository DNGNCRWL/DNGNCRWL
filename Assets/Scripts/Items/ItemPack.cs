using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemPack", menuName = "Item/Item Pack", order = 1)]
public class ItemPack : Item
{
    public Item[] items;

    public ItemPack(Item[] items) : base(items.Length.ToString() + " Item Pack")
    {
        this.items = items;
    }

    public override string GetExplicitString()
    {
        List<Item> asList = new List<Item>(items);
        return base.GetExplicitString() + ": " + ItemManager.ItemListToExplicitString(new List<Item>(items));
    }
}
