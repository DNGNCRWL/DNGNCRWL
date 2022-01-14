using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Item/Weapon", order = 1)]
public class Weapon : Item
{
    public int dieCount;
    public int damage;
    public bool twoHanded;
    public Stat abilityToUse;

    public Weapon Set(string itemName, int dieCount, int damage, bool twoHanded, Stat abilityToUse, bool broken, int value)
    {
        this.itemName = itemName;
        this.dieCount = dieCount;
        this.damage = damage;
        this.twoHanded = twoHanded;
        this.abilityToUse = abilityToUse;
        this.broken = broken;
        this.value = value;

        return this;
    }

    public virtual int CalculateDamage()
    {
        return GameManager.RollDie(damage);
    }

    public virtual int CalculateCriticalDamage()
    {
        return GameManager.RollDie(damage) * 2;
    }

    public override string GetExplicitString()
    {
        return ((dieCount > 1) ? dieCount.ToString() : "") + "d" + damage + " " + (twoHanded ? "Two Handed " : " ") + itemName;
    }

    public override Item Copy()
    {
        return ScriptableObject.CreateInstance<Weapon>().Set(itemName, dieCount, damage, twoHanded, abilityToUse, broken, value);
    }
}