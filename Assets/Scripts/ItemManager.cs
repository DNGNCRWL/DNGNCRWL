using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager IM;

    public Bag[] bags;
    public Item[] adventureTools;
    public Item[] specialItems;
    public Item[] startingWeapons;
    public Armor[] startingArmors;
    public Armor[] startingArmorsLowtier;

    public Weapon unequippedHumanWeapon;
    public Armor unequippedHumanArmor;

    //Random Generator Stuff
    static public Bag[] BAGS = { };
    static public Item[] ADVENTURE_TOOLS = { };
    static public Item[] SPECIAL_ITEMS = { };
    static public Item[] STARTING_WEAPONS = { };
    static public Armor[] STARTING_ARMORS = { };
    static public Armor[] STARTING_ARMORS_LOWTIER = { };

    static public Weapon UNEQUIPPED_HUMAN_WEAPON;
    static public Armor UNEQUIPPED_HUMAN_ARMOR;

    static public Dictionary<string, Item> STARTING_WEAPON_PAIRS;

    private void Awake()
    {
        if (IM == null) IM = this;
        else { Destroy(gameObject); return; }

        STARTING_WEAPON_PAIRS = new Dictionary<string, Item>();
        STARTING_WEAPON_PAIRS.Add("Bow", new Stackable("Bundle of Arrows", "Arrow", 10, false, 0));
        STARTING_WEAPON_PAIRS.Add("Crossbow", new Stackable("Stack of Bolts", "Bolt", 10, false, 0));

        BAGS = bags;
        ADVENTURE_TOOLS = adventureTools;
        SPECIAL_ITEMS = specialItems;

        STARTING_WEAPONS = startingWeapons;
        STARTING_ARMORS = startingArmors;
        STARTING_ARMORS_LOWTIER = startingArmorsLowtier;

        UNEQUIPPED_HUMAN_ARMOR = unequippedHumanArmor;
        UNEQUIPPED_HUMAN_WEAPON = unequippedHumanWeapon;
    }

    static public Item RANDOM_ITEM(Item[] items)
    {
        if (items.Length == 0) return null;

        int randomIndex = UnityEngine.Random.Range(0, items.Length);
        return items[randomIndex];
    }

    static public bool IsMagical(List<Item> items)
    {
        if (items.Count == 0) return false;

        for(int i = 0; i < items.Count; i++)
        {
            if (items[i] == null) return false;
            else if (items[i] is Scroll) return true;
        }

        return false;
    }

    static public bool IsMagical(Item item)
    {
        return item is Scroll;
    }

    public static int NumberOfItems(List<Item> items)
    {
        int count = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null) return count;
            count++;
        }
        return count;
    }

    public static string ItemListToString(List<Item> items)
    {
        string s = "";
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null) return s;
            if (i > 0) s += ", ";
            s += items[i].name;
        }
        return s;
    }

    public static string ItemListToExplicitString(List<Item> items)
    {
        string s = "";
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null) return s;
            if (i > 0) s += ", ";
            s += items[i].GetExplicitString();
        }
        return s;
    }
}
