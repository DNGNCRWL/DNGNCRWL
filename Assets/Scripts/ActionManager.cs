using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager
{
    static readonly int sneakDifficultyRating = 10;

    static readonly int fightDifficultyRating = 10;
    static readonly int sameSideRangedAttackPenalty = 2;
    static readonly int pushDifficultyRating = 10;

    //Calculations
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

    //Self Actions
    public static void CureBleeding(ParameteredAtomicFunction args)
    {
        args.target.SetBleeding(false);
    }
    public static void CureBlinded(ParameteredAtomicFunction args)
    {
        args.target.SetTempBlinded(false);
    }
    public static void CureDisabled(ParameteredAtomicFunction args)
    {
        args.target.SetTempDisabled(false);
    }
    public static void CureDistracted(ParameteredAtomicFunction args)
    {
        args.target.SetTempDistracted(false);
    }
    public static void CureHands(ParameteredAtomicFunction args)
    {
        args.target.SetTempDisabledHands(false);
    }
    public static void CureInfection(ParameteredAtomicFunction args)
    {
        args.target.SetInfected(false);
    }
    public static void CureLegs(ParameteredAtomicFunction args)
    {
        args.target.SetTempDisabledLegs(false);
    }

    public static void DamageOtherSide(ParameteredAtomicFunction args)
    {
        if (!BattleManager.BM)
            return;

        CharacterSheet[] opponents = BattleManager.BM.GetAllOpponents(args.actor);

        foreach(CharacterSheet opponent in opponents)
        {
            opponent.TakeDamage(args.damage);
        }
    }

    public static void Equip(ParameteredAtomicFunction args)
    {
        args.target.EquipWeapon(args.item);
    }

    public static void IncreaseMaxHP(ParameteredAtomicFunction args)
    {
        args.target.TempIncreaseMaxHP(args.damage);
    }
    public static void IncreaseStrength(ParameteredAtomicFunction args)
    {
        args.target.TempIncreaseStrength(args.damage);
    }
    public static void IncreaseAgility(ParameteredAtomicFunction args)
    {
        args.target.TempIncreaseAgility(args.damage);
    }
    public static void IncreasePresence(ParameteredAtomicFunction args)
    {
        args.target.TempIncreasePresence(args.damage);
    }
    public static void IncreaseToughness(ParameteredAtomicFunction args)
    {
        args.target.TempIncreaseToughness(args.damage);
    }

    public static void RecoverDamage(ParameteredAtomicFunction args)
    {
        args.target.RecoverDamage(args.damage);
    }

    public static void Sneak(ParameteredAtomicFunction args)
    {
        if (!BattleManager.BM) return;
        CharacterSheet opposingLeader = BattleManager.BM.GetOpposingLeader(args.target);

        d20 roll = new d20(sneakDifficultyRating, args.target, Stat.Agility, opposingLeader, Stat.Presence);

        if (roll.IsSuccess())
        {
            args.target.Sneak();
        }
    }



    //Targeted Actions
    public static void Fight(ParameteredAtomicFunction args)
    {
        if (!args.actor) return;
        if (!args.target) return;

        int difficultyRating = fightDifficultyRating;
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
    }
    public static void CounterAttack(ParameteredAtomicFunction args)
    {
        if (!args.actor) return;
        if (!args.target) return;

        int difficultyRating = fightDifficultyRating;
        difficultyRating += CalculateSameSideRangedAttackPenalty(args.actor, args.target); 

        Weapon weapon = args.target.GetWeapon();
        d20 roll = new d20(difficultyRating, args.target, weapon.abilityToUse, args.actor, Stat.Defense);

        if (roll.IsSuccess())
        {
            args.target.TakeDamage(weapon.GetDamage(), roll.IsCritical());
        }
    }
    public static void Push(ParameteredAtomicFunction args)
    {
        if (!args.actor) return;
        if (!args.target) return;

        Weapon weapon = args.actor.GetUnequippedWeapon();
        d20 roll = new d20(pushDifficultyRating, args.actor, Stat.Strength, args.target, Stat.Strength);

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
    }
}