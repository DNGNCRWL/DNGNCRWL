using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile Weapon", menuName = "Item/Projectile Weapon", order = 1)]
public class ProjectileWeapon : Weapon
{
    public string ammoName;

    public ProjectileWeapon(string itemName, int dieCount, int damage, string ammoName, bool twoHanded, bool broken, int value)
        : base(itemName, dieCount, damage, twoHanded, broken, value)
    {
        this.ammoName = ammoName;
    }

    public ProjectileWeapon(string itemName, int dieCount, int damage, string ammoName, bool twoHanded)
        : this(itemName, dieCount, damage, ammoName, twoHanded, false, 0) { }
    public ProjectileWeapon(string itemName, int damage, string ammoName, bool twoHanded)
    : this(itemName, 1, damage, ammoName, twoHanded) { }

    public ProjectileWeapon(string itemName, int dieCount, int damage, string ammoName) : this(itemName, dieCount, damage, ammoName, false) { }
    public ProjectileWeapon(string itemName, int damage, string ammoName) : this(itemName, 1, damage, ammoName) { }

    public override string GetExplicitString()
    {
        return base.GetExplicitString();
    }

    public override Item Copy()
    {
        return new ProjectileWeapon(name, dieCount, damage, ammoName, twoHanded, broken, value);
    }
}