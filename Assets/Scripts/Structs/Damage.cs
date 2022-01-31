using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Damage // Should this be its own class?
{
    public Damage(int dieCount, int dieSize, int modifier, DamageType damageType)
    {
        this.dieCount = dieCount;
        this.dieSize = dieSize;
        this.modifier = modifier;
        this.damageType = damageType;
    }

    public int dieCount;
    public int dieSize;
    public int modifier;
    public DamageType damageType;
}

public struct DamageReturn
{
    public DamageReturn(int damageDone, string message, bool killerBlow)
    {
        this.damageDone = damageDone;
        this.message = message;
        this.killerBlow = killerBlow;
    }

    public int damageDone;
    public string message;
    public bool killerBlow;
}

[System.Serializable]
public enum DamageType { Untyped, Bludgeon, Cut, Electric, Fire, Magic, Pierce, Spirit }

[System.Serializable]
public struct Resistances
{
    public Resistances(int bludgeon, int cut, int electric, int fire, int magic, int pierce, int spirit)
    {
        this.bludgeon = bludgeon;
        this.cut = cut;
        this.electric = electric;
        this.fire = fire;
        this.magic = magic;
        this.pierce = pierce;
        this.spirit = spirit;
    }

    public int bludgeon;
    public int cut;
    public int electric;
    public int fire;
    public int magic;
    public int pierce;
    public int spirit;
}