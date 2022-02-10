using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Item/Weapon", order = 1)]
public class Weapon : Item
{
    public Damage damage;
    public bool twoHanded;
    public Stat abilityToUse;

    public Weapon CopyWeaponVariables(Damage damage, bool twoHanded, Stat abilityToUse)
    {
        this.damage = damage;
        this.twoHanded = twoHanded;
        this.abilityToUse = abilityToUse;

        return this;
    }

    public virtual Damage GetDamage()
    {
        return damage;
    }

    public virtual bool LongRanged(){
        return false;
    }

    public override string GetExplicitString()
    {
        return ((damage.dieCount > 1) ? damage.dieCount.ToString() : "") + "d" + damage.dieSize + " " + (twoHanded ? "Two Handed " : " ") + itemName;
    }

    public override Item Copy()
    {
        Weapon copy = ScriptableObject.CreateInstance<Weapon>();
        copy.CopyVariables(itemName, description, broken, value, actions);
        copy.CopyWeaponVariables(damage, twoHanded, abilityToUse);

        return copy;
    }
}

