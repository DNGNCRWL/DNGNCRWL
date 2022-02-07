using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bag", menuName = "Item/Bag", order = 1)]
public class Bag : Item
{
    public int carryingCapacity;

    public override Item Copy()
    {
        Bag copy = ScriptableObject.CreateInstance<Bag>();
        copy.CopyVariables(itemName, description, broken, value, actions, sprite);
        copy.carryingCapacity = this.carryingCapacity;

        return copy;
    }
}
