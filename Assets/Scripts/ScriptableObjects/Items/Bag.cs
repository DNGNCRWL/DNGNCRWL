using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bag", menuName = "Item/Bag", order = 1)]
public class Bag : Item
{
    static int MINIMUM_CARRYING_CAPACITY = 3;
    public int carryingCapacity;

    public Bag(string itemName, bool broken, int carryingCapacity, int value) : base(itemName, broken, value)
    {
        this.carryingCapacity = carryingCapacity;
    }

    public override Item Copy()
    {
        return new Bag(name, broken, carryingCapacity, value);
    }
}
