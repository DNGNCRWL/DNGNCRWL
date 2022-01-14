using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager
{
    public static void Fight(CharacterSheet actor, CharacterSheet target)
    {
        d20 roll = new d20(10, actor, actor.GetWeapon().abilityToUse, target, Stat.Defense);

        if (roll.IsSuccess())
        {
            int damage =
                (roll.IsCritical()) ?
                    actor.GetWeapon().CalculateCriticalDamage() :
                    actor.GetWeapon().CalculateDamage();
            target.TakeDamage(damage);
        }
        else
        {
            if (roll.IsFumble())
            {
                CounterAttack(target, actor);
            }
        }
    }
    
    public static void Push(CharacterSheet actor, CharacterSheet target)
    {
        d20 roll = new d20(10, actor, Stat.Strength, target, Stat.Strength);

        if (roll.IsSuccess())
        {
            target.Push();
            int damage =
                (roll.IsCritical()) ?
                    actor.GetDefaultWeapon().CalculateCriticalDamage() :
                    actor.GetDefaultWeapon().CalculateDamage();
            target.TakeDamage(damage);
        }
        else
        {
            if (roll.IsFumble())
            {
                CounterAttack(target, actor);
            }
        }
    }

    public static void CounterAttack(CharacterSheet actor, CharacterSheet target)
    {
        d20 roll = new d20(10, actor, actor.GetWeapon().abilityToUse, target, Stat.Defense);

        if (roll.IsSuccess())
        {
            int damage =
                (roll.IsCritical()) ?
                    actor.GetWeapon().CalculateCriticalDamage() :
                    actor.GetWeapon().CalculateDamage();
            target.TakeDamage(damage);
        }
    }

    public static void DrinkLifeElixir(CharacterSheet actor)
    {
        actor.RecoverDamage(GameManager.RollDie(6));
        actor.SetInfected(false);
        //consume item?
    }

    public static void SprayPerfume(int difficultyRating, CharacterSheet actor, CharacterSheet[] targets)
    {
        foreach(CharacterSheet target in targets)
        {
            d20 roll = new d20(difficultyRating, target, Stat.Toughness, null, 0);
            if (roll.IsFailure())
            {
                target.SetTempDistracted(true);
                target.TempIncreasePresence(-GameManager.RollDie(4));
            }
        }
        //consume item
    }
}
