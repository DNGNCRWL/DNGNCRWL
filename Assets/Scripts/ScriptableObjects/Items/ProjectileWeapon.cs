using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile Weapon", menuName = "Item/Projectile Weapon", order = 1)]
public class ProjectileWeapon : Weapon
{
    public string ammoName;

    public ProjectileWeapon
        Set(string itemName, int dieCount, int damage, string ammoName, bool twoHanded, Stat abilityToUse, bool broken, int value)
    {
        this.itemName = itemName;
        this.dieCount = dieCount;
        this.damage = damage;
        this.ammoName = ammoName;
        this.twoHanded = twoHanded;
        this.abilityToUse = abilityToUse;
        this.broken = broken;
        this.value = value;

        return this;
    }

    public override string GetExplicitString()
    {
        return base.GetExplicitString();
    }

    public override Item Copy()
    {
        return ScriptableObject.CreateInstance<ProjectileWeapon>().Set(itemName, dieCount, damage, ammoName, twoHanded, abilityToUse, broken, value);
    }
}