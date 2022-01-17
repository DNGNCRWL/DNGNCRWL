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

    public Item CopyItemVariables(string itemName, string description, bool broken, int value, List<CharacterAction> actions)
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
        Item copy = ScriptableObject.CreateInstance<Item>().CopyItemVariables(itemName, description, broken, value, actions);

        return copy;
    }

    public virtual void Consume()
    {
        broken = true;
    }
}