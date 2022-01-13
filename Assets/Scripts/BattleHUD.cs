using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleHUD : MonoBehaviour
{

    public TextMeshProUGUI characterName, stats;

    public void UpdateText(CharacterSheet cs)
    {
        //Debug.Log("Updating HUD with character: " + cs.characterName);
        if (!cs)
            Debug.Log("Null character sheet");

        characterName.text = cs.GetCharacterName();
        char lb = '\n';
        stats.text =
            ConvertCShitPointsToString(cs) + lb +
            ConvertPowersToString(cs) + lb +
            ConvertOmensToString(cs);
    }

    string ConvertCShitPointsToString(CharacterSheet cs)
    {
        //we need 9 characters
        string r = "HP: "; //4 characters
        int hp = cs.GetHitPoints();
        int hpM = cs.GetMaxHitPoints();
        r += (hp > 9) ? "" + hp : " " + hp;
        r += "/";
        r += (hpM > 9) ? "" + hpM : " " + hpM;

        return r;
    }

    string ConvertPowersToString(CharacterSheet cs)
    {
        int pow = cs.GetPowers();
        string r = "Powers:" + ((cs.GetPowers() > 9) ? "": " ") + pow;
        return r;
    }

    string ConvertOmensToString(CharacterSheet cs)
    {
        int pow = cs.GetOmens();
        string r = "Omens: " + ((cs.GetOmens() > 9) ? "" : " ") + pow;
        return r;
    }
}
