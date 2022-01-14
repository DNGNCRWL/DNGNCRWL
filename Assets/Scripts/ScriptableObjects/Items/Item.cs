using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/Item", order = 1)]
public class Item : ScriptableObject
{
    public string itemName;
    public bool broken;
    public int value;
    public List<CharacterAction> actions;

    public virtual Item Set(string itemName, bool broken, int value)
    {
        this.itemName = itemName;
        this.broken = broken;
        this.value = value;

        return this;
    }

    public virtual string GetExplicitString()
    {
        return ((broken) ? "Broken" : "") + itemName;
    }

    public virtual Item Copy()
    {
        return ScriptableObject.CreateInstance<Item>().Set(itemName, broken, value);
    }
}