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

    public Scroll(bool known, bool clean, int index, bool randomizeOnCopy, bool broken, int value) :
        base("Unknown Scroll", broken, value)
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
            if (clean) this.name = SACRED_SCROLLS[index];
            else this.name = UNCLEAN_SCROLLS[index];
        }
        else
            this.name = "Unknown " + ((clean) ? "Sacred" : "Unclean") + " Power";

    }

    public Scroll(bool clean) : this(false, clean, 0, true, false, 0) { }

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
        return new Scroll(known, clean, index, randomizeOnCopy, broken, value);
    }
}