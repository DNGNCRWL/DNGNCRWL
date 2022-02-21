using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI characterName, stats, position;
    CharacterSheet targetCharacter;
    static char LB = '\n';

    public Color firstColor;
    public Color notFirstColor;
    public Color sneakingColor;

    public void UpdateText()
    {
        gameObject.SetActive(targetCharacter);
        if (!targetCharacter)
            return;

        characterName.text = targetCharacter.GetCharacterName();
        
        stats.text =
            ConvertCShitPointsToString(targetCharacter) + LB +
            ConvertPowersToString(targetCharacter) + LB +
            ConvertOmensToString(targetCharacter);

        if(targetCharacter.GetSneaking()){
            position.color = sneakingColor;
        }
        else {
            int battleOrder = targetCharacter.GetBattleOrder();
            if(battleOrder == 0){
                position.color = firstColor;
            }
            else{
                position.color = notFirstColor;
            }
        }

        position.text = targetCharacter.GetBattleOrderString();
    }

    public void UpdateText(CharacterSheet targetCharacter)
    {
        this.targetCharacter = targetCharacter;
        targetCharacter.SetBattleHUD(this);
        UpdateText();
    }

    string ConvertCShitPointsToString(CharacterSheet cs)
    {
        int hp = cs.GetHitPoints();
        int hpM = cs.GetMaxHitPoints();
        //we need 9 characters
        string r = "HP: "; //4 char
        r += (hp > 9 || hp < 0) ? "" : " ";
        r += hp;
        r += "/";
        r += (hpM > 9) ? "": " ";
        r += hpM;

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
