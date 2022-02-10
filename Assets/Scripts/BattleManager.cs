using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleManager : MonoBehaviour
{
    public static BattleManager BM;
    MenuManager MM;

    [SerializeField] Party playerParty;
    [SerializeField] Party enemyParty;
    public BattleHUD[] characterDisplays;
    public int initiativeRate = 3;
    public List<CharacterSheet> charactersYetToAct;
    private CharacterSheet target;

    [System.Serializable]
    public struct Party
    {
        public string name;
        public List<CharacterSheet> characters;
        public Transform[] positions;
        public bool playerControlled;
    }
    public TurnPhase currentPhase;

    public enum TurnPhase
    {
        SelectCharacter,
        SelectMove,
        SelectAction,
        Done
    }

    private void Awake()
    {
        if (!BM) { BM = this; }
        else { Destroy(gameObject); return; }

        MM = GetComponent<MenuManager>();

        //Set up Parties
        for (int i = 0; i < GameManager.GM.playerCharacters.Count; i++)
        {
            playerParty.characters.Add(GameManager.GM.playerCharacters[i]);
        }

        SetBattleOrder(playerParty);
        SetBattleOrder(enemyParty);

        //Set up HUDs
        for (int i = 0; i < 4; i++)
        {
            CharacterSheet current = null;
            if (i < playerParty.characters.Count)
                current = playerParty.characters[i];
            characterDisplays[i].UpdateText(current);
        }

        charactersYetToAct = new List<CharacterSheet>();

        MM.SetUpCharacterMenuLabels(playerParty.characters);

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
        do
        {
            Debug.Log("Start Round");
            GameManager.GM.SetText("Start Round");
            yield return new WaitForSeconds(1);

            int initiative = GameManager.RollDie(6);
            bool playersGoFirst = initiative > initiativeRate;

            if (playersGoFirst)
            {
                GameManager.GM.SetText("Player goes first");
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(PartyTurn(playerParty));
                yield return StartCoroutine(PartyTurn(enemyParty));
            }
            else
            {
                GameManager.GM.SetText("Enemies go first");
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(PartyTurn(enemyParty));
                yield return StartCoroutine(PartyTurn(playerParty));
            }
        } while (SideIsAlive(playerParty) && SideIsAlive(enemyParty));

        yield return CheckWinner();
    }

    IEnumerator CheckWinner()
    {

        if (SideIsAlive(playerParty))
        {
            GameManager.GM.SetText(playerParty.name + " is victorious");
        }
        else
        {
            GameManager.GM.SetText(enemyParty.name + " is victorious");
            GameManager.Reset();
        }

        yield return null;
    }

    IEnumerator PartyTurn(Party party)
    {
        if(!BothSidesAlive())
            yield break;

        GameManager.GM.SetText(party.name + "'s turn");
        yield return new WaitForSeconds(1);

        charactersYetToAct.Clear();

        foreach (CharacterSheet character in party.characters)
        {
            if (character.GetCanAct())
                charactersYetToAct.Add(character);
        }

        currentPhase = TurnPhase.SelectCharacter;
        CharacterSheet currentCharacter = null;

        while (charactersYetToAct.Count > 0 && BothSidesAlive())
        {
            //while character not selected
            //display select character
            if (currentPhase == TurnPhase.SelectCharacter)
            {
                if (party.playerControlled)
                {
                    GameManager.GM.SetText("Choose a character to act");
                    MM.OpenCharacterMenu(charactersYetToAct);

                    Menu charMenu = MM.characterMenu.GetComponent<Menu>();

                    while (!charMenu.IsSelected())
                    {
                        yield return null;
                    }

                    MM.CloseAllMenus();
                    currentCharacter = party.characters[charMenu.PullSelected()];
                }
                else
                {
                    currentCharacter = charactersYetToAct[Random.Range(0, charactersYetToAct.Count)];
                }

                currentPhase = TurnPhase.SelectMove;
            }

            //move not selected
            //display move menu
            //execute move

            if (currentPhase == TurnPhase.SelectMove)
            {
                int move = 0;

                if (party.playerControlled)
                {
                    GameManager.GM.SetText(currentCharacter.GetCharacterName() + "'s move");
                    MM.OpenMoveMenu();

                    Menu moveMenu = MM.moveMenu.GetComponent<Menu>();

                    while (!moveMenu.IsSelected())
                    {
                        yield return null;
                    }

                    MM.CloseAllMenus();
                    move = moveMenu.PullSelected();
                } else {
                    move = Random.Range(0, 4); //replace with EnemyClass.CalculateMove
                }

                switch (move)
                {
                    case 0: //stand ground
                        ActionManager.AM.LoadAction(currentCharacter, null, null, currentCharacter.standGround);
                        currentPhase = TurnPhase.SelectAction;
                        break;
                    case 1: //advance 
                        ActionManager.AM.LoadAction(currentCharacter, null, null, currentCharacter.defend);
                        currentPhase = TurnPhase.SelectAction;
                        break;
                    case 2: //retreat
                        ActionManager.AM.LoadAction(currentCharacter, null, null, currentCharacter.returnToBack);
                        currentPhase = TurnPhase.SelectAction;
                        break;
                    case 3: //sneak
                        ActionManager.AM.LoadAction(currentCharacter, null, null, currentCharacter.sneak);
                        currentPhase = TurnPhase.SelectAction;
                        break;
                    case 99:
                    default:
                        currentPhase = TurnPhase.SelectCharacter;
                        break;
                }
            }

            //while action not selected
            //display action menu
            //execute action
            if (currentPhase == TurnPhase.SelectAction)
            {
                while (!ActionManager.EmptyQueue())
                    yield return null;

                if (party.playerControlled)
                {
                    GameManager.GM.SetText(currentCharacter.GetCharacterName() + "'s action");
                    MM.OpenActionMenu();

                    Menu actionMenu = MM.actionMenu.GetComponent<Menu>();

                    while (!actionMenu.IsSelected())
                    {
                        yield return null;
                    }
                    MM.CloseAllMenus();
                    //action can be attack, using item, using environment, or "special"
                    switch (actionMenu.PullSelected())
                    {
                        case 0: //stand ground
                            List<CharacterSheet> targets = AttackTargets(currentCharacter);
                            target = currentCharacter;
                            yield return SelectTarget(targets);

                            if (target == currentCharacter)
                            {
                                currentPhase = TurnPhase.SelectAction;
                            }
                            else
                            {
                                ActionManager.AM.LoadAction(currentCharacter, target, null, currentCharacter.fight);
                                currentPhase = TurnPhase.Done;
                            }
                            break;
                        case 1:
                        case 2:
                        case 3:
                        default:
                            currentPhase = TurnPhase.Done;
                            break;
                    }
                }
                else
                {
                    //EnemyFunctions.GetAction
                    List<CharacterSheet> targets = AttackTargets(currentCharacter);
                    target = Fun.RandomFromArray(targets.ToArray());

                    ActionManager.AM.LoadAction(currentCharacter, target, null, currentCharacter.fight);
                    currentPhase = TurnPhase.Done;
                }
            }

            //remove current character from charactersYetToAct
            if (currentPhase == TurnPhase.Done)
            {
                while (!ActionManager.EmptyQueue())
                    yield return null;

                MM.CloseAllMenus();
                if (currentCharacter)
                    charactersYetToAct.Remove(currentCharacter);

                currentPhase = TurnPhase.SelectCharacter;
            }

            yield return null;
        }

        yield return null;
    }

    IEnumerator SelectTarget(List<CharacterSheet> targets)
    {
        Menu targetMenu = MM.targetMenu.GetComponent<Menu>();
        MM.OpenTargetMenu(targets);

        while (!targetMenu.IsSelected())
        {
            yield return null;
        }
        MM.CloseAllMenus();

        int selected = targetMenu.PullSelected();
        if (selected >= 0 && selected < 8)
        {
            target = targets[selected];
        }
    }

    //BATTLE ANIMATIONS
    void DOMoveAllPositions(Party party, float time)
    {
        foreach (CharacterSheet actor in party.characters)
            StartCoroutine(DOMoveToPositionCR(actor, time));
    }

    IEnumerator DOMoveToPositionCR(CharacterSheet actor, float time)
    {
        Party currentParty = GetParty(actor);

        int position = currentParty.characters.IndexOf(actor);

        //locations 0 to 3 are the normal side locations
        //locations 4 to 7 are the sneaking locations
        Vector3 destination = currentParty.positions[position + (actor.GetSneaking() ? 4 : 0)].position;

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

        foreach (CharacterSheet character in party.characters)
        {
            if (!character.GetSneaking())
                StartCoroutine(DOMoveToPositionCR(character, 0.25f));
        }

        SetBattleOrder(party);
    }
    public void SendToBackSneak(CharacterSheet actor)
    {
        //StartCoroutine(DOMoveToPositionCR(actor, 0.25f));

        Party party = GetParty(actor);
        party.characters.Remove(actor);
        party.characters.Add(actor);

        foreach (CharacterSheet character in party.characters)
        {
            if (character.GetSneaking())
                StartCoroutine(DOMoveToPositionCR(character, 0.25f));
        }

        SetBattleOrder(party);
    }
    public void SendToFront(CharacterSheet actor)
    {
        Party party = GetParty(actor);
        party.characters.Remove(actor);
        party.characters.Insert(0, actor);
        //DOMoveAllPositions(party, .25f);

        foreach (CharacterSheet character in party.characters)
        {
            if (!character.GetSneaking())
                StartCoroutine(DOMoveToPositionCR(character, 0.25f));
        }

        SetBattleOrder(party);
    }

    //CALCULATIONS
    List<CharacterSheet> AttackTargets(CharacterSheet actor)
    {
        Party enemies = GetOppositeParty(actor);

        List<CharacterSheet> targets = new List<CharacterSheet>();

        Weapon weapon = actor.GetWeapon();
        bool longRange = weapon.LongRanged();

        if (longRange)
        {
            foreach (CharacterSheet character in enemies.characters)
                if(character.GetCanBeHit())
                    targets.Add(character);
            return targets;
        }

        if (actor.GetSneaking())
        {
            foreach (CharacterSheet character in enemies.characters)
            {
                if (!character.GetSneaking() && character.GetCanBeHit())
                    targets.Add(character);
            }
            return targets;
        }

        foreach (CharacterSheet character in enemies.characters)
        {
            if (character == PartyFront(enemies) || character.GetSneaking())
                if(character.GetCanBeHit())
                    targets.Add(character);
        }
        return targets;
    }

    bool BothSidesAlive(){
        return SideIsAlive(playerParty) && SideIsAlive(enemyParty);
    }

    bool SideIsAlive(Party party)
    {
        foreach (CharacterSheet character in party.characters)
        {
            if (character.GetCanAct())
                return true;
        }
        return false;
    }

    Party GetParty(CharacterSheet actor)
    {
        return (IsInParty(actor, playerParty)) ? playerParty : enemyParty;
    }

    Party GetOppositeParty(CharacterSheet actor)
    {
        return (IsInParty(actor, playerParty)) ? enemyParty : playerParty;
    }

    static bool IsInParty(CharacterSheet actor, Party party)
    {
        foreach (CharacterSheet character in party.characters)
        {
            if (actor == character)
                return true;
        }
        return false;
    }
    static CharacterSheet PartyFront(Party party){
        for (int i = 0; i < party.characters.Count; i++)
        {
            if (!party.characters[i].GetSneaking())
                if(party.characters[i].GetCanBeHit())
                    return party.characters[i];
        }
        return party.characters[0];
    }

    static CharacterSheet PartyLeader(Party party)
    {
        for (int i = 0; i < party.characters.Count; i++)
        {
            if (!party.characters[i].GetSneaking())
                if(party.characters[i].GetCanAct())
                    return party.characters[i];
        }
        return party.characters[0];
    }

    public static bool SameParty(CharacterSheet actor, CharacterSheet target)
    {
        if (
            IsInParty(actor, BM.playerParty) &&
            IsInParty(target, BM.playerParty))
            return true;
        else if (
            IsInParty(actor, BM.enemyParty) &&
            IsInParty(target, BM.enemyParty)
            )
            return true;
        else
            return false;
    }
    public static CharacterSheet GetOppositeFront(CharacterSheet actor)
    {
        if (!BattleManager.BM)
            return null;

        if (IsInParty(actor, BattleManager.BM.playerParty))
            return BattleManager.PartyFront(BattleManager.BM.enemyParty);
        else
            return BattleManager.PartyFront(BattleManager.BM.playerParty);
    }

    public static CharacterSheet GetOppositeLeader(CharacterSheet actor)
    {
        if (!BattleManager.BM)
            return null;

        if (IsInParty(actor, BattleManager.BM.playerParty))
            return BattleManager.PartyLeader(BattleManager.BM.enemyParty);
        else
            return BattleManager.PartyLeader(BattleManager.BM.playerParty);
    }

    public static bool SameSide(CharacterSheet actor, CharacterSheet target)
    {
        //same party AND same sneaking
        //opposite party AND opposite sneaking

        if (!BM)
            return false;

        bool sameParty = SameParty(actor, target);
        bool sameSneaking = actor.GetSneaking() == target.GetSneaking();

        return sameParty == sameSneaking;
    }

    static public void SetBattleOrder(Party party)
    {
        foreach (CharacterSheet character in party.characters)
        {
            int location = party.characters.IndexOf(character);
            character.SetBattleOrder(location);
            character.UpdateBattleHUD();
        }
    }
}

