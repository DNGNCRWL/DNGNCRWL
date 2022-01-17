using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    static readonly int bombDifficultyRating = 10;

    static readonly int sneakDifficultyRating = 10;

    static readonly int fightDifficultyRating = 10;
    static readonly int sameSideRangedAttackPenalty = 2;
    static readonly int pushDifficultyRating = 10;

    public static ActionManager AM;

    private void Awake()
    {
        if (!AM) AM = this;
        else { Destroy(gameObject); return; }
    }

    public void StartAction(CharacterSheet actor, CharacterSheet target, Item item, CharacterAction action)
    {
        StartCoroutine(StartActionCR(actor, target, item, action));
    }

    IEnumerator StartActionCR(CharacterSheet actor, CharacterSheet target, Item item, CharacterAction action)
    {
        Stat actorStatToTest = FromStatSelectorToStat(action.chooseActorStatToTest, actor);
        Stat targetStatToTest = FromStatSelectorToStat(action.chooseTargetStatToTest, target);

        //START
        string startText = actor.GetCharacterName() + " " + action.verb;
        if (target && !item)
            startText += " " + target.GetCharacterName();
        else if (!target && item)
            startText += " " + item.itemName;
        else if (target && item)
            startText += " " + target.GetCharacterName() + " with " + item.itemName;

        if (BattleManager.BM)
        {
            BattleManager.BM.SetDialogueText(startText);
            yield return new WaitForSeconds(1);
        }

        if(action.alwaysDoThese.Count > 0)
            yield return ExecutePAFs(action.alwaysDoThese, new d20(), actor, target, item);

        if (action.useRoll)
        {
            //ACTION
            d20 roll = new d20(action.difficultyRating, actor, actorStatToTest, target, targetStatToTest);
            int effort = roll.Roll();

            Debug.Log("Roll value: " + roll.Roll());

            string actionText;
            if (effort <= 8)
                actionText = "What is going on?!";
            else if (effort <= 12)
                actionText = "It looks good";
            else
                actionText = "Amazing effort";

            if (BattleManager.BM)
            {
                BattleManager.BM.AddDialogueText(actionText);
                yield return new WaitForSeconds(1);
            }

            //RESULT
            if (roll.IsSuccess())
            {
                Debug.Log("Success");
                if (roll.IsCritical())
                {
                    Debug.Log("Critical");
                    yield return ExecutePAFs(action.critical, roll, actor, target, item);
                }
                yield return ExecutePAFs(action.success, roll, actor, target, item);
            }
            else
            {
                Debug.Log("Fail");
                if (roll.IsFumble())
                {
                    Debug.Log("Fumble");
                    yield return ExecutePAFs(action.fumble, roll, actor, target, item);
                }
                yield return ExecutePAFs(action.failure, roll, actor, target, item);
            }
        }

        yield return null;
    }

    Stat FromStatSelectorToStat(StatSelector selectedStat, CharacterSheet character)
    {
        switch (selectedStat)
        {
            case StatSelector.Agility: return Stat.Agility;
            case StatSelector.Presence: return Stat.Presence;
            case StatSelector.Strength: return Stat.Strength;
            case StatSelector.Toughness: return Stat.Toughness;
            case StatSelector.Defense: return Stat.Defense;
            case StatSelector.WeaponStat:
                if (character) return character.GetWeapon().abilityToUse;
                else return 0;
        }

        return 0;
    }

    static IEnumerator ExecutePAFs(List<ParameteredAtomicFunction> pafs, d20 roll, CharacterSheet actor, CharacterSheet target, Item item)
    {
        Debug.Log("ExecutePAFs " + pafs.Count);
        for (int i = 0; i < pafs.Count; i++)
        {
            ParameteredAtomicFunction paf = pafs[i];
            paf.roll = roll;
            paf.actor = actor;
            paf.target = target;
            paf.item = item;

            yield return ExecutePAF(paf);
        }
    }

    static IEnumerator ExecutePAF(ParameteredAtomicFunction paf)
    {
        Debug.Log("ExecutePAF " + paf.name);
        switch (paf.atomicFunction)
        {
            //self actions
            case EnumeratedAtomicFunction.HP: yield return HP(paf); break;
            case EnumeratedAtomicFunction.Infection: yield return Infection(paf); break;
            case EnumeratedAtomicFunction.MaxHP: yield return MaxHP(paf); break;
            case EnumeratedAtomicFunction.Strength: yield return Strength(paf); break;
            case EnumeratedAtomicFunction.Toughness: yield return Toughness(paf); break;
        }

        yield return null;
    }

    static IEnumerator HP(ParameteredAtomicFunction paf)
    {
        Debug.Log("HP");

        int increase = paf.actor.RecoverDamage(paf.damage);

        string resultText = paf.actor.GetCharacterName() + " recovers " + increase + "HP";

        if (BattleManager.BM)
        {
            BattleManager.BM.AddDialogueText(resultText);
            yield return new WaitForSeconds(paf.floatValue);
        }

        yield return null;
    }

    static IEnumerator MaxHP(ParameteredAtomicFunction paf)
    {
        Debug.Log("MaxHP");

        int increase = paf.actor.TempIncreaseMaxHP(paf.damage);

        string resultText = paf.actor.GetCharacterName() + " gains " + increase + " MaxHP";

        if (BattleManager.BM)
        {
            BattleManager.BM.AddDialogueText(resultText);
            yield return new WaitForSeconds(paf.floatValue);
        }

        yield return null;
    }

    static IEnumerator Strength(ParameteredAtomicFunction paf)
    {
        Debug.Log("Strength");

        int increase = paf.actor.TempIncreaseStrength(paf.damage);

        string resultText = paf.actor.GetCharacterName();
        resultText += (increase >= 0) ?
            " gains " + increase + " strength" :
            " loses " + (-increase) + " strength";

        if (BattleManager.BM && increase != 0)
        {
            BattleManager.BM.AddDialogueText(resultText);
            yield return new WaitForSeconds(paf.floatValue);
        }

        yield return null;
    }

    static IEnumerator Toughness(ParameteredAtomicFunction paf)
    {
        Debug.Log("Toughness");

        int increase = paf.actor.TempIncreaseToughness(paf.damage);

        string resultText = paf.actor.GetCharacterName();
        resultText += (increase >= 0) ?
            " gains " + increase + " toughness" :
            " loses " + (-increase) + " toughness";

        if (BattleManager.BM && increase != 0)
        {
            BattleManager.BM.AddDialogueText(resultText);
            yield return new WaitForSeconds(paf.floatValue);
        }

        yield return null;
    }

    static IEnumerator Infection(ParameteredAtomicFunction paf)
    {
        Debug.Log("Infection");

        paf.actor.SetInfected(paf.boolean);

        string resultText = 
            paf.actor.GetCharacterName() + ((paf.boolean)? " is now infected" : " is cured of infection");

        if (BattleManager.BM)
        {
            BattleManager.BM.AddDialogueText(resultText);
            yield return new WaitForSeconds(paf.floatValue);
        }

        yield return null;
    }

    //public IEnumerator EACoroutine(
    //    bool useRoll, d20 roll, int difficultyRating, Stat toTest, ParameteredAtomicFunction[] doTheseThings,
    //    CharacterSheet actor, CharacterSheet target, Item item)
    //{
    //    if (useRoll)
    //        roll = new d20(difficultyRating, actor, toTest);

    //    for (int i = 0; i < doTheseThings.Length; i++)
    //    {
    //        doTheseThings[i].roll = roll;
    //        doTheseThings[i].actor = actor;
    //        doTheseThings[i].target = target;
    //        doTheseThings[i].item = item;

    //        yield return ExecutePAF(doTheseThings[i]);
    //    }
    //}

    //public void ExecuteAction(
    //    bool useRoll, d20 roll, int difficultyRating, Stat toTest, ParameteredAtomicFunction[] doTheseThings,
    //    CharacterSheet actor, CharacterSheet target, Item item)
    //{
    //    StartCoroutine(EACoroutine(useRoll, roll, difficultyRating, toTest, doTheseThings, actor, target, item));
    //}



    ////Calculations
    //static int CalculateSameSideRangedAttackPenalty(CharacterSheet actor, CharacterSheet target)
    //{
    //    int penalty = 0;
    //    bool sameSide = false;
    //    bool usingRanged =
    //        actor.GetWeapon().GetType().Equals(typeof(ProjectileWeapon));
    //    if (BattleManager.BM)
    //        sameSide = BattleManager.BM.SameSide(actor, target);

    //    if (sameSide && usingRanged)
    //        penalty += sameSideRangedAttackPenalty;

    //    return penalty;
    //}

    ////Self Actions
    //public static IEnumerator CureBleeding(ParameteredAtomicFunction args)
    //{
    //    args.target.SetBleeding(false);
    //    yield return null;
    //}
    //public static IEnumerator CureBlinded(ParameteredAtomicFunction args)
    //{
    //    args.target.SetTempBlinded(false);
    //    yield return null;
    //}
    //public static IEnumerator CureDisabled(ParameteredAtomicFunction args)
    //{
    //    args.target.SetTempDisabled(false);
    //    yield return null;
    //}
    //public static IEnumerator CureDistracted(ParameteredAtomicFunction args)
    //{
    //    args.target.SetTempDistracted(false);
    //    yield return null;
    //}
    //public static IEnumerator CureHands(ParameteredAtomicFunction args)
    //{
    //    args.target.SetTempDisabledHands(false);
    //    yield return null;
    //}
    //public static IEnumerator CureInfection(ParameteredAtomicFunction args)
    //{
    //    args.target.SetInfected(false);
    //    yield return null;
    //}
    //public static IEnumerator CureLegs(ParameteredAtomicFunction args)
    //{
    //    args.target.SetTempDisabledLegs(false);
    //    yield return null;
    //}

    //public static IEnumerator ThrowBomb(ParameteredAtomicFunction args)
    //{
    //    if (!BattleManager.BM)
    //        yield break;

    //    d20 roll = new d20(bombDifficultyRating, args.actor, Stat.Presence);

    //    CharacterSheet[] opponents = BattleManager.BM.GetAllOpponents(args.actor);

    //    foreach(CharacterSheet opponent in opponents)
    //    {
    //        if(roll.CompareTo(opponent.GetAgility()))
    //            opponent.TakeDamage(args.damage);
    //    }
    //    yield return null;
    //}

    //public static IEnumerator Equip(ParameteredAtomicFunction args)
    //{
    //    args.target.EquipWeapon(args.item);
    //    yield return null;
    //}

    //public static IEnumerator IncreaseMaxHP(ParameteredAtomicFunction args)
    //{
    //    args.target.TempIncreaseMaxHP(args.damage);
    //    yield return null;
    //}
    //public static IEnumerator IncreaseStrength(ParameteredAtomicFunction args)
    //{
    //    args.target.TempIncreaseStrength(args.damage);
    //    yield return null;
    //}
    //public static IEnumerator IncreaseAgility(ParameteredAtomicFunction args)
    //{
    //    args.target.TempIncreaseAgility(args.damage);
    //    yield return null;
    //}
    //public static IEnumerator IncreasePresence(ParameteredAtomicFunction args)
    //{
    //    args.target.TempIncreasePresence(args.damage);
    //    yield return null;
    //}
    //public static IEnumerator IncreaseToughness(ParameteredAtomicFunction args)
    //{
    //    args.target.TempIncreaseToughness(args.damage);
    //    yield return null;
    //}

    //public static IEnumerator RecoverDamage(ParameteredAtomicFunction args)
    //{
    //    args.target.RecoverDamage(args.damage);
    //    yield return null;
    //}

    //public static IEnumerator Sneak(ParameteredAtomicFunction args)
    //{
    //    if (!BattleManager.BM)
    //        yield break;
    //    CharacterSheet opposingLeader = BattleManager.BM.GetOpposingLeader(args.target);

    //    d20 roll = new d20(sneakDifficultyRating, args.target, Stat.Agility, opposingLeader, Stat.Presence);

    //    if (roll.IsSuccess())
    //    {
    //        args.target.Sneak();
    //    }
    //    yield return null;
    //}



    ////Targeted Actions
    //public static IEnumerator Fight(ParameteredAtomicFunction args)
    //{
    //    if (!args.actor) yield break;
    //    if (!args.target) yield break;

    //    int difficultyRating = fightDifficultyRating;
    //    difficultyRating += CalculateSameSideRangedAttackPenalty(args.actor, args.target);

    //    Weapon weapon = args.actor.GetWeapon();
    //    d20 roll = new d20(difficultyRating, args.actor, weapon.abilityToUse, args.target, Stat.Defense);

    //    if (roll.IsSuccess())
    //    {
    //        args.target.TakeDamage(weapon.GetDamage(), roll.IsCritical());
    //    }
    //    else
    //    {
    //        if (roll.IsFumble())
    //        {
    //            CounterAttack(args);
    //        }
    //    }

    //    yield return null;
    //}
    //public static IEnumerator CounterAttack(ParameteredAtomicFunction args)
    //{
    //    if (!args.actor) yield break;
    //    if (!args.target) yield break;

    //    int difficultyRating = fightDifficultyRating;
    //    difficultyRating += CalculateSameSideRangedAttackPenalty(args.actor, args.target); 

    //    Weapon weapon = args.target.GetWeapon();
    //    d20 roll = new d20(difficultyRating, args.target, weapon.abilityToUse, args.actor, Stat.Defense);

    //    if (roll.IsSuccess())
    //    {
    //        args.target.TakeDamage(weapon.GetDamage(), roll.IsCritical());
    //    }

    //    yield return null;
    //}
    //public static IEnumerator Push(ParameteredAtomicFunction args)
    //{
    //    if (!args.actor) yield break;
    //    if (!args.target) yield break;

    //    Weapon weapon = args.actor.GetUnequippedWeapon();
    //    d20 roll = new d20(pushDifficultyRating, args.actor, Stat.Strength, args.target, Stat.Strength);

    //    if (roll.IsSuccess())
    //    {
    //        args.target.TakeDamage(weapon.GetDamage(), roll.IsCritical());
    //        args.target.Push();
    //    }
    //    else
    //    {
    //        if (roll.IsFumble())
    //        {
    //            CounterAttack(args);
    //        }
    //    }

    //    yield return null;
    //}



    ////Utility
    //public static IEnumerator Wait(float f)
    //{
    //    yield return new WaitForSeconds(f);
    //}

    //public static IEnumerator Display(string s, float f)
    //{
    //    if (BattleManager.BM)
    //        BattleManager.BM.AddDialogueText(s);
    //    return Wait(f);
    //}

    //public static IEnumerator SetDisplay(string s, float f)
    //{
    //    if (BattleManager.BM)
    //        BattleManager.BM.SetDialogueText(s);
    //    return Wait(f);
    //}
}

