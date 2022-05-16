using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleManager : MonoBehaviour {
    public static Item BATTLE_LOOT;
    public static BattleManager BM;
    static EnemyEncounter ENEMY_ENCOUNTER;
    public EnemyEncounter defaultEnemyEncounter;
    public static void SetENEMY_ENCOUNTER(EnemyEncounter enemyEncounter) { ENEMY_ENCOUNTER = enemyEncounter; }
    MenuManager MM;

    [SerializeField] Party playerParty;
    [SerializeField] Party enemyParty;
    public BattleHUD[] characterDisplays;
    public int initiativeRate = 3;
    public List<CharacterSheet> charactersYetToAct;
    public GameObject selectedCharacter;
    public GameObject fade;

    //[System.Serializable]
    // public struct Party
    // {
    //     public string name;
    //     public List<CharacterSheet> characters;
    //     public Transform[] positions;
    //     public bool playerControlled;
    // }

    public TurnPhase currentPhase;

    public enum TurnPhase {
        SelectCharacter,
        SelectMove,
        SelectAction,
        Done
    }

    bool battleCompleted = false;

    private void Awake() {
        if (!BM) { BM = this; } else { Destroy(gameObject); return; }

        MM = GetComponent<MenuManager>();

        GameManager.PartySetActive(true);
        //Set up PlayerParty
        for (int i = 0; i < GameManager.GM.playerCharacters.Count; i++) {
            playerParty.characters.Add(GameManager.GM.playerCharacters[i]);
        }
        //Set up EnemyParty
        EnemyEncounter thisEncounter = defaultEnemyEncounter;
        if (ENEMY_ENCOUNTER != null)
            thisEncounter = ENEMY_ENCOUNTER;

        GameObject[] enemies = thisEncounter.GetEnemies();

        for (int i = 0; i < enemies.Length; i++) {
            GameObject go = Instantiate(enemies[i]);
            CharacterSheet goCS = go.GetComponent<CharacterSheet>();
            goCS.MakeItemCopies();
            enemyParty.characters.Add(goCS);
        }

        SetBattleOrder(playerParty);
        SetBattleOrder(enemyParty);

        //Set up HUDs
        for (int i = 0; i < 4; i++) {
            CharacterSheet current = null;
            if (i < playerParty.characters.Count)
                current = playerParty.characters[i];
            characterDisplays[i].UpdateText(current);
        }

        charactersYetToAct = new List<CharacterSheet>();

        DOMoveAllPositions(playerParty, 0);
        DOMoveAllPositions(enemyParty, 0);
    }

    IEnumerator Start() {
        GameManager.GM.SetText("Enemies appeared");
        if (ENEMY_ENCOUNTER != null)
            GameManager.GM.AddText(ENEMY_ENCOUNTER.GetEncounterName());
        MM.CloseAllMenus();

        //yield return new WaitForSeconds(3);
        //GameManager.GoToDungeonNavigation();

        yield return StartCoroutine(Combat());
    }

    //BATTLE STATES
    IEnumerator Combat() {
        do {
            Debug.Log("Start Round");
            GameManager.GM.SetText("Start Round");
            yield return new WaitForSeconds(1);

            int initiative = GameManager.RollDie(6);
            bool playersGoFirst = initiative > initiativeRate;

            if (playersGoFirst) {
                GameManager.GM.SetText("Player goes first");
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(PartyTurn(playerParty));
                yield return StartCoroutine(PartyTurn(enemyParty));
            } else {
                GameManager.GM.SetText("Enemies go first");
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(PartyTurn(enemyParty));
                yield return StartCoroutine(PartyTurn(playerParty));
            }
        } while (SideIsAlive(playerParty) && SideIsAlive(enemyParty));

        yield return CheckWinner();
    }

    IEnumerator CheckWinner() {

        if (SideIsAlive(playerParty)) {
            GameManager.GM.SetText(playerParty.name + " is victorious");
            yield return new WaitForSeconds(1);

            int silver = 0;
            float exp = 0;
            Item item = null;
            if (ENEMY_ENCOUNTER != null) {
                silver = ENEMY_ENCOUNTER.GetSilver();
                exp = ENEMY_ENCOUNTER.GetExperience();
                item = ENEMY_ENCOUNTER.GetLoot();
            }

            if (silver > 0) {
                GameManager.GM.AddText("They acquire " + silver + " silver coins");
                yield return new WaitForSeconds(1);
            }
            GameManager.GM.playerCharacters[0].AddSilver(silver);

            if (item != null) {
                GameManager.GM.AddText("And they found " + item.GetExplicitString());
                bool pickedup = false;
                // for (int i = 0; i < GameManager.GM.playerCharacters.Count; i++) {
                //     if (!pickedup)
                //         GameManager.GM.playerCharacters[i].PickupItem(item);
                // }
                BATTLE_LOOT = item;
                yield return new WaitForSeconds(1);
            }

            GameManager.GM.AddText("And they receive " + exp + " experience");
            yield return new WaitForSeconds(1);

            int survivorCount = 0;
            foreach (CharacterSheet charSheet in playerParty.characters)
                if (charSheet.GetCanBeHit())
                    survivorCount++;

            List<CharacterSheet> leveledUp = new List<CharacterSheet>();
            if (survivorCount > 0)
                foreach (CharacterSheet charSheet in playerParty.characters)
                    if (charSheet.GetCanBeHit())
                        if (charSheet.AddExperience(exp / survivorCount))
                            leveledUp.Add(charSheet);

            foreach (CharacterSheet charSheet in leveledUp) {
                GameManager.GM.AddText(charSheet.GetCharacterName() + " leveled up!");
                charSheet.UpdateBattleHUD();
                yield return new WaitForSeconds(1);
            }

            foreach (CharacterSheet charSheet in playerParty.characters)
                charSheet.ResetPostBattle();

            yield return new WaitForSeconds(1);
            battleCompleted = true;
        } else {
            GameManager.GM.SetText(enemyParty.name + " is victorious");
            GameManager.Reset();

            yield return new WaitForSeconds(3);
            GameManager.GameOver();
        }

        yield return null;
    }

    void SetSelectedCharacter(CharacterSheet character) {
        if (character != null)
            selectedCharacter = character.gameObject;
        else
            selectedCharacter = null;
    }

    IEnumerator PartyTurn(Party party) {
        if (!BothSidesAlive())
            yield break;

        GameManager.GM.SetText(party.name + "'s turn");
        yield return new WaitForSeconds(1);

        charactersYetToAct.Clear();

        foreach (CharacterSheet character in party.characters) {
            if (character.GetCanAct())
                charactersYetToAct.Add(character);
        }

        currentPhase = TurnPhase.SelectCharacter;
        CharacterSheet currentCharacter = null;

        int roundCount = -1;

        while (charactersYetToAct.Count > 0 && BothSidesAlive()) {
            ++roundCount;
            //while character not selected
            //display select character
            if (currentPhase == TurnPhase.SelectCharacter) {
                if (party.playerControlled) {
                    GameManager.GM.SetText("Choose a character to act");
                    MM.OpenCharacterMenu(charactersYetToAct, playerParty.characters);

                    Menu charMenu = MM.characterMenu.GetComponent<Menu>();

                    while (!charMenu.IsSelected()) {
                        yield return null;
                    }

                    MM.CloseAllMenus();
                    currentCharacter = party.characters[charMenu.PullSelected()];
                } else {
                    currentCharacter = charactersYetToAct[Random.Range(0, charactersYetToAct.Count)];
                }

                SetSelectedCharacter(currentCharacter);

                bool validCharacter = false;

                foreach (CharacterSheet cs in charactersYetToAct) {
                    if (cs == currentCharacter)
                        validCharacter = true;
                }

                if (validCharacter)
                    currentPhase = TurnPhase.SelectMove;
            }

            //move not selected
            //display move menu
            //execute move

            if (currentPhase == TurnPhase.SelectMove) {
                int move = 0;
                currentCharacter.gameObject.GetComponentInChildren<Animator>().SetTrigger("Prepare");

                if (party.playerControlled) {
                    GameManager.GM.SetText(currentCharacter.GetCharacterName() + "'s move");
                    MM.OpenMoveMenu();

                    Menu moveMenu = MM.moveMenu.GetComponent<Menu>();

                    while (!moveMenu.IsSelected()) {
                        yield return null;
                    }

                    MM.CloseAllMenus();
                    move = moveMenu.PullSelected();
                } else {
                    move = currentCharacter.GetAI().CalculateMove(currentCharacter);
                }

                switch (move) {
                    case 0: //stand ground                     
                        yield return DoAction(currentCharacter, currentCharacter.standGround, TurnPhase.SelectMove, TurnPhase.SelectAction);
                        break;
                    case 1: //advance 
                        yield return DoAction(currentCharacter, currentCharacter.defend, TurnPhase.SelectMove, TurnPhase.SelectAction);
                        break;
                    case 2: //retreat
                        yield return DoAction(currentCharacter, currentCharacter.returnToBack, TurnPhase.SelectMove, TurnPhase.SelectAction);
                        break;
                    case 3: //sneak
                        yield return DoAction(currentCharacter, currentCharacter.sneak, TurnPhase.SelectMove, TurnPhase.SelectAction);
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
            if (currentPhase == TurnPhase.SelectAction) {
                currentCharacter.gameObject.GetComponentInChildren<Animator>().SetTrigger("Prepare");

                while (!ActionManager.EmptyQueue())
                    yield return null;

                if (party.playerControlled) {
                    GameManager.GM.SetText(currentCharacter.GetCharacterName() + "'s action");
                    MM.OpenActionMenu();

                    Menu actionMenu = MM.actionMenu.GetComponent<Menu>();

                    while (!actionMenu.IsSelected()) {
                        yield return null;
                    }
                    MM.CloseAllMenus();
                    //action can be attack, using item, using environment, or "special"
                    switch (actionMenu.PullSelected()) {
                        case 0:
                            yield return DoAction(currentCharacter, currentCharacter.fight, TurnPhase.SelectAction, TurnPhase.Done);
                            break;
                        case 1:
                            List<Item> items = new List<Item>(currentCharacter.GetInventoryList());
                            yield return SelectItem(items, currentCharacter);
                            break;
                        case 2:
                        case 3:
                        default:
                            currentPhase = TurnPhase.Done;
                            break;
                    }
                } else {
                    CharacterAction calculatedAction = currentCharacter.GetAI().CalculateAction(this, currentCharacter);
                    CharacterSheet calculatedTarget = currentCharacter.GetAI().CalculateTarget(this, currentCharacter, calculatedAction);

                    ActionManager.AM.LoadAction(currentCharacter, calculatedTarget, null, calculatedAction);
                    currentPhase = TurnPhase.Done;
                }
            }

            //remove current character from charactersYetToAct
            if (currentPhase == TurnPhase.Done) {

                while (!ActionManager.EmptyQueue())
                    yield return null;

                MM.CloseAllMenus();
                if (currentCharacter)
                    charactersYetToAct.Remove(currentCharacter);

                currentPhase = TurnPhase.SelectCharacter;
            }
            SetSelectedCharacter(null);
            yield return null;
        }

        yield return null;
    }
    IEnumerator DoAction(CharacterSheet actor, CharacterAction action, TurnPhase returnPhase, TurnPhase nextPhase) {
        CharacterSheet target = null;

        if (action.targetingType != CharacterAction.TargetingType.None) {
            List<CharacterSheet> targets = PossibleTargets(actor, action);
            yield return SelectTarget(targets);

            currentPhase = returnPhase;
            if (targets.Count <= 0)
                yield break;
            else
                target = targets[0];
        }

        //Debug.Log(actor.GetCharacterName() + " is doing " + action.name + " to " + target.GetCharacterName());
        ActionManager.AM.LoadAction(actor, target, null, action);
        currentPhase = nextPhase;
    }

    IEnumerator SelectItem(List<Item> items, CharacterSheet character) {
        Menu itemMenu = MM.itemMenu.GetComponent<Menu>();

        MM.OpenItemMenu(items);

        while (!itemMenu.IsSelected()) {
            yield return null;
        }
        MM.CloseAllMenus();

        int selected = itemMenu.PullSelected();
        if (selected < 0 || selected >= items.Count) {
            yield break;
        }

        SelectItemAction(items[selected], character);
    }

    IEnumerator SelectItemAction(Item item, CharacterSheet character) {
        Menu subActionMenu = MM.actionMenu.GetComponent<Menu>();

        MM.OpenSubActionMenu(item.actions);

        while (!subActionMenu.IsSelected()) {
            yield return null;
        }
        MM.CloseAllMenus();

        int selected = subActionMenu.PullSelected();
        if (selected < 0 || selected >= item.actions.Count) {
            yield break;
        }

        DoAction(character, item.actions[selected], TurnPhase.SelectAction, TurnPhase.Done);
    }

    IEnumerator SelectTarget(List<CharacterSheet> targets) {
        Menu targetMenu = MM.targetMenu.GetComponent<Menu>();
        MM.OpenTargetMenu(targets);

        while (!targetMenu.IsSelected()) {
            yield return null;
        }
        MM.CloseAllMenus();

        int selected = targetMenu.PullSelected();
        if (selected < 0 || selected >= 8) {
            targets.Clear();
            yield break;
        }

        CharacterSheet target = targets[selected];
        targets.Clear();
        targets.Add(target);
    }

    //BATTLE ANIMATIONS
    void DOMoveAllPositions(Party party, float time) {
        foreach (CharacterSheet actor in party.characters)
            StartCoroutine(DOMoveToPositionCR(actor, time));
    }

    IEnumerator DOMoveToPositionCR(CharacterSheet actor, float time) {
        Party currentParty = GetParty(actor);

        int position = currentParty.characters.IndexOf(actor);

        //locations 0 to 3 are the normal side locations
        //locations 4 to 7 are the sneaking locations
        Vector3 destination = currentParty.positions[position + (actor.GetSneaking() ? 4 : 0)].position;

        actor.gameObject.transform.DOMove(destination, time);

        yield return null;
    }

    //BATTLE EVENTS
    public void SendToBack(CharacterSheet actor) {
        Party party = GetParty(actor);
        party.characters.Remove(actor);
        party.characters.Add(actor);
        //DOMoveAllPositions(party, .25f);

        foreach (CharacterSheet character in party.characters) {
            if (!character.GetSneaking())
                StartCoroutine(DOMoveToPositionCR(character, 0.25f));
        }

        SetBattleOrder(party);
    }
    public void SendToBackSneak(CharacterSheet actor) {
        //StartCoroutine(DOMoveToPositionCR(actor, 0.25f));

        Party party = GetParty(actor);
        party.characters.Remove(actor);
        party.characters.Add(actor);

        foreach (CharacterSheet character in party.characters) {
            if (character.GetSneaking())
                StartCoroutine(DOMoveToPositionCR(character, 0.25f));
        }

        SetBattleOrder(party);
    }
    public void SendToFront(CharacterSheet actor) {
        Party party = GetParty(actor);
        party.characters.Remove(actor);
        party.characters.Insert(0, actor);
        //DOMoveAllPositions(party, .25f);

        foreach (CharacterSheet character in party.characters) {
            if (!character.GetSneaking())
                StartCoroutine(DOMoveToPositionCR(character, 0.25f));
        }

        SetBattleOrder(party);
    }

    //CALCULATIONS
    List<CharacterSheet> PossibleTargets(CharacterSheet actor, CharacterAction action) {
        List<CharacterSheet> targets = new List<CharacterSheet>();

        switch (action.targetingType) {
            case CharacterAction.TargetingType.Allies:
                return GetParty(actor).characters;
            case CharacterAction.TargetingType.Any:
                targets.AddRange(BM.playerParty.characters);
                targets.AddRange(BM.enemyParty.characters);
                break;
            case CharacterAction.TargetingType.Enemies:
                return GetOppositeParty(actor).characters;
            case CharacterAction.TargetingType.NearbyAllies:
                foreach (CharacterSheet ally in GetParty(actor).characters) {
                    if (ally.GetSneaking() == actor.GetSneaking())
                        targets.Add(ally);
                }
                break;
            case CharacterAction.TargetingType.WeaponAttack:
                return WeaponAttackTargets(actor);
        }

        return targets;
    }

    public List<CharacterSheet> AllyTargets(CharacterSheet actor) {
        List<CharacterSheet> targets = new List<CharacterSheet>();
        foreach (CharacterSheet cs in GetParty(actor).characters)
            targets.Add(cs);
        return targets;
    }
    public List<CharacterSheet> AnyTargets(CharacterSheet actor) {
        List<CharacterSheet> targets = new List<CharacterSheet>();
        foreach (CharacterSheet cs in GetParty(actor).characters)
            targets.Add(cs);
        foreach (CharacterSheet cs in GetOppositeParty(actor).characters)
            targets.Add(cs);
        return targets;
    }
    public List<CharacterSheet> EnemyTargets(CharacterSheet actor) {
        List<CharacterSheet> targets = new List<CharacterSheet>();
        foreach (CharacterSheet cs in GetOppositeParty(actor).characters)
            targets.Add(cs);
        return targets;
    }
    public List<CharacterSheet> NearbyAllyTargets(CharacterSheet actor) {
        List<CharacterSheet> targets = new List<CharacterSheet>();
        foreach (CharacterSheet cs in GetParty(actor).characters) {
            if (AreInSameArea(actor, cs))
                targets.Add(cs);
        }
        return targets;
    }
    public List<CharacterSheet> AnyTargets() {
        List<CharacterSheet> targets = new List<CharacterSheet>();
        foreach (CharacterSheet cs in playerParty.characters)
            targets.Add(cs);
        foreach (CharacterSheet cs in enemyParty.characters)
            targets.Add(cs);
        return targets;
    }
    public List<CharacterSheet> WeaponAttackTargets(CharacterSheet actor) {
        Party enemies = GetOppositeParty(actor);

        List<CharacterSheet> targets = new List<CharacterSheet>();

        Weapon weapon = actor.GetWeapon();
        bool longRange = weapon.LongRanged();

        if (longRange) {
            foreach (CharacterSheet character in enemies.characters)
                if (character.GetCanBeHit())
                    targets.Add(character);
            return targets;
        }

        if (actor.GetSneaking()) {
            foreach (CharacterSheet character in enemies.characters) {
                if (!character.GetSneaking() && character.GetCanBeHit())
                    targets.Add(character);
            }
            return targets;
        }

        foreach (CharacterSheet character in enemies.characters) {
            if (character == PartyFront(enemies) || character.GetSneaking())
                if (character.GetCanBeHit())
                    targets.Add(character);
        }
        return targets;
    }

    bool BothSidesAlive() {
        return SideIsAlive(playerParty) && SideIsAlive(enemyParty);
    }

    bool SideIsAlive(Party party) {
        foreach (CharacterSheet character in party.characters) {
            if (character.GetCanAct())
                return true;
        }
        return false;
    }

    Party GetParty(CharacterSheet actor) {
        return (IsInParty(actor, playerParty)) ? playerParty : enemyParty;
    }

    Party GetOppositeParty(CharacterSheet actor) {
        return (IsInParty(actor, playerParty)) ? enemyParty : playerParty;
    }

    static bool IsInParty(CharacterSheet actor, Party party) {
        foreach (CharacterSheet character in party.characters) {
            if (actor == character)
                return true;
        }
        return false;
    }
    static CharacterSheet PartyFront(Party party)//first able to be hit
    {
        for (int i = 0; i < party.characters.Count; i++) {
            if (!party.characters[i].GetSneaking())
                if (party.characters[i].GetCanBeHit())
                    return party.characters[i];
        }
        return party.characters[0];
    }

    static CharacterSheet PartyLeader(Party party)//first able to act
    {
        for (int i = 0; i < party.characters.Count; i++) {
            if (!party.characters[i].GetSneaking())
                if (party.characters[i].GetCanAct())
                    return party.characters[i];
        }
        return party.characters[0];
    }

    public static bool AreSameParty(CharacterSheet actor, CharacterSheet target) {
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
    public static CharacterSheet GetOppositeFront(CharacterSheet actor) {
        if (!BattleManager.BM)
            return null;

        if (IsInParty(actor, BattleManager.BM.playerParty))
            return BattleManager.PartyFront(BattleManager.BM.enemyParty);
        else
            return BattleManager.PartyFront(BattleManager.BM.playerParty);
    }

    public static CharacterSheet GetOppositeLeader(CharacterSheet actor) {
        if (!BattleManager.BM)
            return null;

        if (IsInParty(actor, BattleManager.BM.playerParty))
            return BattleManager.PartyLeader(BattleManager.BM.enemyParty);
        else
            return BattleManager.PartyLeader(BattleManager.BM.playerParty);
    }

    public static bool AreInSameArea(CharacterSheet actor, CharacterSheet target) {
        //same party AND same sneaking
        //opposite party AND opposite sneaking

        if (!BM)
            return false;

        bool sameParty = AreSameParty(actor, target);
        bool sameSneaking = actor.GetSneaking() == target.GetSneaking();

        return sameParty == sameSneaking;
    }

    static public void SetBattleOrder(Party party) {
        foreach (CharacterSheet character in party.characters) {
            int location = party.characters.IndexOf(character);
            character.SetBattleOrder(location);
            character.UpdateBattleHUD();
        }
    }

    void Update() {
        if (battleCompleted && Input.GetMouseButtonDown(0))
            GameManager.GoToDungeonNavigation();

        if (selectedCharacter) {
            fade.transform.position = selectedCharacter.transform.position + Camera.main.transform.forward;
            fade.SetActive(true);
        } else
            fade.SetActive(false);

    }
}

