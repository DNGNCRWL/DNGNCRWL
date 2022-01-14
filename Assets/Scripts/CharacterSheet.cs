using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stat { Strength, Agility, Presence, Toughness, Defense };

public class CharacterSheet : MonoBehaviour //can probably remove this as a monobehavior if we get rid of the unity calls at the bottom
{
    public bool randomizeClassless;

    [SerializeField] string
        characterName, description, characterClass;
    [SerializeField] int
        hitPoints, maxHitPoints,
        strength, agility, presence, toughness,
        powers, omens, silver, food;
    static int abilityMin = -3;
    static int abilityMax = 6; //this is 3 in the book
    [SerializeField] bool waterskin; //make this an item?

    [SerializeField] Weapon
        mainhand, offhand, unequippedWeapon;
    [SerializeField] Bag bag;
    [SerializeField] Armor armor, unequippedArmor;
    [SerializeField] List<Item> inventory;

    public enum State{ Active, Inactive, Unconscious, Hemorrhaging, Dead };
    [SerializeField] State currentState;
    bool
        infected, bleeding,
        brokenMainhand, brokenOffhand, brokenLeftLeg, brokenRightLeg, blindedLeft, blindedRight,
        canMove, canSee = false;
    int reviveCounter = 0;
    float hemorrhageTimer = 0;

    //getters setters
    public string GetCharacterName() { return characterName; }
    public string GetDescription() { return description; }
    public string GetCharacterClass() { return characterClass; }
    public int GetHitPoints() { return hitPoints; }
    public int GetMaxHitPoints() { return maxHitPoints + maxHitPointsTempIncrease; }
    public int GetPowers() { return powers; }
    public int GetOmens() { return omens; }
    

    //Battle Effects
    public int
        maxHitPointsTempIncrease, strengthTemp, agilityTemp, presenceTemp, toughnessTemp, defenseTemp;
    public bool
        tempDisabledHands, tempDisabledLegs, tempBlinded, tempDistracted;

