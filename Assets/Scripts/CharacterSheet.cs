using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheet : MonoBehaviour
{
    public bool randomizeClassless;

    public string
        characterName, description, characterClass;
    public int
        hitPoints, maxHitPoints,
        strength, agility, presence, toughness,
        powers, omens, silver, food;
    static int abilityMin = -3;
    static int abilityMax = 3;
    public bool waterskin;

    public Weapon
        mainhand, offhand, unequippedWeapon;
    public Bag bag;
    public Armor armor, unequippedArmor;
    public List<Item> inventory;

    public enum State { Active, Inactive, Unconscious, Hemorrhaging, Dead };
    public State currentState;
    public bool brokenMainhand, brokenOffhand, brokenLeftLeg, brokenRightLeg, blindedLeft, blindedRight = false;
    public bool canMove, canSee = false;
    public int reviveCounter = 0;
    public float hemorrhageTimer = 0;

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
            "Offhand:     " + GetOffHandExplicit() + lb +
            "Armor:       " + ((armor != null) ? armor.GetExplicitString() : "Tier 0/0 Naked") + lb +
            "Bag:         " + GetBagExplicitString() + lb;

        if (inventory.Count == 0) toReturn += "Equipment:   No Equipment";
        else
        {
            toReturn += "Inventory:   ";
            toReturn += ItemManager.ItemListToExplicitString(inventory);
        }

        return toReturn;

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
        if (ItemManager.STARTING_WEAPON_PAIRS.ContainsKey(mainhand.name))
            PickupItem(ItemManager.STARTING_WEAPON_PAIRS[mainhand.name]);

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

    bool EquipBag(Bag newBag)
    {
        if (newBag == null)
            return GameManager.Error("Cannot equip nothing as a bag");

        Bag oldBag = bag;
        bag = newBag;
        if (oldBag == null) return true;
        return PickupItem(oldBag);
    }

    string GetBagExplicitString()
    {
        if (bag == null) return "No Bag";

        string s = "";
        s += bag.name + " with ";
        s += ItemManager.NumberOfItems(inventory) + "/" + bag.carryingCapacity + " capacity";
        return s;
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

    string GetOffHandExplicit()
    {
        if (mainhand.twoHanded) return "None";
        else return ((offhand != null) ? offhand.GetExplicitString() : "d2 Fist");
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
        if (bag == null) return GameManager.Error("Cannot pickup " + item.name + " without a bag.");
        if (inventory.Count >= bag.carryingCapacity) return GameManager.Error("Not enough carrying capacity");

        if (inventory.Contains(item))
            return GameManager.Error("Already carrying " + item.name + ".");

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
                if(item.name.CompareTo(i.name) == 0)
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

    public int GetStrength()
    {
        int toReturn = strength;
        toReturn -= HemorrhagePenalty();

        return toReturn;
    }
    public int GetAgility()
    {
        int toReturn = agility;
        toReturn -= HemorrhagePenalty();

        int armorTier = 0;
        if(armor != null)
            armorTier = armor.armorTier;

        if (armorTier == 2)
            toReturn -= 2;
        else if (armorTier == 3)
            toReturn -= 4;

        return toReturn;
    }
    public int GetDefense()
    {
        int toReturn = agility;
        toReturn -= HemorrhagePenalty();

        int armorTier = 0;
        if (armor != null)
            armorTier = armor.armorTier;

        if (armorTier > 1)
            toReturn -= 2;

        return toReturn;
    }
    public int GetPresence()
    {
        int toReturn = presence;
        toReturn -= HemorrhagePenalty();

        return toReturn;
    }
    public int GetToughness()
    {
        int toReturn = toughness;
        toReturn -= HemorrhagePenalty();

        return toReturn;
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;

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
