using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Item/Weapon", order = 1)]
public class Weapon : Item
{
    public Damage damage;
    public bool twoHanded;
    public Stat abilityToUse;
    public AnimationClip effectAnimation;
    public AnimationClip criticalEffectAnimation;

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
}

