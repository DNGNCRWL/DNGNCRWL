using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armory", menuName = "Armory", order = 1)]
public class Armory : ScriptableObject
{
    public Bag[] bags;
    public Item[] adventureTools;
    public Item[] specialItems;
    public Item[] startingWeapons;
    public Armor[] startingArmors;
    public Armor[] startingArmorsLowtier;

    public Weapon unequippedHumanWeapon;
    public Armor unequippedHumanArmor;
    public WeaponPair[] weaponPairs;

    [Serializable]
    public struct WeaponPair {
     public Weapon weapon;
     public Item pair;
    }
}
