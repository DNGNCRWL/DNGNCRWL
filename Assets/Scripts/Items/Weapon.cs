using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Item/Weapon", order = 1)]
public class Weapon : Item
{
    public int dieCount;
    public int damage;
    public bool twoHanded;

    public Weapon(string itemName, int dieCount, int damage, bool twoHanded, bool broken, int value) : base(itemName, broken, value)
    {
        this.dieCount = dieCount;
        this.damage = damage;
        this.twoHanded = twoHanded;
    }
    //two handed constructors
    public Weapon(string itemName, int dieCount, int damage, bool twoHanded) : this(itemName, dieCount, damage, twoHanded, false, 0) { }
    public Weapon(string itemName, int damage, bool twoHanded) : this(itemName, 1, damage, twoHanded) { }

    //one handed constructors
    public Weapon(string itemName, int dieCount, int damage) : this(itemName, dieCount, damage, false) { }
    public Weapon(string itemName, int damage) : this(itemName, 1, damage) { }

    public virtual int calculateDamage()
    {
        return GameManager.RollDie(damage);
    }

    public override string GetExplicitString()
    {
        return ((dieCount > 1) ? dieCount.ToString() : "") + "d" + damage + " " + (twoHanded ? "Two Handed " : " ") + name;
    }

    public override Item Copy()
    {
        return new Weapon(name, dieCount, damage, twoHanded, broken, value);
    }
}