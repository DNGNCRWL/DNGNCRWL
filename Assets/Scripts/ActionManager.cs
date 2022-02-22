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

    public static bool EmptyQueue(){
        return AM.actionQueue.Count == 0 && !AM.doingAnAction;
    }

    public void LoadAction(CharacterSheet actor, CharacterSheet target, Item item, CharacterAction action)
    {
        actionQueue.Add(new ActionParameters(actor, target, item, action));
    }

    IEnumerator StartActionCR(CharacterSheet actor, CharacterSheet target, Item item, CharacterAction action)
    {
        doingAnAction = true;

        //initialize insertable message variables
        //input these into the messages using the {n} where n is the index of the array
        string[] messageVariables = {
            (actor)? actor.GetCharacterName() : "",
            (target)? target.GetCharacterName() : "",
            (item)? item.GetExplicitString() : "",
            (actor)? actor.GetWeapon().itemName : "",
            (actor)? actor.GetWeapon().GetExplicitString() : "",
            (actor)? GameManager.DamageTypeToString(actor.GetWeapon().damage.damageType) : "",
        };

        GameManager.GM.ClearText();

        //START
        if (action.TargetHead())
            target = BattleManager.GetOppositeLeader(actor);

        //START TEXT
        yield return GameManager.DisplayMessagePackage(action.startMessage, 0.5f, messageVariables);
        float positivity = action.startMessage.index / 20f;
        
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
        yield return GameManager.DisplayMessagePackage(action.actionMessage, 1 - positivity, messageVariables);
        positivity = action.actionMessage.index / 20f;

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



        //RESULT & RESULT TEXT
        if (success)
        {
            if (crit)
            {
                yield return GameManager.DisplayMessagePackage(action.criticalMessage, 1-positivity, messageVariables);
                yield return ExecutePAFs(action.critical, crit, fumble, effort, actor, target, item, action);
            }

            yield return GameManager.DisplayMessagePackage(action.successMessage, 1 - positivity, messageVariables);
            yield return ExecutePAFs(action.success, crit, fumble, effort, actor, target, item, action);
        }
        else
        {
            yield return GameManager.DisplayMessagePackage(action.failMessage, 1 - positivity, messageVariables);
            yield return ExecutePAFs(action.failure, crit, fumble, effort, actor, target, item, action);

            if (fumble)
            {
                yield return GameManager.DisplayMessagePackage(action.fumbleMessage, 1 - positivity, messageVariables);
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
            sameSide = BattleManager.AreInSameArea(actor, target);

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
            Ammo ammo = null;
            List<Item> inventory = actor.GetInventory();

            foreach (Item i in inventory)
            {
                if (i.GetType().Equals(typeof(Ammo)))
                {
                    Ammo asAmmo = (Ammo)i;
                    if (asAmmo.itemName == ammoName)
                        ammo = asAmmo;
                }
            }

            hasAmmo = ((ammo != null) && ammo.Consume());

            if(ammo != null)
            {
                Debug.Log(ammo.itemName);
                //Debug.Log(ammo.unit);
                Debug.Log("This much: " + ammo.amount);
            }
        }

        if (isProjectileWeapon && !hasAmmo)
        {
            GameManager.GM.AddText(actor.GetCharacterName() + " does not have any " + ammoName + "s");
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

        GameManager.GM.AddText(resultText);
        yield return new WaitForSeconds(args.floatValue);

        yield return null;
    }
    static IEnumerator MaxHP(ParameteredAtomicFunction args)
    {
        int increase = args.actor.TempIncreaseMaxHP(args.damage);

        string resultText = args.actor.GetCharacterName() + " gains " + increase + " MaxHP";

        GameManager.GM.AddText(resultText);
        yield return new WaitForSeconds(args.floatValue);

        yield return null;
    }
    static IEnumerator Strength(ParameteredAtomicFunction args)
    {
        int increase = args.actor.TempIncreaseStrength(args.damage);

        string resultText = args.actor.GetCharacterName();
        resultText += (increase >= 0) ?
            " gains " + increase + " strength" :
            " loses " + (-increase) + " strength";

        if (increase != 0)
        {
            GameManager.GM.AddText(resultText);
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

        if (increase != 0)
        {
            GameManager.GM.AddText(resultText);
            yield return new WaitForSeconds(args.floatValue);
        }

        yield return null;
    }
    static IEnumerator Infection(ParameteredAtomicFunction args)
    {
        args.actor.SetInfected(args.boolean);

        string resultText = 
            args.actor.GetCharacterName() + ((args.boolean)? " is now infected" : " is cured of infection");

        GameManager.GM.AddText(resultText);
        yield return new WaitForSeconds(args.floatValue);

        yield return null;
    }

    static IEnumerator DisplayString(ParameteredAtomicFunction args)
    {
        GameManager.GM.AddText(Fun.RandomFromArray(args.messages));
        yield return new WaitForSeconds(args.floatValue);
    }

    //targeted
    static IEnumerator TargetTakeWeaponDamage(ParameteredAtomicFunction args)
    {
        DamageReturn damageReturn = args.target.TakeDamage(args.actor.GetWeapon().damage, args.critical);

        GameManager.GM.AddText(args.target.GetCharacterName() + " takes " + damageReturn.damageDone + " damage");
        yield return new WaitForSeconds(args.floatValue);

        GameManager.GM.AddText(damageReturn.message);
        yield return new WaitForSeconds(args.floatValue);

        yield return null;
    }

    //BIGGUNS
    static IEnumerator Sneak(ParameteredAtomicFunction args)// SNEAK IS SPECIAL
    {
        if(args.actor.GetSneaking())
            GameManager.GM.AddText("They're already sneaking");
        else
            args.actor.Sneak();

        yield return null;

        // if (!BattleManager.BM)
        //     yield break;
        // CharacterSheet opposingLeader = BattleManager.GetOppositeLead(args.target);

        // d20 roll = new d20(args.action.difficultyRating, args.target, StatSelector.Agility, opposingLeader, StatSelector.Presence);

        // string resultText = "Sneak is";
        // if (roll.Roll() <= 8)
        //     resultText += " looking questionable";
        // else if (roll.Roll() <= 13)
        //     resultText += " going well";
        // else
        //     resultText = args.actor.GetCharacterName() + " disappears";

        // GameManager.GM.AddText(resultText);
        // yield return new WaitForSeconds(args.floatValue);

        // if (roll.IsSuccess())
        // {
        //     if (roll.Roll() <= 8)
        //         resultText = "But they still made it!";
        //     else
        //         resultText = "And they made it through";
        //     args.actor.Sneak();

        //     GameManager.GM.AddText(resultText);
        //     yield return new WaitForSeconds(args.floatValue);

        // }
        // else
        // {
        //     if (roll.Roll() > 8)
        //         resultText = "But " + opposingLeader.GetCharacterName() + " stopped them";
        //     else
        //         resultText = "And they couldn't make it";

        //     GameManager.GM.AddText(resultText);
        //     yield return new WaitForSeconds(args.floatValue);
        // }
        // yield return null;
    }

    static IEnumerator CounterAttack(ParameteredAtomicFunction args)
    {
        if (!args.actor) yield break;
        if (!args.target) yield break;

        GameManager.GM.AddText("COUNTER ATTACK!");
        yield return new WaitForSeconds(args.floatValue);

        int difficultyRating = args.action.difficultyRating;
        difficultyRating += CalculateSameSideRangedAttackPenalty(args.actor, args.target);

        Weapon weapon = args.target.GetWeapon();
        d20 roll = new d20(difficultyRating, args.target, weapon.abilityToUse, args.actor, Stat.Defense);

        GameManager.GM.AddText(args.target.GetCharacterName() + " pulls out " + weapon.GetExplicitString());
        yield return new WaitForSeconds(args.floatValue);

        if (roll.IsSuccess())
        {
            if (roll.IsCritical())
            {
                GameManager.GM.AddText("CRITICAL COUNTER HIT!");
                yield return new WaitForSeconds(args.floatValue);
            }

            DamageReturn damageReturn = args.target.TakeDamage(weapon.GetDamage(), roll.IsCritical());

            GameManager.GM.AddText(args.actor.GetCharacterName() + " takes " + damageReturn.damageDone + " damage");
            yield return new WaitForSeconds(args.floatValue);
            
            if(damageReturn.killerBlow || GameManager.RollDie(20) > 15)
            {
                GameManager.GM.AddText(damageReturn.message);
                yield return new WaitForSeconds(args.floatValue);
            }
        }
        else
        {
            GameManager.GM.AddText("A wasted opportunity!");
            yield return new WaitForSeconds(args.floatValue);
        }

        yield return null;
    }
    static IEnumerator Push(ParameteredAtomicFunction args)
    {
        args.target.Backlines();

        // if (!args.actor) yield break;
        // if (!args.target) yield break;

        // Weapon weapon = args.actor.GetUnequippedWeapon();
        // d20 roll = new d20(args.action.difficultyRating, args.actor, Stat.Strength, args.target, Stat.Strength);

        // if (roll.IsSuccess())
        // {
        //     args.target.TakeDamage(weapon.GetDamage(), roll.IsCritical());
        //     args.target.Push();
        // }
        // else
        // {
        //     if (roll.IsFumble())
        //     {
        //         CounterAttack(args);
        //     }
        // }

        yield return null;
    }
    static IEnumerator Return(ParameteredAtomicFunction args)
    {
        args.actor.Backlines();
        yield return null;
    }

    static IEnumerator Defend(ParameteredAtomicFunction args)
    {
        args.actor.Defend();
        yield return null;
    }

    static IEnumerator StandGround(ParameteredAtomicFunction args){
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
            case EnumeratedAtomicFunction.Return: yield return Return(args); break;
            case EnumeratedAtomicFunction.Defend: yield return Defend(args); break;
            case EnumeratedAtomicFunction.StandGround: yield return StandGround(args); break;

            case EnumeratedAtomicFunction.Fight: yield return TargetTakeWeaponDamage(args); break;
            case EnumeratedAtomicFunction.Push: yield return Push(args); break;
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
    Resist = 23,
    
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
    TargetResist = 123,

    TargetTakeWeaponDamage = 150,

    Sneak = 200, //no target
    Return = 201, //no target
    Defend = 202,
    StandGround=203,

    Fight = 300,
    Push = 301,
    CounterAttack = 302
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