using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager
{
    //public static d20 TestGeneric(
    //    int difficulty, CharacterSheet actor, Stat actorAbility, CharacterSheet target, Stat targetAbility, 
    //    Func<CharacterSheet, CharacterSheet, d20> SuccessMethod,
    //    Func<CharacterSheet, CharacterSheet, d20> CriticalMethod,
    //    Func<CharacterSheet, CharacterSheet, d20> SuccessOnlyMethod,
    //    Func<CharacterSheet, CharacterSheet, d20> FailMethod,
    //    Func<CharacterSheet, CharacterSheet, d20> FumbleMethod,
    //    Func<CharacterSheet, CharacterSheet, d20> FailOnlyMethod)
    //{
    //    d20 roll = new d20(difficulty, actor, actorAbility, target, targetAbility);

    //    if (roll.Success())
    //    {
    //        SuccessMethod?.Invoke(actor, target);
    //        if (roll.Critical())
    //            CriticalMethod?.Invoke(actor, target);
    //        else
    //            SuccessOnlyMethod?.Invoke(actor, target);
    //    }
    //    else
    //    {
    //        FailMethod?.Invoke(actor, target);
    //        if (roll.Fumble())
    //            FumbleMethod?.Invoke(actor, target);
    //        else
    //            FailOnlyMethod?.Invoke(actor, target);
    //    }

    //    return roll;
    //}

    //public static void Fight(CharacterSheet actor, CharacterSheet target)
    //{
    //    d20 roll = new d20(10, actor, actor.GetWeapon().abilityToUse, target, Stat.Defense);

    //    if (roll.Success())
    //    {
    //        int damage =
    //            (roll.Critical()) ?
    //                actor.GetWeapon().CalculateCriticalDamage() :
    //                actor.GetWeapon().CalculateDamage();
    //        target.TakeDamage(damage);
    //    }
    //    else
    //    {
    //        if (roll.Fumble())
    //        {
    //            CounterAttack(target, actor);
    //        }
    //    }
    //}

    //public static d20 TestGeneric(CharacterSheet actor, CharacterSheet target, Action action)
    //{
    //    return TestGeneric(action.difficulty, actor, action.actorAbility, target, action.targetAbility,
    //        Action.FuncFinder(action.SuccessMethod), Action.FuncFinder(action.CriticalMethod), Action.FuncFinder(action.SuccessOnlyMethod),
    //        Action.FuncFinder(action.FailMethod), Action.FuncFinder(action.FumbleMethod), Action.FuncFinder(action.FailOnlyMethod)
    //        );
    //}

    //public static void FightGeneric(CharacterSheet actor, CharacterSheet target)
    //{
    //    TestGeneric(10, actor, actor.GetWeapon().abilityToUse, target, Stat.Defense,
    //        null,
    //        ApplyCriticalDamage,
    //        ApplyDamage,
    //        null,
    //        CounterAttack,
    //        null);
    //}

    ////Possible Methods

    //public static d20 ApplyCriticalDamage(CharacterSheet actor, CharacterSheet target)
    //{
    //    int damage = actor.GetWeapon().CalculateCriticalDamage();
    //    target.TakeDamage(damage);

    //    return null;
    //}

    //public static d20 ApplyDamage(CharacterSheet actor, CharacterSheet target)
    //{
    //    int damage = actor.GetWeapon().CalculateDamage();
    //    target.TakeDamage(damage);

    //    return null;
    //}

    //public static d20 CounterAttack(CharacterSheet previousAttacker, CharacterSheet counterAttacker)
    //{
    //    d20 roll = new d20(10, counterAttacker, counterAttacker.GetWeapon().abilityToUse, previousAttacker, Stat.Defense);

    //    if (roll.Success())
    //    {
    //        int damage =
    //            (roll.Critical()) ?
    //                counterAttacker.GetWeapon().CalculateCriticalDamage() :
    //                counterAttacker.GetWeapon().CalculateDamage();
    //        previousAttacker.TakeDamage(damage);
    //    }

    //    return roll;
    //}


}