    //On with the show
    void InitializeCharacter()
    {
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

    void RandomClassless()
    {
        InitializeCharacter();

        powers = GameManager.RollDie(4);
        omens = GameManager.RollDie(2);
        silver = 10 * GameManager.RollDice(2, 6);
        food = GameManager.RollDie(4);
        waterskin = true;

        randomizeClassless = false;

        //This is where you're supposed to make a choice in abilities, but I'm going to randomize the choice
        int firstAbility = GameManager.RollDie(4);
        int secondAbility = GameManager.RollDie(3);

        int strengthRoll = 3;
        int agilityRoll = 3;
        int presenceRoll = 3;
        int toughnessRoll = 3;

        switch (firstAbility)
        {
            case 1: strengthRoll++;
                switch (secondAbility)
                {
                    case 1: agilityRoll++; break;
                    case 2: presenceRoll++; break;
                    case 3: toughnessRoll++; break;
                }
                break;
            case 2: agilityRoll++;
                switch (secondAbility)
                {
                    case 1: strengthRoll++; break;
                    case 2: presenceRoll++; break;
                    case 3: toughnessRoll++; break;
                }
                break;
            case 3: presenceRoll++;
                switch (secondAbility)
                {
                    case 1: strengthRoll++; break;
                    case 2: agilityRoll++; break;
                    case 3: toughnessRoll++; break;
                }
                break;
            case 4: toughnessRoll++;
                switch (secondAbility)
                {
                    case 1: strengthRoll++; break;
                    case 2: agilityRoll++; break;
                    case 3: presenceRoll++; break;
                }
                break;
        }

        strength = RollAbilityScore(strengthRoll, 0);
        agility = RollAbilityScore(agilityRoll, 0);
        presence = RollAbilityScore(presenceRoll, 0);
        toughness = RollAbilityScore(presenceRoll, 0);

        hitPoints = maxHitPoints = Mathf.Max(1, toughness + GameManager.RollDie(8));
        currentState = State.Active;

        EquipBag((Bag) ItemManager.RANDOM_ITEM(ItemManager.BAGS).Copy());
        PickupItem(ItemManager.RANDOM_ITEM(ItemManager.ADVENTURE_TOOLS).Copy());
        PickupItem(ItemManager.RANDOM_ITEM(ItemManager.SPECIAL_ITEMS).Copy());

        bool isMagical = ItemManager.IsMagical(inventory);

        EquipMainhand((Weapon) (ItemManager.RANDOM_ITEM(ItemManager.STARTING_WEAPONS).Copy()));
        if (ItemManager.STARTING_WEAPON_PAIRS.ContainsKey(mainhand.itemName))
            PickupItem(ItemManager.STARTING_WEAPON_PAIRS[mainhand.itemName]);

        EquipOffhand(null);

        unequippedArmor = ItemManager.UNEQUIPPED_HUMAN_ARMOR;
        unequippedWeapon = ItemManager.UNEQUIPPED_HUMAN_WEAPON;

        if (isMagical)
            EquipArmor((Armor) (ItemManager.RANDOM_ITEM(ItemManager.STARTING_ARMORS_LOWTIER).Copy()));
        else
            EquipArmor((Armor) (ItemManager.RANDOM_ITEM(ItemManager.STARTING_ARMORS).Copy()));

        characterName = RandomName();

        Debug.Log(DebugString());
    }

    string RandomName()
    {
        string[] names = {
            //"Aerg-Tval", "Agn", "Arvant", "Belsum", "Belum", "Brint", "Börda", "Daeru",
            //"Eldar", "Felban", "Gotven", "Graft", "Grin", "Grittr", "Haerü", "Hargha",
            //"Harmug", "Jotna", "Karg", "Karva", "Katla", "Keftar", "Klort", "Kratar",
            //"Kutz", "Kvetin", "Lygan", "Margar", "Merkari", "Nagl", "Niduk", "Nifehl",
            //"Prügl", "Qillnach", "Risten", "Svind", "Theras", "Therg", "Torvul", "Törn",
            //"Urm", "Urvarg", "Vagal", "Von", "Vrakh", "Vresi", "Wemut",
            //"Connor", "Isaiah", "James", "Raphael", "Tomasz"

            "Aerg-Tval", "Agn", "Arvant", "Belsum", "Belum", "Brint", "Borda", "Daeru",
            "Eldar", "Felban", "Gotven", "Graft", "Grin", "Grittr", "Haeru", "Hargha",
            "Harmug", "Jotna", "Karg", "Karva", "Katla", "Keftar", "Klort", "Kratar",
            "Kutz", "Kvetin", "Lygan", "Margar", "Merkari", "Nagl", "Niduk", "Nifehl",
            "Prugl", "Qillnach", "Risten", "Svind", "Theras", "Therg", "Torvul", "Torn",
            "Urm", "Urvarg", "Vagal", "Von", "Vrakh", "Vresi", "Wemut",

            "Connor", "Isaiah", "James", "Raphael", "Tomasz",

            "Gilt", "Hastings", "Subar", "Osgar", "Beauner", "Edill", "Karth", "Rosse", "Kathe",
            "Arash", "Bthil", "Coln", "Django", "Etrick", "Fong", "Gozzler", "Henk", "Iglis",
            "Jeronimo", "Kong", "Loop", "Menthis", "Nort", "Orcrun", "Penelope", "Quisling",
            "Rik", "Somner", "Tzands", "Uruth", "Vader", "Wander", "Xoco", "Ygg", "Zyphon",
            "Terra", "Lucky", "Edwin", "Sabre", "Acele", "Gao", "Istrago", "Ganun", "Cyan",
            "Magenta", "Yellow", "Kblack", "Lute", "Assel", "Robo", "Mono", "Ziegfried",
            "Demos", "Riku", "Bluto", "Rhelm", "Cecil", "Kayn", "Risse", "Jane", "Mal",
            "Ashe", "Ventok", "Figaro", "Ushtar", "Quinns",
            "Shadow", "Rock", "Heavy", "Jrue", "Yan", "Portus", "Kris",
            "Baltur", "Vera", "Scorpo", "Mazer", "Raydn",
            "Herschil", "Peow", "Pao", "Qobo", "Wenth", "Erskin", "Remmy",
            "Thistle", "Ygrinth", "Ursul", "Ivank", "Owenis", "Pisto"
        };

        int randomIndex = UnityEngine.Random.Range(0, names.Length);
        return names[randomIndex];
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
            "HP:          " + hitPoints + "/" + maxHitPoints + lb +
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
    public Weapon GetDefaultWeapon()
    {
        return unequippedWeapon;
    }



    //ACTION FUNCTIONS
    //equip item
    public void CheckHPBounds()
    {
        if (hitPoints > maxHitPoints)
            hitPoints = maxHitPoints;

        if (hitPoints < 0)
            currentState = State.Dead;

        if (hitPoints == 0)
            DeathRoll();
    }
    public void DeathRoll()
    {
        if (currentState == State.Dead) return;

        int deathRoll = GameManager.RollDie(4);

        switch (deathRoll)
        {
            case 1:
                currentState = State.Unconscious;
                reviveCounter = GameManager.RollDie(4);
                break;
            case 2:
                currentState = State.Inactive;
                reviveCounter = GameManager.RollDie(4);
                int brokenRoll = GameManager.RollDie(6);
                if(brokenRoll != 6)
                {
                    int brokenLimb = GameManager.RollDie(4);
                    switch (brokenLimb)
                    {
                        case 1: brokenMainhand = true; break;
                        case 2: brokenOffhand = true; break;
                        case 3: brokenLeftLeg = true; break;
                        case 4: brokenRightLeg = true; break;
                    }
                }
                else
                {
                    int brokenEye = GameManager.RollDie(4);
                    if (brokenEye == 1)
                        blindedLeft = true;
                    else
                        blindedRight = true;
                }
                break;
            case 3:
                currentState = State.Hemorrhaging;
                hemorrhageTimer = GameManager.RollDie(2);
                break;
            case 4:
                currentState = State.Dead;
                break;
        }
    }
    public void Push() //finish this. pass to battle manager?
    {
        if (BattleManager.BM) BattleManager.BM.Push(this);
    }
    public void RecoverDamage(int damage)
    {
        hitPoints += damage;

        CheckHPBounds();
    }
    public void SetBleeding(bool b) { bleeding = b; }
    public void SetInfected(bool b) { infected = b; }
    public void SetTempDisabledHands(bool b) { tempDisabledHands = b; }
    public void SetTempDisabledLegs(bool b) { tempDisabledLegs = b; }
    public void SetTempDisabled(bool b) { tempDisabledHands = tempDisabledLegs = b; }
    public void SetTempBlinded(bool b) { tempBlinded = b; }
    public void SetTempDistracted(bool b) { tempDistracted = b; }
    public void TakeDamage(int damage)
    {
        hitPoints -= damage;

        CheckHPBounds();
    }
    public void TempIncreaseMaxHP(int increase)
    {
        hitPoints += increase;
        maxHitPointsTempIncrease += increase;
    }
    public void TempIncreaseStrength(int increase)
    {
        strengthTemp += increase;
    }
    public void TempIncreaseAgility(int increase)
    {
        agilityTemp += increase;
    }
    public void TempIncreasePresence(int increase)
    {
        presenceTemp += increase;
    }
    public void TempIncreaseToughness(int increase)
    {
        toughnessTemp += increase;
    }
    public void TempIncreaseStat(int increase, Stat stat)
    {
        switch (stat)
        {
            case Stat.Agility: TempIncreaseAgility(increase); break;
            case Stat.Presence: TempIncreasePresence(increase); break;
            case Stat.Strength: TempIncreaseStrength(increase); break;
            case Stat.Toughness: TempIncreaseToughness(increase); break;
        }
    }


    //    tempDisabledHands, tempDisabledLegs, tempBlinded, tempDistracted;



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
        if (randomizeClassless) RandomClassless();
    }
}
