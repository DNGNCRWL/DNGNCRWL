using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/Item", order = 1)]
public class Item : ScriptableObject
{
    public string name;
    public bool broken;
    public int value;

    public Item(string itemName, bool broken, int value)
    {
        this.name = itemName;
        this.broken = broken;
        this.value = value;
    }

    public Item(string itemName, bool broken) : this(itemName, broken, 0) { }
    public Item(string itemName, int value) : this(itemName, false, value) { }
    public Item(string itemName) : this(itemName, false, 0) { }

    public virtual string GetExplicitString()
    {
        return ((broken) ? "Broken" : "") + name;
    }

    public virtual Item Copy()
    {
        return new Item(name, broken, value);
    }
}