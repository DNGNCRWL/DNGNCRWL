using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheet : MonoBehaviour //can probably remove this as a monobehavior if we get rid of the unity calls at the bottom
{
    public bool randomizeClassless;
    public bool test;
    public CharacterAction testAction;
    public CharacterSheet testTarget;
    public bool test2;
    public CharacterAction testAction2;
    public CharacterSheet testTarget2;

    [Header("Character Stuff")]
    [SerializeField] string characterName; ////for some reason if i header and serialize field in front of a bunch of declarations, header is duplicated
    [SerializeField] string description, characterClass;
    [SerializeField] int
        hitPoints, maxHitPoints,
        strength, agility, presence, toughness,
        powers, omens, silver, food;
    static readonly int abilityMin = -3;
    static readonly int abilityMax = 6; //this is 3 in the book
    [SerializeField] bool waterskin; //make this an item?
    
    [Header("Resistances")]
    [SerializeField]
    Resistances naturalResistances;
    [SerializeField]
    Resistances equipmentResistances;

    [Header("Equipment")]
    [SerializeField] Weapon mainhand;
    [SerializeField] Weapon offhand, unequippedWeapon;
    [SerializeField] Bag bag;
    [SerializeField] Armor armor, unequippedArmor;
    [SerializeField] List<Item> inventory;

    [Header("Persistent Status")]
    [SerializeField] State currentState;
    public enum State{ Active, Inactive, Unconscious, Hemorrhaging, Dead };
    [SerializeField] bool
        infected, bleeding,
        brokenMainhand, brokenOffhand, brokenLeftLeg, brokenRightLeg, blindedLeft, blindedRight,
        canMove, canSee = false;
    [SerializeField] int reviveCounter = 0;
    [SerializeField] float hemorrhageTimer = 0;

    [Header("Battle Status")]
    [SerializeField]
    int maxHitPointsTempIncrease;
    [SerializeField] 
    int strengthTemp, agilityTemp, presenceTemp, toughnessTemp, defenseTemp;
    [SerializeField]
    bool sneaking, tempDisabledHands, tempDisabledLegs, tempBlinded, tempDistracted;
    [SerializeField]
    Resistances resistancesTemp;

    //Hooks, baby
    BattleHUD battleHUD;


    //getters setters
    public string GetCharacterName() { return characterName; }
    public string GetDescription() { return description; }
    public string GetCharacterClass() { return characterClass; }
    public int GetHitPoints() { return hitPoints; }
    public int GetMaxHitPoints() { return Mathf.Max(1, maxHitPoints + maxHitPointsTempIncrease); }
    public int GetPowers() { return powers; }
    public int GetOmens() { return omens; }
    public List<Item> GetInventory() { return inventory; }

    public bool GetSneaking(){return sneaking;}
    public bool GetCanAct(){
        switch(currentState){
            case State.Active: return true;
            case State.Hemorrhaging: return true;
        }
        return false;
    }

    //On with the show
    void InitializeCharacter()
    {
        randomizeClassless = false;

        characterName = "Unnamed";
        description = "Nothing is known. ";
        characterClass = "Classless";

        hitPoints = maxHitPoints = 1;
        strength = agility = presence = toughness = abilityMin;
        powers = omens = silver = food = 0;
        waterskin = false;

        mainhand = offhand = null;
        bag = null;
        armor = null;
        inventory = new List<Item>();
    }

    CharacterRollingPackage RandomClasslessRollPackage()
    {
        int firstAbility = GameManager.RollDie(4);
        int secondAbility = GameManager.RollDie(3);

        int strengthRoll = 3;
        int agilityRoll = 3;
        int presenceRoll = 3;
        int toughnessRoll = 3;

        switch (firstAbility)
        {
            case 1:
                strengthRoll++;
                switch (secondAbility)
                {
                    case 1: agilityRoll++; break;
                    case 2: presenceRoll++; break;
                    case 3: toughnessRoll++; break;
                }
                break;
            case 2:
                agilityRoll++;
                switch (secondAbility)
                {
                    case 1: strengthRoll++; break;
                    case 2: presenceRoll++; break;
                    case 3: toughnessRoll++; break;
                }
                break;
            case 3:
                presenceRoll++;
                switch (secondAbility)
                {
                    case 1: strengthRoll++; break;
                    case 2: agilityRoll++; break;
                    case 3: toughnessRoll++; break;
                }
                break;
            case 4:
                toughnessRoll++;
                switch (secondAbility)
                {
                    case 1: strengthRoll++; break;
                    case 2: agilityRoll++; break;
                    case 3: presenceRoll++; break;
                }
                break;
        }
        
        return new CharacterRollingPackage(strengthRoll, agilityRoll, presenceRoll, toughnessRoll, 8, 4, 2, 2, 6, 4);
    }

    void InitializeClassless(CharacterRollingPackage rp)
    {
        InitializeCharacter();

        powers = GameManager.RollDie(rp.powers);
        omens = GameManager.RollDie(rp.omens);
        silver = 10 * GameManager.RollDice(rp.silverDieCount, rp.silverDieSize);
        food = GameManager.RollDie(rp.food);
        waterskin = true;
        
        strength = RollAbilityScore(rp.strength, 0);
        agility = RollAbilityScore(rp.agility, 0);
        presence = RollAbilityScore(rp.presence, 0);
        toughness = RollAbilityScore(rp.toughness, 0);

        hitPoints = maxHitPoints = Mathf.Max(1, toughness + GameManager.RollDie(rp.hpDieSize));
        currentState = State.Active;

        EquipBag((Bag) ItemManager.RANDOM_ITEM(ItemManager.BAGS).Copy());
        PickupItem(ItemManager.RANDOM_ITEM(ItemManager.ADVENTURE_TOOLS).Copy());
        PickupItem(ItemManager.RANDOM_ITEM(ItemManager.SPECIAL_ITEMS).Copy());

        bool isMagical = ItemManager.IsMagical(inventory);

        EquipMainhand((Weapon) (ItemManager.RANDOM_ITEM(ItemManager.STARTING_WEAPONS).Copy()));
        if (ItemManager.STARTING_WEAPON_PAIRS.ContainsKey(mainhand.itemName))
            PickupItem(ItemManager.STARTING_WEAPON_PAIRS[mainhand.itemName].Copy());

        EquipOffhand(null);

        unequippedArmor = ItemManager.UNEQUIPPED_HUMAN_ARMOR;
        unequippedWeapon = ItemManager.UNEQUIPPED_HUMAN_WEAPON;

        if (isMagical)
            EquipArmor((Armor) (ItemManager.RANDOM_ITEM(ItemManager.STARTING_ARMORS_LOWTIER).Copy()));
        else
            EquipArmor((Armor) (ItemManager.RANDOM_ITEM(ItemManager.STARTING_ARMORS).Copy()));

        characterName = Fun.RandomFromArray(Fun.names);

        UpdateBattleHUD();

        Debug.Log(DebugString());
    }

    public void SetBattleHUD(BattleHUD battleHUD) { this.battleHUD = battleHUD; }

    void UpdateBattleHUD()
    {
        if (battleHUD)
            battleHUD.UpdateText();
    }
    
    int RollAbilityScore(int howMany, int shift)
    {
        int diceResult = GameManager.RollDice(howMany, 6) + shift;

        if (diceResult > 16) return 3;
        else if (diceResult > 14) return 2;
        else if (diceResult > 12) return 1;
        else if (diceResult > 8) return 0;
        else if (diceResult > 6) return -1;
        else if (diceResult > 4) return -2;
        else return -3;
    }

    string BagToString()
    {
        if (bag == null) return "No Bag";

        string s = "";
        s += bag.itemName + " with ";
        s += ItemManager.NumberOfItems(inventory) + "/" + bag.carryingCapacity + " capacity";
        return s;
    }
    string OffhandToString()
    {
        if (mainhand.twoHanded) return "None";
        else return ((offhand != null) ? offhand.GetExplicitString() : "d2 Fist");
    }
    string DebugString()
    {
        char lb = '\n';

        string state;
        switch (currentState)
        {
            case State.Active: state = "Active"; break;
            case State.Dead: state = "Dead"; break;
            case State.Hemorrhaging: state = "Hemorrhaging"; break;
            case State.Inactive: state = "Inactive"; break;
            case State.Unconscious: state = "Unconscious"; break;
            default: state = "Unknown"; break;
        }

        string injuries = " ";
        //    public bool brokenMainhand, brokenOffhand, brokenLeftLeg, brokenRightLeg, blindedLeft, blindedRight = false;
        if (!brokenMainhand && !brokenOffhand && !brokenLeftLeg && !brokenRightLeg && !blindedLeft && !blindedRight)
        {
            injuries = " No injuries. ";
        }
        else
        {
            if (brokenMainhand && brokenOffhand) injuries += "Hands are useless. ";
            else if (brokenMainhand) injuries += "Mainhand is broken. ";
            else if (brokenOffhand) injuries += "Offhand is broken. ";

            if (!canMove) injuries += "Legs are useless. ";
            else if (brokenLeftLeg || brokenRightLeg) injuries += "One leg is broken. ";

            if (!canSee) injuries += "Completely blind. ";
            else if (blindedLeft || blindedRight) injuries += "One eye is blinded. ";
        }

        string toReturn =
            "Name:        " + ((characterName != null) ? characterName : "No name") + lb +
            "State:       " + state + lb +
            "Description: " + ((description != null) ? description : "No known history.") + injuries + lb +
            "Class:       " + ((characterClass != null) ? characterClass : "No description") + lb +
            "HP:          " + hitPoints + "/" + GetMaxHitPoints() + lb +
            "Strength:    " + strength + lb +
            "Agility:     " + agility + lb +
            "Presence:    " + presence + lb +
            "Toughness:   " + toughness + lb +
            "Powers:      " + powers + lb +
            "Omens:       " + omens + lb +
            "Silver:      " + silver + lb +
            "Food:        " + food + lb +
            "Waterskin:   " + waterskin + lb +
            "Mainhand:    " + ((mainhand != null) ? mainhand.GetExplicitString() : "d2 Fist") + lb +
            "Offhand:     " + OffhandToString() + lb +
            "Armor:       " + ((armor != null) ? armor.GetExplicitString() : "Tier 0/0 Naked") + lb +
            "Bag:         " + BagToString() + lb;

        if (inventory.Count == 0) toReturn += "Equipment:   No Equipment";
        else
        {
            toReturn += "Inventory:   ";
            toReturn += ItemManager.ItemListToExplicitString(inventory);
        }

        return toReturn;

    }



    //EQUIP STUFF
    bool EquipBag(Bag newBag)
    {
        if (newBag == null)
            return GameManager.Error("Cannot equip nothing as a bag");

        Bag oldBag = bag;
        bag = newBag;
        if (oldBag == null) return true;
        return PickupItem(oldBag);
    }
    public bool EquipWeapon(Item tryEquip)
    {
        if (tryEquip == null)
            return EquipMainhand(null);

        if (!tryEquip.GetType().Equals(typeof(Weapon)))
            return GameManager.Error("Not a Weapon");

        Weapon newWeapon = (Weapon)tryEquip;

        foreach(Item i in inventory)
        {
            if (i.Equals(newWeapon))
                inventory.Remove(i);
        }

        return EquipMainhand(newWeapon);
    }
    bool EquipMainhand(Weapon newWeapon)
    {
        if (newWeapon!= null &&
            newWeapon.twoHanded)
        {
            return EquipTwohanded(newWeapon);
        }

        Weapon oldWeapon = mainhand;
        mainhand = newWeapon;
        if (oldWeapon == null) return false;
        return PickupItem(oldWeapon);
    }
    bool EquipOffhand(Weapon newWeapon)
    {
        if (newWeapon!= null &&
            newWeapon.twoHanded)
        {
            return EquipTwohanded(newWeapon);
        }

        Weapon oldWeapon = offhand;
        offhand = newWeapon;
        if (oldWeapon == null) return false;
        return PickupItem(oldWeapon);
    }
    bool EquipTwohanded(Weapon newWeapon)
    {
        EquipMainhand(null);
        EquipOffhand(null);
        mainhand = newWeapon;
        return true;//?? unsure if this is correct
    }
    bool EquipArmor(Armor newArmor)
    {
        Armor oldArmor = armor;
        armor = newArmor;
        if (oldArmor == null) return false;
        return PickupItem(oldArmor);
    }
    bool PickupItem(Item item)
    {
        if (item == null) return GameManager.Error("No item to pickup");
        if (bag == null) return GameManager.Error("Cannot pickup " + item.itemName + " without a bag.");
        if (inventory.Count >= bag.carryingCapacity) return GameManager.Error("Not enough carrying capacity");

        if (inventory.Contains(item))
            return GameManager.Error("Already carrying " + item.itemName + ".");

        //ITEMPACK??
        if (item is ItemPack)
        {
            //Debug.Log("Found a " + item.itemName);
            ItemPack ip = (ItemPack)item;
            bool pickedUpEverything = true;
            foreach (Item i in ip.items)
                pickedUpEverything = PickupItem(i) && pickedUpEverything;
            if (pickedUpEverything)
                //Debug.Log("Successfully picked up " + item.GetExplicitString());
            return pickedUpEverything;
        }

        //STACK???
        if(item is Stackable) //rework this!!
        {
            foreach(Item i in inventory)
            {
                if(item.itemName.CompareTo(i.itemName) == 0)
                {
                    Stackable inInventory = (Stackable)i;
                    Stackable other = (Stackable)item;

                    inInventory.amount += other.amount;
                    //Debug.Log("Successfully picked up " + item.GetExplicitString());
                    return true;
                }
            }
        }

        inventory.Add(item);
        return true;
    }



    //ABILITY & STAT GETTERS
    public int AbilityClamp(int input)
    {
        return Mathf.Clamp(input, abilityMin, abilityMax);
    }
    public int GetStrength()
    {
        int toReturn = strength;
        toReturn += strengthTemp;

        toReturn -= HemorrhagePenalty();

        return AbilityClamp(toReturn);
    }
    public int GetAgility()
    {
        int toReturn = agility;
        toReturn += agilityTemp;

        toReturn -= HemorrhagePenalty();
        toReturn = GetArmor().AgilityPenalty();

        return AbilityClamp(toReturn);
    }
    public int GetPresence()
    {
        int toReturn = presence;
        toReturn += presenceTemp;

        toReturn -= HemorrhagePenalty();

        return AbilityClamp(toReturn);
    }
    public int GetToughness()
    {
        int toReturn = toughness;
        toReturn += toughnessTemp;

        toReturn -= HemorrhagePenalty();

        return AbilityClamp(toReturn);
    }

    public int GetDefense()
    {
        int toReturn = agility;
        toReturn += agilityTemp;
        toReturn += defenseTemp;

        toReturn -= HemorrhagePenalty();
        toReturn -= GetArmor().DefensePenalty();

        return AbilityClamp(toReturn);
    }

    public int GetStat(Stat stat)
    {
        switch (stat)
        {
            case Stat.Agility: return GetAgility();
            case Stat.Presence: return GetPresence();
            case Stat.Strength: return GetStrength();
            case Stat.Toughness: return GetToughness();
            case Stat.Defense: return GetDefense();
            default: return 0;
        }
    }



    public Weapon GetWeapon()
    {
        if (mainhand != null) return mainhand;
        else return unequippedWeapon;
    }
    public Armor GetArmor()
    {
        if (armor != null) return armor;
        else return unequippedArmor;
    }
    public Weapon GetUnequippedWeapon()
    {
        return unequippedWeapon;
    }



    //ACTION FUNCTIONS
    //equip item??
    public DamageReturn CheckHPBounds()
    {
        if (hitPoints > GetMaxHitPoints())
            hitPoints = GetMaxHitPoints();

        if (hitPoints < 0)
        {
            currentState = State.Dead;
            return new DamageReturn(0, GetCharacterName() + " is dead", true);
        }

        if (hitPoints == 0)
            return DeathRoll();

        float fudgedPercent = ((float)hitPoints) / ((float)GetMaxHitPoints());
        fudgedPercent += Random.Range(-.25f, .25f);

        string r = GetCharacterName();

        if (fudgedPercent < 0.25f)
            r += " looks like death";
        else if (fudgedPercent < 0.5f)
            r += " is covered in bruises";
        else
            r += " is unscathed";

        return new DamageReturn(0, r, false);
    }
    public DamageReturn DeathRoll()
    {
        if (currentState == State.Dead) return new DamageReturn(0, GetCharacterName() + " is already dead", true); //beating a dead horse

        int deathRoll = GameManager.RollDie(4);

        string hurt = GetCharacterName();
        bool killerBlow = false;

        switch (deathRoll)
        {
            case 1: //knocked unconscious
                currentState = State.Unconscious;
                reviveCounter = GameManager.RollDie(4);
                hurt += " is knocked unconscious for " + reviveCounter * GameManager.secondsPerRound + " seconds";
                break;
            case 2:
                currentState = State.Inactive;
                reviveCounter = GameManager.RollDie(4);
                int brokenRoll = GameManager.RollDie(6);
                if(brokenRoll != 6)
                {
                    hurt += " breaks their ";
                    int brokenLimb = GameManager.RollDie(4);
                    switch (brokenLimb)
                    {
                        case 1: brokenMainhand = true; hurt += "fighting hand"; break;
                        case 2: brokenOffhand = true; hurt += "off hand"; break;
                        case 3: brokenLeftLeg = true; hurt += "left leg"; break;
                        case 4: brokenRightLeg = true; hurt += "right leg"; break;
                    }
                }
                else
                {
                    int brokenEye = GameManager.RollDie(2);
                    if (brokenEye == 1)
                    {
                        blindedLeft = true;
                        hurt += " loses their left eye";
                    }
                    else
                    {
                        blindedRight = true;
                        hurt += " loses their right eye";
                    };
                }
                break;
            case 3:
                currentState = State.Hemorrhaging;
                hemorrhageTimer = GameManager.RollDie(2);
                hurt = "There is blood everywhere";
                break;
            case 4:
                currentState = State.Dead;
                hurt += " breaths their last breath";
                killerBlow = true;
                break;
        }

        return new DamageReturn(0, hurt, killerBlow);
    }

    //??
    public void Sneak()
    {
        sneaking = true;
        if (BattleManager.BM)
            BattleManager.BM.SendToBackSneak(this);
    }
    public void Backlines()
    {
        sneaking = false;
        if (BattleManager.BM)
            BattleManager.BM.SendToBack(this);
    }
    public void Defend(){
        sneaking = false;
        if(BattleManager.BM)
            BattleManager.BM.SendToFront(this);
    }

    public int RecoverDamage(Damage damage)
    {
        int increase = GameManager.RollDice(damage.dieCount, damage.dieSize);
        increase += damage.modifier;
        hitPoints += increase;
        CheckHPBounds();

        return increase;
    }

    public void SetBleeding(bool b) { bleeding = b; }
    public void SetInfected(bool b) { infected = b; }
    public void SetTempDisabledHands(bool b) { tempDisabledHands = b; }
    public void SetTempDisabledLegs(bool b) { tempDisabledLegs = b; }
    public void SetTempDisabled(bool b) { tempDisabledHands = tempDisabledLegs = b; }
    public void SetTempBlinded(bool b) { tempBlinded = b; }
    public void SetTempDistracted(bool b) { tempDistracted = b; }

    public DamageReturn TakeDamage(Damage damage, bool critical)
    {
        int total = GameManager.RollDice(damage.dieCount, damage.dieSize);;
        total += damage.modifier;
        total *= critical ? 2 : 1;

        if (total < 1)
            total = 1;

        hitPoints -= total;

        DamageReturn r = CheckHPBounds();
        r.damageDone = total;

        return r;
    }

    public DamageReturn TakeDamage(Damage damage) { return TakeDamage(damage, false); }

    public int TempIncreaseMaxHP(Damage damage)
    {
        int increase = GameManager.RollDice(damage.dieCount, damage.dieSize);
        increase += damage.modifier;
        hitPoints += increase;
        maxHitPointsTempIncrease += increase;
        CheckHPBounds();

        return increase;
    }
    public int TempIncreaseStrength(Damage damage)
    {
        int increase = GameManager.RollDice(damage.dieCount, damage.dieSize);
        strengthTemp += increase;
        increase += damage.modifier;

        return increase;
    }
    public int TempIncreaseAgility(Damage damage)
    {
        int increase = GameManager.RollDice(damage.dieCount, damage.dieSize);
        agilityTemp += increase;
        increase += damage.modifier;

        return increase;
    }
    public int TempIncreasePresence(Damage damage)
    {
        int increase = GameManager.RollDice(damage.dieCount, damage.dieSize);
        presenceTemp += increase;
        increase += damage.modifier;

        return increase;
    }
    public int TempIncreaseToughness(Damage damage)
    {
        int increase = GameManager.RollDice(damage.dieCount, damage.dieSize);
        toughnessTemp += increase;
        increase += damage.modifier;

        return increase;
    }
    public int TempIncreaseDefense(Damage damage)
    {
        int increase = GameManager.RollDice(damage.dieCount, damage.dieSize);
        defenseTemp += increase;
        increase += damage.modifier;

        return increase;
    }
    public int TempIncreaseStat(Damage damage, Stat stat)
    {
        switch (stat)
        {
            case Stat.Agility: return TempIncreaseAgility(damage);
            case Stat.Presence: return TempIncreasePresence(damage);
            case Stat.Strength: return TempIncreaseStrength(damage); 
            case Stat.Toughness: return TempIncreaseToughness(damage);
            case Stat.Defense: return TempIncreaseDefense(damage);
        }

        return 0;
    }

    //PENALTIES
    public int HemorrhagePenalty()
    {
        int i = 0;
        if (currentState == State.Hemorrhaging)
            if (hemorrhageTimer > 1) i += 6;
            else i += 8;
        return i;
    }

    //**** UNITY CALLS ****
    private void Awake()
    {
        Update(); //lol
    }

    private void Update()
    {
        if (randomizeClassless) InitializeClassless(RandomClasslessRollPackage());

        if (test) Test();
        if (test2) Test2();
    }

    void Test()
    {
        test = false;
        //Item elixir = ItemManager.SPECIAL_ITEMS[6].Copy();
        //List<CharacterAction> actions = elixir.actions;

        ActionManager.AM.LoadAction(this, testTarget, null, testAction);
    }

    void Test2()
    {
        test2 = false;

        ActionManager.AM.LoadAction(this, testTarget2, null, testAction2);
    }
}