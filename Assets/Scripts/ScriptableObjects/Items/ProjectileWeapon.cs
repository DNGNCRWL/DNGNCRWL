using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile Weapon", menuName = "Item/Projectile Weapon", order = 1)]
public class ProjectileWeapon : Weapon
{
    public string ammoName;

    public ProjectileWeapon
        Set(string itemName, string description, bool broken, int value, Damage damage, bool twoHanded, Stat abilityToUse, string ammoName)
    {
        this.itemName = itemName;
        this.description = description;
        this.broken = broken;
        this.value = value;

        this.damage = damage;
        this.twoHanded = twoHanded;
        this.abilityToUse = abilityToUse;

        this.ammoName = ammoName;

        return this;
    }

    public override string GetExplicitString()
    {
        return base.GetExplicitString();
    }

    public override Item Copy()
    {
        ProjectileWeapon copy = ScriptableObject.CreateInstance<ProjectileWeapon>();
        copy.CopyVariables(itemName, description, broken, value, actions, sprite);
        copy.CopyWeaponVariables(damage, twoHanded, abilityToUse);

        copy.ammoName = this.ammoName;

        return copy;
    }
}