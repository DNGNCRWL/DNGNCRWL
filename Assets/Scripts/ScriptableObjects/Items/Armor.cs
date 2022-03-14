using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "Item/Armor", order = 1)]
public class Armor : Item
{
    public int armorTier;
    public int maxArmorTier;

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

    public void Break()
    {
        armorTier--;
        if (armorTier == 0)
            Consume();
    }

    public void Repair()
    {
        armorTier++;
        if (armorTier > maxArmorTier)
            armorTier = maxArmorTier;
    }
}
