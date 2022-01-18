using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    static readonly int sameSideRangedAttackPenalty = 4;

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

        BattleManager.SetDialogueText(startText);
        if (BattleManager.BM)
            yield return new WaitForSeconds(1);
        
        if(action.alwaysDoThese.Count > 0)
            yield return ExecutePAFs(action.alwaysDoThese, new d20(), actor, target, item, action);

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

            BattleManager.AddDialogueText(actionText);
            if (BattleManager.BM)
                yield return new WaitForSeconds(1);

            //RESULT
            if (roll.IsSuccess())
            {
                Debug.Log("Success");
                if (roll.IsCritical())
                {
                    Debug.Log("Critical");
                    yield return ExecutePAFs(action.critical, roll, actor, target, item, action);
                }
                yield return ExecutePAFs(action.success, roll, actor, target, item, action);
            }
            else
            {
                Debug.Log("Fail");
                if (roll.IsFumble())
                {
                    Debug.Log("Fumble");
                    yield return ExecutePAFs(action.fumble, roll, actor, target, item, action);
                }
                yield return ExecutePAFs(action.failure, roll, actor, target, item, action);
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

    static IEnumerator ExecutePAFs(List<ParameteredAtomicFunction> pafs, d20 roll, CharacterSheet actor, CharacterSheet target, Item item, CharacterAction action)
    {
        Debug.Log("ExecutePAFs " + pafs.Count);
        for (int i = 0; i < pafs.Count; i++)
        {
            ParameteredAtomicFunction paf = pafs[i];
            paf.roll = roll;
            paf.actor = actor;
            paf.target = target;
            paf.item = item;
            paf.action = action;

            yield return ExecutePAF(paf);
        }
    }

    static IEnumerator ExecutePAF(ParameteredAtomicFunction args)
    {
        Debug.Log("ExecutePAF " + args.name);
        switch (args.atomicFunction)
        {
            //self actions
            case EnumeratedAtomicFunction.HP: yield return HP(args); break;
            case EnumeratedAtomicFunction.Infection: yield return Infection(args); break;
            case EnumeratedAtomicFunction.MaxHP: yield return MaxHP(args); break;
            case EnumeratedAtomicFunction.Strength: yield return Strength(args); break;
            case EnumeratedAtomicFunction.Toughness: yield return Toughness(args); break;

            case EnumeratedAtomicFunction.Sneak: yield return Sneak(args); break;
            case EnumeratedAtomicFunction.Fight: yield return Fight(args); break;
            case EnumeratedAtomicFunction.Push: yield return Push(args); break;
            case EnumeratedAtomicFunction.Return: yield return Return(args); break;
        }

        yield return null;
    }

    static IEnumerator HP(ParameteredAtomicFunction args)
    {
        Debug.Log("HP");

        int increase = args.actor.RecoverDamage(args.damage);

        string resultText = args.actor.GetCharacterName() + " recovers " + increase + "HP";

        if (BattleManager.BM)
        {
            BattleManager.AddDialogueText(resultText);
            yield return new WaitForSeconds(args.floatValue);
        }

        yield return null;
    }

    static IEnumerator MaxHP(ParameteredAtomicFunction args)
    {
        Debug.Log("MaxHP");

        int increase = args.actor.TempIncreaseMaxHP(args.damage);

        string resultText = args.actor.GetCharacterName() + " gains " + increase + " MaxHP";

        if (BattleManager.BM)
        {
            BattleManager.AddDialogueText(resultText);
            yield return new WaitForSeconds(args.floatValue);
        }

        yield return null;
    }

    static IEnumerator Strength(ParameteredAtomicFunction args)
    {
        Debug.Log("Strength");

        int increase = args.actor.TempIncreaseStrength(args.damage);

        string resultText = args.actor.GetCharacterName();
        resultText += (increase >= 0) ?
            " gains " + increase + " strength" :
            " loses " + (-increase) + " strength";

        if (BattleManager.BM && increase != 0)
        {
            BattleManager.AddDialogueText(resultText);
            yield return new WaitForSeconds(args.floatValue);
        }

        yield return null;
    }

    static IEnumerator Toughness(ParameteredAtomicFunction args)
    {
        Debug.Log("Toughness");

        int increase = args.actor.TempIncreaseToughness(args.damage);

        string resultText = args.actor.GetCharacterName();
        resultText += (increase >= 0) ?
            " gains " + increase + " toughness" :
            " loses " + (-increase) + " toughness";

        if (BattleManager.BM && increase != 0)
        {
            BattleManager.AddDialogueText(resultText);
            yield return new WaitForSeconds(args.floatValue);
        }

        yield return null;
    }

    static IEnumerator Infection(ParameteredAtomicFunction args)
    {
        Debug.Log("Infection");

        args.actor.SetInfected(args.boolean);

        string resultText = 
            args.actor.GetCharacterName() + ((args.boolean)? " is now infected" : " is cured of infection");

        if (BattleManager.BM)
        {
            BattleManager.AddDialogueText(resultText);
            yield return new WaitForSeconds(args.floatValue);
        }

        yield return null;
    }

    //calculations
    static int CalculateSameSideRangedAttackPenalty(CharacterSheet actor, CharacterSheet target)
    {
        int penalty = 0;
        bool sameSide = false;
        bool usingRanged =
            actor.GetWeapon().GetType().Equals(typeof(ProjectileWeapon));
        if (BattleManager.BM)
            sameSide = BattleManager.BM.SameSide(actor, target);

        if (sameSide && usingRanged)
            penalty += sameSideRangedAttackPenalty;

        return penalty;
    }

    //BIGGUNS
    public static IEnumerator Sneak(ParameteredAtomicFunction args)// SNEAK IS SPECIAL
    {
        if (!BattleManager.BM)
            yield break;
        CharacterSheet opposingLeader = BattleManager.BM.GetOpposingLeader(args.target);

        d20 roll = new d20(args.action.difficultyRating, args.target, Stat.Agility, opposingLeader, Stat.Presence);

        string resultText = "Sneak is";
        if (roll.Roll() <= 7)
            resultText += " looking questionable";
        else if (roll.Roll() <= 13)
            resultText += " going well";
        else
            resultText = args.actor.GetCharacterName() + " is a ghost";

        BattleManager.AddDialogueText(resultText);
        if (BattleManager.BM)
            yield return new WaitForSeconds(args.floatValue);

        if (roll.IsSuccess())
        {
            if (roll.Roll() <= 8)
                resultText = "But they still made it!";
            else
                resultText = "And they made it through";

            BattleManager.AddDialogueText(resultText);
            if (BattleManager.BM)
                yield return new WaitForSeconds(args.floatValue);

            args.actor.Sneak();
        }
        else
        {
            if (roll.Roll() >= 13)
                resultText = "But " + opposingLeader.GetCharacterName() + " stopped them";
            else
                resultText = "And they couldn't make it";
            BattleManager.AddDialogueText(resultText);
            if (BattleManager.BM)
                yield return new WaitForSeconds(args.floatValue);
        }
        yield return null;
    }

    static IEnumerator Fight(ParameteredAtomicFunction args)
    {
        Debug.Log("Fight");

        if (!args.actor) yield break;
        if (!args.target) yield break;

        int difficultyRating = args.action.difficultyRating;
        difficultyRating += CalculateSameSideRangedAttackPenalty(args.actor, args.target);

        Weapon weapon = args.actor.GetWeapon();
        d20 roll = new d20(difficultyRating, args.actor, weapon.abilityToUse, args.target, Stat.Defense);

        if (roll.IsSuccess())
        {
            args.target.TakeDamage(weapon.GetDamage(), roll.IsCritical());
        }
        else
        {
            if (roll.IsFumble())
            {
                CounterAttack(args);
            }
        }

        yield return null;
    }

    public static IEnumerator CounterAttack(ParameteredAtomicFunction args)
    {
        if (!args.actor) yield break;
        if (!args.target) yield break;

        int difficultyRating = args.action.difficultyRating;
        difficultyRating += CalculateSameSideRangedAttackPenalty(args.actor, args.target);

        Weapon weapon = args.target.GetWeapon();
        d20 roll = new d20(difficultyRating, args.target, weapon.abilityToUse, args.actor, Stat.Defense);

        if (roll.IsSuccess())
        {
            args.target.TakeDamage(weapon.GetDamage(), roll.IsCritical());
        }

        yield return null;
    }
    public static IEnumerator Push(ParameteredAtomicFunction args)
    {
        if (!args.actor) yield break;
        if (!args.target) yield break;

        Weapon weapon = args.actor.GetUnequippedWeapon();
        d20 roll = new d20(args.action.difficultyRating, args.actor, Stat.Strength, args.target, Stat.Strength);

        if (roll.IsSuccess())
        {
            args.target.TakeDamage(weapon.GetDamage(), roll.IsCritical());
            args.target.Push();
        }
        else
        {
            if (roll.IsFumble())
            {
                CounterAttack(args);
            }
        }

        yield return null;
    }

    public static IEnumerator Return(ParameteredAtomicFunction args)
    {
        args.actor.Return();

        yield return null;
    }
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
    TargetPierceResist,

    Sneak, //no target
    Return, //no target
    Fight,
    Push
}

[System.Serializable]
public struct ParameteredAtomicFunction
{
    public string name;
    [HideInInspector] public d20 roll;
    [HideInInspector] public CharacterSheet actor;
    [HideInInspector] public CharacterSheet target;
    [HideInInspector] public Item item;
    [HideInInspector] public CharacterAction action;
    public EnumeratedAtomicFunction atomicFunction;
    public Damage damage;
    public bool boolean;
    public float floatValue;
}