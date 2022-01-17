using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scroll", menuName = "Item/Scroll", order = 1)]
public class Scroll : Item
{
    public static string[] UNCLEAN_SCROLLS = {
        "Unclean I: Palms Open the Southern Gate",
        "Unclean II: Tongue of Eris",
        "Unclean III: Te-le-kin-esis",
        "Unclean IV: Lucy-fires Levitation",
        "Unclean V: Daemon of Capillaries",
        "Unclean VI: Nine Violet Signs Unknot the Storm",
        "Unclean VII: Metzhuatl Blind Your Eye",
        "Unclean VIII: Foul Psychopomp",
        "Unclean IX: Eyelid Blinds the Mind",
        "Unclean X: Death"
    };

    public static string[] SACRED_SCROLLS =
    {
        "Sacred I: Grace of a Dead Saint",
        "Sacred II: Grace for a Sinner",
        "Sacred III: Whispers Pass the Gate",
        "Sacred IV: Aegis of Sorrow",
        "Sacred V: Unmet Fate",
        "Sacred VI: Bestial Speech",
        "Sacred VII: False Dawn/Night's Chariot",
        "Sacred VIII: Hermetic Step",
        "Sacred IX: Roskoe's Consuming Glare",
        "Sacred X: Enochian Syntax"
    };

    public bool known;
    public bool clean;
    public int index;
    public bool randomizeOnCopy;

    public Scroll Set(string description, bool known, bool clean, int index, bool randomizeOnCopy)
    {
        this.known = known;
        this.clean = clean;
        if (randomizeOnCopy)
            this.index = RandomIndex();
        else
            this.index = index;
        this.randomizeOnCopy = false;

        if (known)
        {
            if (clean) this.itemName = SACRED_SCROLLS[index];
            else this.itemName = UNCLEAN_SCROLLS[index];

            this.description = description;
        }
        else
        {
            this.itemName = "Unknown " + ((clean) ? "Sacred" : "Unclean") + " Power";
            this.description = "Unknown";
        }

        return this;
    }

    int RandomIndex()
    {
        if (clean) return UnityEngine.Random.Range(0, SACRED_SCROLLS.Length);
        else return UnityEngine.Random.Range(0, UNCLEAN_SCROLLS.Length);
    }

    public override string GetExplicitString()
    {
        return "Scroll of " + base.GetExplicitString();
    }

    public override Item Copy()
    {
        Scroll copy = ScriptableObject.CreateInstance<Scroll>();
        copy.CopyItemVariables(itemName, description, broken, value, actions);

        copy.known = known;
        copy.clean = clean;
        copy.index = index;

        copy.Set(description, known, clean, index, randomizeOnCopy);

        return copy;

        //return ScriptableObject.CreateInstance<Scroll>().Set(description, broken, value, known, clean, index, randomizeOnCopy);
    }
}