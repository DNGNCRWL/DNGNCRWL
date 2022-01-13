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

    public virtual string GetExplicitString()
    {
        return ((broken) ? "Broken" : "") + name;
    }

    public virtual Item Copy()
    {
        return new Item(name, broken, value);
    }
}