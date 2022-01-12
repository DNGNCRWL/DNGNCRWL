using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "Item/Armor", order = 1)]
public class Armor : Item
{
    public int armorTier;
    public int maxArmorTier;

    public Armor(string itemName, int armorTier, int maxArmorTier, bool broken, int value) : base(itemName, broken, value)
    {
        this.armorTier = armorTier;
        this.maxArmorTier = maxArmorTier;
    }

    public Armor(string itemName, int armorTier, int maxArmorTier) : this(itemName, armorTier, maxArmorTier, false, 0) { }
    public Armor(string itemName, int armorTier) : this(itemName, armorTier, armorTier) { }

    public override string GetExplicitString()
    {
        return "Tier " + armorTier + "/" + maxArmorTier + " " + name;
    }

    public override Item Copy()
    {
        return new Armor(name, armorTier, maxArmorTier, broken, value);
    }
}
