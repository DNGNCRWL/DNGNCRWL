using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stackable", menuName = "Item/Stackable", order = 1)]
public class Stackable : Item
{
    public string unit;
    public int amount;

    public Stackable(string itemName, string unit, int amount, bool broken, int value) : base(itemName, broken, value)
    {
        this.unit = unit;
        this.amount = amount;
    }

    public Stackable(string itemName, string unit, int amount) : this(itemName, unit, amount, false, 0) { }

    public override string GetExplicitString()
    {
        return base.GetExplicitString() + " with " + amount + " " + unit + ((amount > 1) ? "s" : "");
    }

    public override Item Copy()
    {
        return new Stackable(name, unit, amount, broken, value);
    }
}