using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Item/Item", order = 1)]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public bool broken;
    public int value;
    public List<CharacterAction> actions;
    
    public List<CharacterAction> inventoryActions;
    public Sprite sprite;
    public int stackLimit = 1;
    public int amount = 1;

    public Sprite GetSprite() {
        return sprite;
    }

    public virtual string GetExplicitString()
    {
        return ((broken) ? "Broken " : "") + itemName;
    }

    public Item Copy()
    {
        if (this == null){
            Debug.LogError("Tried copying null item");
            return null;
        }
        Item copy = Instantiate(this);

        return copy;
    }

    public virtual bool Consume()
    {
        if (broken)
            return false;
        broken = true;
        return true;
    }

    public bool IsStackable() {
        if(stackLimit > 1)
            return true;
        else
            return false;
    } 
}