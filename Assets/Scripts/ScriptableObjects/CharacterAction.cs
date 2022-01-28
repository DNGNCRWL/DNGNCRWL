using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Action", menuName = "Action", order = 1)]
public class CharacterAction : ScriptableObject
{
    public string actionName;

    [Header("STARTING MESSAGES")]
    public MessagePackage startMessage;
    public MessagePackage actionMessage;

    [Header("TESTS")]
    public d20[] tests;
    public int difficultyRating;
    public bool testAtDisadvantage;

    [Header("RESULT MESSAGES")]
    public MessagePackage criticalMessage;
    public MessagePackage successMessage;
    public MessagePackage failMessage;
    public MessagePackage fumbleMessage;
    [Header("RESULT ACTIONS")]
    public List<ParameteredAtomicFunction> critical;
    public List<ParameteredAtomicFunction> success;
    public List<ParameteredAtomicFunction> failure;
    public List<ParameteredAtomicFunction> fumble;

    bool targetHead;
    bool calculatedHeadhunt = false;
    bool needsTarget;
    bool calculatedTarget = false;

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
                case EnumeratedAtomicFunction.TargetDistracted:
                case EnumeratedAtomicFunction.TargetEquip:
                case EnumeratedAtomicFunction.TargetHands:
                case EnumeratedAtomicFunction.TargetHP:
                case EnumeratedAtomicFunction.TargetInfection:
                case EnumeratedAtomicFunction.TargetLegs:
                case EnumeratedAtomicFunction.TargetMaxHP:
                case EnumeratedAtomicFunction.TargetPresence:
                case EnumeratedAtomicFunction.TargetResist:
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

[System.Serializable]
public struct MessagePackage
{
    public string[] messages;
    public float time;
    [SerializeField, Range(0, 1)]
    public float weight;
    [HideInInspector] public int index;
}