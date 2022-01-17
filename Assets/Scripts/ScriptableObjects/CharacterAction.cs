using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action", menuName = "Action", order = 1)]
public class CharacterAction : ScriptableObject
{
    public string actionName;
    public ActionTarget actionTarget;
    public ParameteredAtomicFunction[] doTheseThings;
    
    /*EAF 2 Atomic Function
     * 
     * This takes in an Enumerated Atomic Function and returns the actual function.
     * Enums can be made visible in the Unity Editor, so users can create a new Action
     * and surface the action. This function will grow as we add functions
     */
    public static Action<ParameteredAtomicFunction> EAF2atomicFunction(EnumeratedAtomicFunction eaf)
    {
        switch (eaf)
        {
            //self actions
            case EnumeratedAtomicFunction.CureBleeding: return ActionManager.CureBleeding;
            case EnumeratedAtomicFunction.CureBlinded: return ActionManager.CureBlinded;
            case EnumeratedAtomicFunction.CureDisabled: return ActionManager.CureDisabled;
            case EnumeratedAtomicFunction.CureDistracted: return ActionManager.CureDistracted;
            case EnumeratedAtomicFunction.CureHands: return ActionManager.CureHands;
            case EnumeratedAtomicFunction.CureInfection: return ActionManager.CureInfection;
            case EnumeratedAtomicFunction.DamageOtherSide: return ActionManager.DamageOtherSide;
            case EnumeratedAtomicFunction.IncreaseMaxHP: return ActionManager.IncreaseMaxHP;
            case EnumeratedAtomicFunction.IncreaseStrength: return ActionManager.IncreaseStrength;
            case EnumeratedAtomicFunction.IncreaseAgility: return ActionManager.IncreaseAgility;
            case EnumeratedAtomicFunction.IncreasePresence: return ActionManager.IncreasePresence;
            case EnumeratedAtomicFunction.IncreaseToughness: return ActionManager.IncreaseToughness;
            case EnumeratedAtomicFunction.RecoverDamage: return ActionManager.RecoverDamage;

            //other actions
            case EnumeratedAtomicFunction.Fight: return ActionManager.Fight;
            case EnumeratedAtomicFunction.Push: return ActionManager.Fight;
            default: return null;
        }
    }

    public void ExecuteAction(CharacterSheet actor, CharacterSheet target, Item item)
    {
        if (actionTarget != ActionTarget.Other)
            return;

        for (int i = 0; i < doTheseThings.Length; i++)
        {
            doTheseThings[i].actor = actor;
            doTheseThings[i].target = target;
            doTheseThings[i].item = item;
            EAF2atomicFunction(doTheseThings[i].atomicFunction)(doTheseThings[i]);
        }
    }

    public void ExecuteAction(CharacterSheet actor, CharacterSheet target)
    {
        if (actionTarget != ActionTarget.Other)
            return;

        for(int i = 0; i < doTheseThings.Length; i++)
        {
            doTheseThings[i].actor = actor;
            doTheseThings[i].target = target;
            EAF2atomicFunction(doTheseThings[i].atomicFunction)(doTheseThings[i]);
        }
    }

    public void ExecuteAction(CharacterSheet actor, Item item)
    {
        if (actionTarget != ActionTarget.Self)
            return;

        for (int i = 0; i < doTheseThings.Length; i++)
        {
            doTheseThings[i].actor = actor;
            doTheseThings[i].target = actor;
            doTheseThings[i].item = item;
            EAF2atomicFunction(doTheseThings[i].atomicFunction)(doTheseThings[i]);
        }
    }

    public void ExecuteAction(CharacterSheet actor)
    {
        if (actionTarget != ActionTarget.Self)
            return;

        for (int i = 0; i < doTheseThings.Length; i++)
        {
            doTheseThings[i].actor = actor;
            doTheseThings[i].target = actor;
            EAF2atomicFunction(doTheseThings[i].atomicFunction)(doTheseThings[i]);
        }
    }
}

[System.Serializable]
public enum ActionTarget { Self, Other };

[System.Serializable]
public enum EnumeratedAtomicFunction
{
    Nothing,
    CureBleeding,
    CureBlinded,
    CureDisabled,
    CureDistracted,
    CureHands,
    CureInfection,
    DamageOtherSide,
    IncreaseMaxHP,
    IncreaseStrength,
    IncreaseAgility,
    IncreasePresence,
    IncreaseToughness,
    RecoverDamage,

    Fight,
    Push
}

[System.Serializable]
public struct ParameteredAtomicFunction
{
    public EnumeratedAtomicFunction atomicFunction;
    public int dieCount;
    public int dieSize;
    public Damage damage;
    [HideInInspector] public CharacterSheet actor;
    [HideInInspector] public CharacterSheet target;
    [HideInInspector] public Item item;
}