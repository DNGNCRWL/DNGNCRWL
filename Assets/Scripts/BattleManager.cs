using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class BattleManager : MonoBehaviour
{
    public static BattleManager BM;

    [SerializeField] Party playerParty;
    [SerializeField] Party enemyParty;
    public BattleHUD[] characterDisplays;
    public int initiativeRate = 3;
    public List<CharacterSheet> charactersYetToAct;

    [System.Serializable]
    struct Party
    {
        public string name;
        public List<CharacterSheet> characters;
        public Transform[] positions;
    }

    private void Awake()
    {
        if (!BM) { BM = this; }
        else { Destroy(gameObject); return; }

        //Set up Parties
        for(int i = 0; i < GameManager.GM.playerCharacters.Count; i++){
            playerParty.characters.Add(GameManager.GM.playerCharacters[i]);
        }

        //Set up HUDs
        for(int i = 0; i < 4; i++)
        {
            CharacterSheet current = null;
            if (i < playerParty.characters.Count)
                current = playerParty.characters[i];
            characterDisplays[i].UpdateText(current);
        }

        charactersYetToAct = new List<CharacterSheet>();

        DOMoveAllPositions(playerParty, 0);
        DOMoveAllPositions(enemyParty, 0);
    }


    IEnumerator Start()
    {
        GameManager.GM.SetText("Enemies appeared");
        yield return new WaitForSeconds(1);

        yield return StartCoroutine(Combat());

        yield return null;  
    }

    //BATTLE STATES
    IEnumerator Combat()
    {
        do{
            Debug.Log("Start Round");
            GameManager.GM.SetText("Start Round");
            yield return new WaitForSeconds(1);

            int initiative = GameManager.RollDie(6);
            bool playersGoFirst = initiative > initiativeRate;

            if(playersGoFirst){
                GameManager.GM.SetText("Player goes first");
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(PartyTurn(playerParty));
                yield return StartCoroutine(PartyTurn(enemyParty));
            } else{
                GameManager.GM.SetText("Enemies go first");
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(PartyTurn(enemyParty));
                yield return StartCoroutine(PartyTurn(playerParty));
            }
        } while (SideIsAlive(playerParty) && SideIsAlive(enemyParty));
    }

    IEnumerator CheckWinner(){

        if(SideIsAlive(playerParty)){
            GameManager.GM.SetText(playerParty.name + " is victorious");
        } else {
            GameManager.GM.SetText(enemyParty.name + " is victorious");
            GameManager.Reset();
        }

        yield return null;
    }

    IEnumerator PartyTurn(Party party)
    {
        Debug.Log(party.name + "'s turn");

        GameManager.GM.SetText(party.name + "'s turn");
        yield return new WaitForSeconds(1);

        charactersYetToAct.Clear();

        foreach(CharacterSheet character in party.characters){
            if(character.GetCanAct())
                charactersYetToAct.Add(character);
        }

        while(charactersYetToAct.Count > 0){        
            yield return null;
        }

        yield return null;
    }

    //BATTLE ANIMATIONS
    void DOMoveAllPositions(Party party, float time){
        foreach(CharacterSheet actor in party.characters)
            StartCoroutine(DOMoveToPositionCR(actor, time));
    }

    IEnumerator DOMoveToPositionCR(CharacterSheet actor, float time)
    {
        Party currentParty = GetParty(actor);

        int position = currentParty.characters.IndexOf(actor);

        //locations 0 to 3 are the normal side locations
        //locations 4 to 7 are the sneaking locations
        Vector3 destination = currentParty.positions[position + (actor.GetSneaking() ? 4: 0)].position;

        actor.gameObject.transform.DOMove(destination, time);

        yield return null;
    }

    //BATTLE EVENTS
    public void SendToBack(CharacterSheet actor)
    {
        Party party = GetParty(actor);
        party.characters.Remove(actor);
        party.characters.Add(actor);
        //DOMoveAllPositions(party, .25f);

        foreach(CharacterSheet character in party.characters){
            if(!character.GetSneaking())
                StartCoroutine(DOMoveToPositionCR(character, 0.25f));
        }
    }
    public void SendToBackSneak(CharacterSheet actor)
    {
        //StartCoroutine(DOMoveToPositionCR(actor, 0.25f));

        Party party = GetParty(actor);
        party.characters.Remove(actor);
        party.characters.Add(actor);

        foreach(CharacterSheet character in party.characters){
            if(character.GetSneaking())
                StartCoroutine(DOMoveToPositionCR(character, 0.25f));
        }
    }
    public void SendToFront(CharacterSheet actor){
        Party party = GetParty(actor);
        party.characters.Remove(actor);
        party.characters.Insert(0, actor);
        //DOMoveAllPositions(party, .25f);

        foreach(CharacterSheet character in party.characters){
            if(!character.GetSneaking())
                StartCoroutine(DOMoveToPositionCR(character, 0.25f));
        }
    }

    //CALCULATIONS
    bool SideIsAlive(Party party)
    {
        foreach(CharacterSheet character in party.characters){
            if(character.GetCanAct())
                return true;
        }
        return false;
    }

    Party GetParty(CharacterSheet actor){
        return (IsInParty(actor, playerParty))? playerParty : enemyParty;
    }

    static bool IsInParty(CharacterSheet actor, Party party){
        foreach(CharacterSheet character in party.characters){
            if(actor == character)
                return true;
        }
        return false;
    }

    static CharacterSheet PartyLeader(Party party){
        for(int i = 0; i < party.characters.Count; i++){
            if(!party.characters[i].GetSneaking())
                return party.characters[i];
        }
        return party.characters[0];
    }

    public static bool SameParty(CharacterSheet actor, CharacterSheet target){
        if(
            IsInParty(actor, BM.playerParty) &&
            IsInParty(target, BM.playerParty))
            return true;
        else if(
            IsInParty(actor, BM.enemyParty) &&
            IsInParty(target, BM.enemyParty)
            )
            return true;
        else
            return false;
    }

    public static CharacterSheet GetOppositeLead(CharacterSheet actor){
        
        if(!BattleManager.BM)
            return null;

        if(IsInParty(actor, BattleManager.BM.playerParty))
            return BattleManager.PartyLeader(BattleManager.BM.enemyParty);
        else
            return BattleManager.PartyLeader(BattleManager.BM.playerParty);
    }

    public static bool SameSide(CharacterSheet actor, CharacterSheet target){
        //same party AND same sneaking
        //opposite party AND opposite sneaking

        if(!BM)
            return false;

        bool sameParty = SameParty(actor, target);
        bool sameSneaking = actor.GetSneaking() == target.GetSneaking();

        return sameParty == sameSneaking;
    }
}