[System.Serializable]
public enum ActionTarget { Self, Other };

[System.Serializable]
public enum EnumeratedAtomicFunction 
{
    Nothing,
    HP,
    MaxHP,
    Strength,
    Agility,
    Presence,
    Toughness,
    Equip,
    Infection,
    Bleeding,
    Hands,
    Legs,
    Blind,
    Distracted,
    BludgeonResist,
    CutResist,
    ElectricResist,
    FireResist,
    MagicResist,
    PierceResist,

    TargetHP,
    TargetMaxHP,
    TargetStrength,
    TargetAgility,
    TargetPresence,
    TargetToughness,
    TargetEquip,
    TargetInfection,
    TargetBleeding,
    TargetHands,
    TargetLegs,
    TargetBlind,
    TargetDistracted,
    TargetBludgeonResist,
    TargetCutResist,
    TargetElectricResist,
    TargetFireResist,
    TargetMagicResist,
    TargetPierceResist
}

[System.Serializable]
public struct ParameteredAtomicFunction
{
    public string name;
    [HideInInspector] public d20 roll;
    [HideInInspector] public CharacterSheet actor;
    [HideInInspector] public CharacterSheet target;
    [HideInInspector] public Item item;
    public EnumeratedAtomicFunction atomicFunction;
    public Damage damage;
    public bool boolean;
    public float floatValue;
}