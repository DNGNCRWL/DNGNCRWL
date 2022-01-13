using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile Weapon", menuName = "Item/Projectile Weapon", order = 1)]
public class ProjectileWeapon : Weapon
{
    public string ammoName;

    public ProjectileWeapon(string itemName, int dieCount, int damage, string ammoName, bool twoHanded, Stat abilityToUse, bool broken, int value)
        : base(itemName, dieCount, damage, twoHanded, abilityToUse, broken, value)
    {
        this.ammoName = ammoName;
    }

    public override string GetExplicitString()
    {
        return base.GetExplicitString();
    }

    public override Item Copy()
    {
        return new ProjectileWeapon(name, dieCount, damage, ammoName, twoHanded, abilityToUse, broken, value);
    }
}