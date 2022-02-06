using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/Item", order = 1)]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public bool broken;
    public int value;
    public List<CharacterAction> actions;
    public Sprite sprite;

    public Item CopyVariables(Item item)
    {
        return CopyVariables(item.itemName, item.description, item.broken, item.value, item.actions);
    }

    public Item CopyVariables(string itemName, string description, bool broken, int value, List<CharacterAction> actions)
    {
        this.name = name;
        this.itemName = itemName;
        this.description = description;
        this.broken = broken;
        this.value = value;
        this.actions = new List<CharacterAction>(actions);
        foreach(CharacterAction action in actions)
        {
            this.actions.Add(action);
        }

        return this;
    }

    public virtual string GetExplicitString()
    {
        return ((broken) ? "Broken " : "") + itemName;
    }

    public virtual Item Copy()
    {
        //Item copy = ScriptableObject.CreateInstance<Item>().CopyVariables(itemName, description, broken, value, actions);
        Item copy = ScriptableObject.CreateInstance<Item>().CopyVariables(this);

        return copy;
    }

    public virtual bool Consume()
    {
        if (broken)
            return false;
        broken = true;
        return true;
    }
}