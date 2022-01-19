using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    static readonly int sameSideRangedAttackPenalty = 4;

    public static ActionManager AM;
    List<ActionParameters> actionQueue;
    bool doingAnAction = false;

    private void Awake()
    {
        if (!AM) AM = this;
        else { Destroy(gameObject); return; }

        actionQueue = new List<ActionParameters>();
    }

    void Update()
    {
        if(!doingAnAction && actionQueue.Count > 0)
        {
            ActionParameters ap = actionQueue[0];
            StartCoroutine(StartActionCR(ap.actor, ap.target, ap.item, ap.action));
            actionQueue.RemoveAt(0);
        }
    }

    public void LoadAction
        (CharacterSheet actor, CharacterSheet target, Item item, CharacterAction action)
    {
        actionQueue.Add(new ActionParameters(actor, target, item, action));
    }

    IEnumerator StartActionCR(CharacterSheet actor, CharacterSheet target, Item item, CharacterAction action)
    {
        Debug.Log("Doing: " + action.actionName);
        doingAnAction = true;

        //START
        if (action.TargetHead())
            target = BattleManager.GetOppositeLead(actor);

        //START TEXT
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
        
        //set up advantage and disadvantage booleans
        //advantage means "passes at least one test"
        bool advCrit = false;
        bool advSuccess = false;
        bool advFail = true;
        bool advFumble = true;
        //disadvantage means "must pass all tests"
        bool disCrit = true;
        bool disSuccess = true;
        bool disFail = false;
        bool disFumble = false;

        int effort = 11;

        //if there are tests, run tests
        for(int i = 0; i < action.tests.Length; i++)
        {
            d20 test = action.tests[i];
            test = new d20(action.difficultyRating, actor, test.actorStat, target, test.targetStat);

            advCrit = advCrit || test.IsCritical();
            advSuccess = advSuccess || test.IsSuccess();
            advFail = advFail && test.IsFailure();
            advFumble = advFumble && test.IsFumble();

            disCrit = disCrit && test.IsCritical();
            disSuccess = disSuccess && test.IsSuccess();
            disFail = disFail || test.IsFailure();
            disFumble = disFumble || test.IsFumble();

            if (i == 0) effort = test.Roll();
            else
            {
                if (action.testAtDisadvantage)
                    effort = Mathf.Min(effort, test.Roll());
                else
                    effort = Mathf.Max(effort, test.Roll());
            }
        }

        //EFFORT TEXT

        bool crit, success, fail, fumble;

        //default to "must pass all tests"
        if (!action.testAtDisadvantage)
        {
            crit = disCrit;
            success = disSuccess;
            fail = disFail;
            fumble = disFumble;
        }
        else
        {
            crit = advCrit;
            success = advSuccess;
            fail = advFail;
            fumble = advFumble;
        }

        Debug.Log(action.actionName + ": " + success + " " + crit + " " + fail + " " + fumble);

        //string actionText;
        //if (effort <= 8)
        //    actionText = "Something is off";
        //else if (effort <= 12)
        //    actionText = "Hard to tell";
        //else
        //    actionText = "It looks good";

        //BattleManager.AddDialogueText(actionText);
        //if (BattleManager.BM)
        //    yield return new WaitForSeconds(1);

        //RESULT & RESULT TEXT
        if (success)
        {
            if (crit)
            {
                yield return ExecutePAFs(action.critical, crit, fumble, effort, actor, target, item, action);
            }

            yield return ExecutePAFs(action.success, crit, fumble, effort, actor, target, item, action);
        }
        else
        {
            yield return ExecutePAFs(action.failure, crit, fumble, effort, actor, target, item, action);

            if (fumble)
            {
                yield return ExecutePAFs(action.fumble, crit, fumble, effort, actor, target, item, action);
            }
        }

        yield return null;

        doingAnAction = false;
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
    static IEnumerator CheckIfProjectileWeaponHasAmmo(CharacterSheet actor, float time)
    {
        Weapon weapon = actor.GetWeapon();
        bool isProjectileWeapon = false;
        bool hasAmmo = false;
        string ammoName = "";
        
        if (weapon.GetType().Equals(typeof(ProjectileWeapon)))
        {
            isProjectileWeapon = true;
            ProjectileWeapon asProjectileWeapon = (ProjectileWeapon)weapon;

            ammoName = asProjectileWeapon.ammoName;
            Stackable ammo = null;
            List<Item> inventory = actor.GetInventory();

            foreach (Item i in inventory)
            {
                if (i.GetType().Equals(typeof(Stackable)))
                {
                    Stackable asStackable = (Stackable)i;
                    if (asStackable.unit.CompareTo(ammoName) == 0)
                        ammo = asStackable;
                }
            }

            hasAmmo = ((ammo != null) && ammo.Consume());

            if(ammo != null)
            {
                Debug.Log(ammo.itemName);
                Debug.Log(ammo.unit);
                Debug.Log("This much: " + ammo.amount + " " + ammo.unit);
            }
        }

        if (isProjectileWeapon && !hasAmmo)
        {
            BattleManager.AddDialogueText(actor.GetCharacterName() + " does not have any " + ammoName + "s");
            if (BattleManager.BM)
                yield return new WaitForSeconds(time);

            actor.EquipWeapon(null);
        }
    }

    //non targeted
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

    static IEnumerator DisplayString(ParameteredAtomicFunction args)
    {
        if (BattleManager.BM)
        {
            BattleManager.AddDialogueText(Fun.RandomFromArray(args.messages));
            yield return new WaitForSeconds(args.floatValue);
        }
    }

    //targeted
    static IEnumerator TargetTakeWeaponDamage(ParameteredAtomicFunction args)
    {
        DamageReturn damageReturn = args.target.TakeDamage(args.actor.GetWeapon().damage, args.critical);

        if (BattleManager.BM)
        {
            if (args.critical)
            {
                BattleManager.AddDialogueText("CRITICAL HIT!");
                yield return new WaitForSeconds(args.floatValue);
            }

            BattleManager.AddDialogueText(args.target.GetCharacterName() + " takes " + damageReturn.damageDone + " damage");
            yield return new WaitForSeconds(args.floatValue);

            BattleManager.AddDialogueText(damageReturn.message);
            yield return new WaitForSeconds(args.floatValue);
        }

        yield return null;
    }

    //BIGGUNS
    static IEnumerator Sneak(ParameteredAtomicFunction args)// SNEAK IS SPECIAL
    {
        if (!BattleManager.BM)
            yield break;
        CharacterSheet opposingLeader = BattleManager.BM.GetOpposingLeader(args.target);

        d20 roll = new d20(args.action.difficultyRating, args.target, StatSelector.Agility, opposingLeader, StatSelector.Presence);

        string resultText = "Sneak is";
        if (roll.Roll() <= 8)
            resultText += " looking questionable";
        else if (roll.Roll() <= 13)
            resultText += " going well";
        else
            resultText = args.actor.GetCharacterName() + " disappears";

        BattleManager.AddDialogueText(resultText);
        if (BattleManager.BM)
            yield return new WaitForSeconds(args.floatValue);

        if (roll.IsSuccess())
        {
            if (roll.Roll() <= 8)
                resultText = "But they still made it!";
            else
                resultText = "And they made it through";
            args.actor.Sneak();

            BattleManager.AddDialogueText(resultText);
            if (BattleManager.BM)
                yield return new WaitForSeconds(args.floatValue);

        }
        else
        {
            if (roll.Roll() > 8)
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
        if (!args.actor) yield break;
        if (!args.target) yield break;

        Weapon weapon = args.actor.GetWeapon();
        yield return CheckIfProjectileWeaponHasAmmo(args.actor, args.floatValue);
        weapon = args.actor.GetWeapon();

        int difficultyRating = args.action.difficultyRating;
        difficultyRating += CalculateSameSideRangedAttackPenalty(args.actor, args.target);
        d20 roll = new d20(difficultyRating, args.actor, weapon.abilityToUse, args.target, Stat.Defense);

        if(roll.Roll() <= 8)
        {
            BattleManager.AddDialogueText("This is not looking good");
            if (BattleManager.BM)
                yield return new WaitForSeconds(args.floatValue);
        }
        else
        {
            BattleManager.AddDialogueText("They raise their " + weapon.GetExplicitString());
            if (BattleManager.BM)
                yield return new WaitForSeconds(args.floatValue);
        }
        
        if (roll.IsSuccess())
        {
            if (roll.IsCritical())
            {
                BattleManager.AddDialogueText("CRITICAL HIT!");
                if (BattleManager.BM)
                    yield return new WaitForSeconds(args.floatValue);
            }
            
            DamageReturn damageReturned = args.target.TakeDamage(weapon.GetDamage(), roll.IsCritical());
            
            BattleManager.AddDialogueText(args.target.GetCharacterName() + " takes " + damageReturned.damageDone + " damage");
            if (BattleManager.BM)
                yield return new WaitForSeconds(args.floatValue);

            if (damageReturned.killerBlow || GameManager.RollDie(20) > 15)
            {
                BattleManager.AddDialogueText(damageReturned.message);
                if (BattleManager.BM)
                    yield return new WaitForSeconds(args.floatValue);
            }
        }
        else
        {
            if (roll.IsFumble())
            {
                yield return CounterAttack(args);
            }
            else
            {
                BattleManager.AddDialogueText(args.actor.GetCharacterName() + " misses");
                if (BattleManager.BM)
                    yield return new WaitForSeconds(args.floatValue);
            }
        }

        yield return null;
    }
    static IEnumerator CounterAttack(ParameteredAtomicFunction args)
    {
        if (!args.actor) yield break;
        if (!args.target) yield break;

        BattleManager.AddDialogueText("COUNTER ATTACK!");
        if (BattleManager.BM)
            yield return new WaitForSeconds(args.floatValue);

        int difficultyRating = args.action.difficultyRating;
        difficultyRating += CalculateSameSideRangedAttackPenalty(args.actor, args.target);

        Weapon weapon = args.target.GetWeapon();
        d20 roll = new d20(difficultyRating, args.target, weapon.abilityToUse, args.actor, Stat.Defense);

        if (roll.IsSuccess())
        {
            if (roll.IsCritical())
            {
                BattleManager.AddDialogueText("CRITICAL COUNTER HIT!");
                if (BattleManager.BM)
                    yield return new WaitForSeconds(args.floatValue);
            }

            DamageReturn damageReturn = args.target.TakeDamage(weapon.GetDamage(), roll.IsCritical());

            BattleManager.AddDialogueText(args.actor.GetCharacterName() + " takes " + damageReturn.damageDone + " damage");
            if (BattleManager.BM)
                yield return new WaitForSeconds(args.floatValue);
            
            if(damageReturn.killerBlow || GameManager.RollDie(20) > 15)
            {
                BattleManager.AddDialogueText(damageReturn.message);
                if (BattleManager.BM)
                    yield return new WaitForSeconds(args.floatValue);
            }
        }
        else
        {
            BattleManager.AddDialogueText("A wasted opportunity!");
            if (BattleManager.BM)
                yield return new WaitForSeconds(args.floatValue);
        }

        yield return null;
    }
    static IEnumerator Push(ParameteredAtomicFunction args)
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
    static IEnumerator Return(ParameteredAtomicFunction args)
    {
        args.actor.Return();

        yield return null;
    }

    //EXECUTE PAFs
    static IEnumerator ExecutePAFs(
        List<ParameteredAtomicFunction> pafs, bool critical, bool fumble, int effort, CharacterSheet actor, CharacterSheet target, Item item, CharacterAction action)
    {
        for (int i = 0; i < pafs.Count; i++)
        {
            ParameteredAtomicFunction paf = pafs[i];
            paf.critical = critical;
            paf.fumble = fumble;
            paf.actor = actor;
            paf.target = target;
            paf.item = item;
            paf.action = action;

            yield return ExecutePAF(paf);
        }
    }

    static IEnumerator ExecutePAF(ParameteredAtomicFunction args)
    {
        switch (args.atomicFunction)
        {
            //self actions
            case EnumeratedAtomicFunction.HP: yield return HP(args); break;
            case EnumeratedAtomicFunction.Infection: yield return Infection(args); break;
            case EnumeratedAtomicFunction.MaxHP: yield return MaxHP(args); break;
            case EnumeratedAtomicFunction.Strength: yield return Strength(args); break;
            case EnumeratedAtomicFunction.Toughness: yield return Toughness(args); break;

            case EnumeratedAtomicFunction.DisplayString: yield return DisplayString(args); break;

            case EnumeratedAtomicFunction.TargetTakeWeaponDamage: yield return TargetTakeWeaponDamage(args); break;

            case EnumeratedAtomicFunction.Sneak: yield return Sneak(args); break;
            case EnumeratedAtomicFunction.Fight: yield return Fight(args); break;
            case EnumeratedAtomicFunction.Push: yield return Push(args); break;
            case EnumeratedAtomicFunction.Return: yield return Return(args); break;
            case EnumeratedAtomicFunction.CounterAttack: yield return CounterAttack(args); break;
        }

        yield return null;
    }
}

[System.Serializable]
public enum EnumeratedAtomicFunction 
{
    Nothing = 0,
    DisplayString = 1,

    HP = 10,
    MaxHP = 11,
    Strength = 12,
    Agility = 13,
    Presence = 14,
    Toughness = 15,
    Equip = 16,
    Infection = 17,
    Bleeding = 18,
    Hands = 19,
    Legs = 20,
    Blind = 21,
    Distracted = 22,
    BludgeonResist = 23,
    CutResist = 24,
    ElectricResist = 25,
    FireResist = 26,
    MagicResist = 27,
    PierceResist = 28,
    
    TargetHP = 110,
    TargetMaxHP = 111,
    TargetStrength = 112,
    TargetAgility = 113,
    TargetPresence = 114,
    TargetToughness = 115,
    TargetEquip = 116,
    TargetInfection = 117,
    TargetBleeding = 118,
    TargetHands = 119,
    TargetLegs = 120,
    TargetBlind = 121,
    TargetDistracted = 122,
    TargetBludgeonResist = 123,
    TargetCutResist = 124,
    TargetElectricResist = 125,
    TargetFireResist = 126,
    TargetMagicResist = 127,
    TargetPierceResist = 128,

    TargetTakeWeaponDamage = 150,

    Sneak = 200, //no target
    Return = 201, //no target
    Fight = 202,
    Push = 203,
    CounterAttack = 204
}

[System.Serializable]
public struct ParameteredAtomicFunction
{
    public string name;
    [HideInInspector] public bool critical;
    [HideInInspector] public bool fumble;
    [HideInInspector] public int effort;
    [HideInInspector] public CharacterSheet actor;
    [HideInInspector] public CharacterSheet target;
    [HideInInspector] public Item item;
    [HideInInspector] public CharacterAction action;
    public EnumeratedAtomicFunction atomicFunction;
    public Damage damage;
    public bool boolean;
    public float floatValue;

    public string[] messages;
}

public struct ActionParameters
{
    public ActionParameters(CharacterSheet actor, CharacterSheet target, Item item, CharacterAction action)
    {
        this.actor = actor;
        this.target = target;
        this.item = item;
        this.action = action;
    }
    public CharacterSheet actor;
    public CharacterSheet target;
    public Item item;
    public CharacterAction action;
}

[System.Serializable]
public enum StringFunctions
{
    Message = 0,
    ActorMessage,
    ActorMessageItem,
}