using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stackable", menuName = "Item/Stackable", order = 1)]
public class Stackable : Item
{
    public string unit;
    public int amount;

    public override string GetExplicitString()
    {
        if (broken) return base.GetExplicitString();
        else if (amount <= 0) return "Empty " + itemName;
        else return itemName + " with " + amount + " " + unit + ((amount > 1) ? "s" : "");
    }

    public override Item Copy()
    {
        Stackable copy = ScriptableObject.CreateInstance<Stackable>();
        copy.CopyItemVariables(itemName, description, broken, value, actions);

        copy.unit = unit;
        copy.amount = amount;

        return copy;
    }

    public override void Consume()
    {
        amount--;
        if (amount <= 0)
            broken = true;
    }
}