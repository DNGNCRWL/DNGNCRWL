using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Action", menuName = "Action", order = 1)]
public class CharacterAction : ScriptableObject
{
    public string actionName;
    public string verb;

    bool targetHead;
    bool calculatedHeadhunt = false;
    bool needsTarget;
    bool calculatedTarget = false;

    [Header("TESTS")]
    public d20[] tests;
    public int difficultyRating;
    public bool testAtDisadvantage;

    Func<ParameteredAtomicFunction, String> startDescriptor;
    Func<ParameteredAtomicFunction, String> actionDescriptor;
    Func<ParameteredAtomicFunction, String> resultDescriptor;

    [Header("RESULTS")]
    public List<ParameteredAtomicFunction> success;
    public List<ParameteredAtomicFunction> critical;
    public List<ParameteredAtomicFunction> failure;
    public List<ParameteredAtomicFunction> fumble;

    public bool TargetHead()
    {
        if (calculatedHeadhunt)
            return targetHead;

        targetHead = (
           TargetHead(success) ||
           TargetHead(critical) ||
           TargetHead(failure) ||
           TargetHead(fumble)
           );

        calculatedHeadhunt = true;

        return targetHead;
    }
    bool TargetHead(List<ParameteredAtomicFunction> list)
    {
        foreach (ParameteredAtomicFunction paf in list)
        {
            switch (paf.atomicFunction)
            {
                case EnumeratedAtomicFunction.Sneak:
                    return true;
            }
        }

        return false;
    }

    public bool NeedsTarget()
    {
        if (calculatedTarget)
            return needsTarget;
        
        needsTarget = (
           NeedsTarget(success) ||
           NeedsTarget(critical) ||
           NeedsTarget(failure) ||
           NeedsTarget(fumble)
           );

        calculatedTarget = true;

        return needsTarget;
    }
    bool NeedsTarget(List<ParameteredAtomicFunction> list)
    {
        foreach (ParameteredAtomicFunction paf in list)
        {
            switch (paf.atomicFunction)
            {
                case EnumeratedAtomicFunction.TargetAgility:
                case EnumeratedAtomicFunction.TargetBleeding:
                case EnumeratedAtomicFunction.TargetBlind:
                case EnumeratedAtomicFunction.TargetBludgeonResist:
                case EnumeratedAtomicFunction.TargetCutResist:
                case EnumeratedAtomicFunction.TargetDistracted:
                case EnumeratedAtomicFunction.TargetElectricResist:
                case EnumeratedAtomicFunction.TargetEquip:
                case EnumeratedAtomicFunction.TargetFireResist:
                case EnumeratedAtomicFunction.TargetHands:
                case EnumeratedAtomicFunction.TargetHP:
                case EnumeratedAtomicFunction.TargetInfection:
                case EnumeratedAtomicFunction.TargetLegs:
                case EnumeratedAtomicFunction.TargetMagicResist:
                case EnumeratedAtomicFunction.TargetMaxHP:
                case EnumeratedAtomicFunction.TargetPierceResist:
                case EnumeratedAtomicFunction.TargetPresence:
                case EnumeratedAtomicFunction.TargetStrength:
                case EnumeratedAtomicFunction.TargetToughness:

                case EnumeratedAtomicFunction.TargetTakeWeaponDamage:

                case EnumeratedAtomicFunction.Fight:
                case EnumeratedAtomicFunction.Push:
                    return true;
            }
        }

        return false;
    }
}