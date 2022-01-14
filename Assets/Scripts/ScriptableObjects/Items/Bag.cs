using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bag", menuName = "Item/Bag", order = 1)]
public class Bag : Item
{
    static int MINIMUM_CARRYING_CAPACITY = 3;
    public int carryingCapacity;

    public Bag Set(string itemName, bool broken, int carryingCapacity, int value)
    {
        this.itemName = itemName;
        this.broken = broken;
        this.carryingCapacity = carryingCapacity;
        this.value = value;

        return this;
    }

    public override Item Copy()
    {
        return ScriptableObject.CreateInstance<Bag>().Set(itemName, broken, carryingCapacity, value);
    }
}
