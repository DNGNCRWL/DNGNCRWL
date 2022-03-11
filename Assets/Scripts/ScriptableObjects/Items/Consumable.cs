using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "Item/Consumable", order = 1)]
public class Consumable : Item
{
    public override string GetExplicitString()
    {
        if (broken) return base.GetExplicitString();
        else if (amount <= 0) return "Empty " + itemName;
        else return itemName + " with " + amount;
    }

    public override bool Consume()
    {
        if (amount <= 0)
            return false;
        amount--;
        Debug.Log(amount);
        return true;
    }
}