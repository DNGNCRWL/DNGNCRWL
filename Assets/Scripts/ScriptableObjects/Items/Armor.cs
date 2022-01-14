using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "Item/Armor", order = 1)]
public class Armor : Item
{
    public int armorTier;
    public int maxArmorTier;

    public Armor Set(string itemName, int armorTier, int maxArmorTier, bool broken, int value)
    {
        this.itemName = itemName;
        this.armorTier = armorTier;
        this.maxArmorTier = maxArmorTier;
        this.broken = broken;
        this.value = value;

        return this;
    }

    public override string GetExplicitString()
    {
        return "Tier " + armorTier + "/" + maxArmorTier + " " + itemName;
    }

    public int AgilityPenalty()
    {
        if (maxArmorTier >= 3)
            return 4;
        if (maxArmorTier >= 2)
            return 2;
        return 0;
    }

    public int DefensePenalty()
    {
        if (maxArmorTier >= 2)
            return 2;
        return 0;
    }

    public override Item Copy()
    {
        return ScriptableObject.CreateInstance<Armor>().Set(itemName, armorTier, maxArmorTier, broken, value);
    }
}
