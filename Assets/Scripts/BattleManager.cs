using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class BattleManager : MonoBehaviour
{
    public GameObject[] characters, enemies;
    public Transform[] playerSidePositions, enemySidePositions;
    public GameObject[] playerObjects, enemyObjects;
    public BattleHUD[] characterDisplays;

    struct PositionMapping
    {
        public Transform[] positions;
        public GameObject[] objects;
    }

    PositionMapping playerMap;
    PositionMapping enemyMap;

    public TextMeshProUGUI dialogueText;
    float snapTime = 0.05f; //3f
    float quickTime = 0.25f; //15f
    float mediumTime = 0.75f; //45f
    float longTime = 1.5f; //90f
    WaitForSeconds snapWait;
    WaitForSeconds quickWait;
    WaitForSeconds mediumWait;
    WaitForSeconds longWait;

    enum BattleState
    {
        Start,
        SelectCharacter,
        ChooseMove, SelectSwapPosition,
        ChooseAction, SelectTarget, ChooseItem, ChooseHazard,
        EnemyTurn,
        Won, Lost
    };
    BattleState currentState = BattleState.Start;

    private void Awake()
    {
        DOTween.Init(null, null, null);

        snapWait = new WaitForSeconds(snapTime);
        quickWait = new WaitForSeconds(quickTime);
        mediumWait = new WaitForSeconds(mediumTime);
        longWait = new WaitForSeconds(longTime);

        playerObjects = new GameObject[8];
        enemyObjects = new GameObject[8];

        playerMap.positions = playerSidePositions;
        playerMap.objects = playerObjects;
        enemyMap.positions = enemySidePositions;
        enemyMap.objects = enemyObjects;

        characters = GameManager.GM.characters;
        for(int i = 0; i < 4; i++)
        {
            if (characters[i])
            {
                playerObjects[i] = characters[i];
                characterDisplays[i].gameObject.SetActive(true);
                characterDisplays[i].UpdateText(characters[i].GetComponent<CharacterSheet>());
            }
            else
            {
                characterDisplays[i].gameObject.SetActive(false);
            }

            if(enemies[i])
                enemyObjects[i] = enemies[i];
        }
        
        for(int i = 0; i <8; i++)
        {
            StartCoroutine(DOMoveToLocation(playerMap, i, 0));
            StartCoroutine(DOMoveToLocation(enemyMap, i, 0));
        }
    }

    IEnumerator Start()
    {
        yield return longWait;

        //for(int i = 0; i < 4; i++)
        //{
        //    yield return StartCoroutine(SwapPositionsMOVE(characters[i], UnityEngine.Random.Range(0, 3)));
        //}
    }

    //**** UI STUFF ****//
    //maybe this should be put into Menu Manager or... maybe Menu Manager should be changed to Battle UI?
    void SetDialogueText(string s)
    {
        dialogueText.text = s; //make nicer
    }

    void AddDialogueText(string s)
    {
        dialogueText.text += "\n" + s;
        string[] lines = dialogueText.text.Split('\n');

        if(lines.Length > 3)
        {
            dialogueText.text = "";
            dialogueText.text += lines[lines.Length-3] + "\n" + lines[lines.Length - 2] + "\n" + lines[lines.Length - 1];
        }
    }



    //**** THIS IS THE MOVING STUFF ****//
    void SwapAndJump(PositionMapping playerMap, int i, int j, float time, float jumpHeight)
    {
        SwapLocations(playerMap.objects, i, j);
        StartCoroutine(DOJumpLocation(playerMap, i, time, jumpHeight));
        StartCoroutine(DOJumpLocation(playerMap, j, time, jumpHeight));
    }
    void SwapAndMove(PositionMapping playerMap, int i, int j, float time)
    {
        SwapLocations(playerMap.objects, i, j);
        StartCoroutine(DOMoveToLocation(playerMap, i, time));
        StartCoroutine(DOMoveToLocation(playerMap, j, time));
    }
    void SwapLocations(GameObject[] objects, int i, int j)
    {
        GameObject a = objects[i];
        GameObject b = objects[j];
        objects[i] = b;
        objects[j] = a;
    }
    int FindRandomEmptyOpposingLocation(GameObject[] objects)
    {
        List<int> indices = new List<int>();

        for(int i = 4; i < 8; i++)
        {
            if (!objects[i]) indices.Add(i);
        }

        if (indices.Count > 0) return indices[UnityEngine.Random.Range(0, indices.Count)];

        return -1; //error
    }
    int FindRandomHomeLocation(GameObject[] objects)
    {
        List<int> indices = new List<int>();

        for (int i = 0; i < 4; i++)
        {
            if (!objects[i]) indices.Add(i);
        }

        if (indices.Count > 0) return indices[UnityEngine.Random.Range(0, indices.Count)];

        return -1; //error
    }
    int Location(GameObject g)
    {
        PositionMapping pm = PositionMappingFromGameObject(g);
        for (int i = 0; i < 16; i++)
        {
            if (g.Equals(pm.objects[i]))
                return i;
        }
        return -1; //error
    }
    PositionMapping PositionMappingFromGameObject(GameObject g)
    {
        for(int i = 0; i < 4; i++)
        {
            if (characters[i] && characters[i] == g)
                return playerMap;
        }

        return enemyMap;
    }
    CharacterSheet FindOppositeLead(GameObject g)
    {
        if (PositionMappingFromGameObject(g).objects == playerMap.objects)
            return FindLead(enemyObjects);
        else
            return FindLead(playerObjects);
    }
    CharacterSheet FindLead(GameObject[] objects)
    {
        for(int i = 0; i < 4; i++)
            if (objects[i])
                return objects[i].GetComponent<CharacterSheet>();

        return null;
    }

    IEnumerator SwapAndRotate(PositionMapping playerMap, int i, int j, float time, float jumpHeight)
    {
        SwapLocations(playerMap.objects, i, j);
        StartCoroutine(DOMoveToLocation(playerMap, i, time));
        yield return StartCoroutine(DOJumpLocation(playerMap, j, time, jumpHeight));
    }
    IEnumerator DOMoveToLocation(PositionMapping pm, int objectIndex, float time)
    {
        if (pm.objects[objectIndex])
            pm.objects[objectIndex].transform.DOMove(pm.positions[objectIndex].transform.position, time);
        yield return null;
    }
    IEnumerator DOJumpLocation(PositionMapping pm, int objectIndex, float time, float jumpPower)
    {
        if (pm.objects[objectIndex])
            pm.objects[objectIndex].transform.DOJump(pm.positions[objectIndex].transform.position, jumpPower, 1, time);
        yield return null;
    }



    ////**** MOVEMENT ****//
    //IEnumerator SwapPositionsMOVE(GameObject active, int i)
    //{
    //    PositionMapping pm = PositionMappingFromGameObject(active);
    //    int l = Location(active);

    //    if (Location(active) > 3)
    //        Return(active);

    //    GameObject other = pm.objects[i];
    //    CharacterSheet gCS = active.GetComponent<CharacterSheet>();


    //    //Debug.Log(oldLead.characterName + " is the old leader");
    //    //Debug.Log(newLead.characterName + " is the new leader");

    //    if (other)
    //    {
    //        CharacterSheet oCS = other.GetComponent<CharacterSheet>();
    //        if (l != i)
    //            SetDialogueText(gCS.characterName + " swaps with " + oCS.characterName);
    //        else
    //            SetDialogueText(gCS.characterName + " stays in place");
    //    }
    //    else
    //    {
    //        SetDialogueText(gCS.characterName + " moves into a new position.\n");
    //    }

    //    CharacterSheet oldLead = FindLead(pm.objects);
    //    if (l != i)
    //        yield return StartCoroutine(SwapAndRotate(pm, l, i, quickTime, 2));
    //    CharacterSheet newLead = FindLead(pm.objects);

    //    string leaderText = (oldLead == newLead)? newLead.characterName + " stays the leader." : newLead.characterName + " is the new leader.";

    //    AddDialogueText(leaderText);
    //    yield return longWait;
    //}

    //IEnumerator SneakMOVE(GameObject g)
    //{
    //    CharacterSheet activeCharacter = g.GetComponent<CharacterSheet>();
    //    CharacterSheet targetCharacter = FindOppositeLead(g);

    //    SetDialogueText(activeCharacter.characterName + " attempts to sneak.");
    //    yield return mediumWait;

    //    d20 roll = new d20();
    //    bool success = false;

    //    if (roll.value < 9)
    //        AddDialogueText("They seem under prepared.");
    //    else if (roll.value < 13)
    //        AddDialogueText("Everything seems to go right.");
    //    else
    //        AddDialogueText("They seem to vanish");
    //    yield return longWait;

    //    if (roll.Normal())
    //    {
    //        int difficultyRating = 0;
    //        difficultyRating = targetCharacter.GetStrength() + 10;
    //        if (roll.value + activeCharacter.GetAgility() > difficultyRating) success = true;
    //    }

    //    if (success)
    //    {
    //        if(roll.value < 9)
    //            AddDialogueText("But " + targetCharacter.characterName + " still fails to stop them!");
    //        else
    //            AddDialogueText("And they slip by " + targetCharacter.characterName + "!");

    //        PositionMapping pm = PositionMappingFromGameObject(g);
    //        int l = Location(g);
    //        SwapAndMove(pm, l, FindRandomEmptyOpposingLocation(pm.objects), quickTime);
    //    }
    //    else
    //    {
    //        if (roll.value < 9)
    //            AddDialogueText("And " + targetCharacter.characterName + " blocks the way.");
    //        else
    //            AddDialogueText("But " + targetCharacter.characterName + " still blocks the way.");
    //    }

    //    yield return longWait;
    //}

    //bool Return(GameObject g)
    //{
    //    PositionMapping pm = PositionMappingFromGameObject(g);
    //    int l = Location(g);
    //    if (l > 3)
    //    {
    //        SwapAndMove(pm, l, FindRandomHomeLocation(pm.objects), quickTime);
    //        SetDialogueText(g.GetComponent<CharacterSheet>().characterName + " comes back to the party");
    //        return true;
    //    }
    //    else
    //    {
    //        SetDialogueText(g.GetComponent<CharacterSheet>().characterName + " is already with the party");
    //        return false;
    //    }
    //}



    ////**** ACTIONS ****//
    //bool TryAttack(GameObject attackerGO, GameObject targetGO)
    //{
    //    //possible ways to attack
    //    //melee
    //    //  targetGO = enemy lead
    //    //  or attackerGO location > 3
    //    //ranged
    //    //  have ammunition

    //    CharacterSheet attacker = attackerGO.GetComponent<CharacterSheet>();


    //    return false;
    //}

    //bool Attack(CharacterSheet attacker, CharacterSheet target)
    //{
    //    int attackerStrength = attacker.GetStrength();
    //    int difficulty = target.GetAgility() + 10;

    //    d20 roll = new d20();

    //    if (roll.value + attackerStrength > difficulty)
    //    {
    //        int damageMultiplier = 1;

    //        if (roll.critical)
    //            damageMultiplier = 2;

    //        int damage = attacker.GetWeapon().CalculateDamage() * damageMultiplier;

    //        target.TakeDamage(damage);

    //        return true;
    //    }
    //    else
    //    {
    //        if (roll.fumble)
    //        {
    //            TryAttack(target.gameObject, attacker.gameObject);
    //        }

    //        return false;
    //    }
    //}

    //**** THIS IS THE ACTUAL COMBAT STUFF ****//
    IEnumerator Combat()
    {
        yield return null;

        //SetDialogueText("A new battle!!");
        //yield return mediumWait;

        //while (SideIsAlive(playerSide) && SideIsAlive(enemySide))
        //{
        //    int initiative = GameManager.RollDie(2);

        //    if(initiative == 1)
        //    {
        //        SetDialogueText("Players go first this round");
        //        yield return StartCoroutine(PlayerTurn());

        //        SetDialogueText("Enemies finish this round");
        //        yield return StartCoroutine(EnemyTurn());
        //    }
        //    else
        //    {
        //        SetDialogueText("Enemies go first this round");
        //        yield return StartCoroutine(EnemyTurn());

        //        SetDialogueText("Players finish this round");
        //        yield return StartCoroutine(PlayerTurn());
        //    }
        //}

        //SetDialogueText("The battle has been decided!");
        //yield return mediumWait;

        //if (SideIsAlive(playerSide))
        //{
        //    SetDialogueText("You win!!!");
        //}
        //else
        //{
        //    SetDialogueText("The party has fallen...");
        //}
    }

    IEnumerator PlayerTurn()
    {
        yield return null;

        //LoadSideIntoList(playerSide, playersToAct);
        //yield return quickWait;

        //while (playersToAct.Count > 0)
        //{
        //    SetDialogueText("Select a character to act");
        //    currentState = BattleState.SelectCharacter;

        //    yield return null;
        //}
    }

    IEnumerator EnemyTurn()
    {
        yield return null;

        //currentState = BattleState.EnemyTurn;
        //LoadSideIntoList(enemySide, enemiesToAct);
        //yield return mediumWait;

        //while (enemiesToAct.Count > 0)
        //{
        //    int randomIndex = UnityEngine.Random.Range(0, enemiesToAct.Count);
        //    CharacterSheet acting = enemiesToAct[randomIndex];

        //    SetDialogueText("Enemy " + acting.characterName + "'s turn");
        //    yield return new WaitForSeconds(1);

        //    SetDialogueText(acting.characterName + "does this cool thing");
        //    yield return new WaitForSeconds(1);

        //    SetDialogueText("The cool thing sure was cool");
        //    yield return new WaitForSeconds(1);

        //    enemiesToAct.RemoveAt(randomIndex);
        //}
    }

    void LoadSideIntoList(CharacterSheet[] side, List<CharacterSheet> list)
    {
        //list.Clear();
        //foreach(CharacterSheet cs in side)
        //{
        //    if (cs != null &&
        //        cs.hitPoints > 0)
        //        list.Add(cs);
        //}
    }

    bool SideIsAlive(CharacterSheet[] toCheck)
    {
        return false;
        //if (toCheck == null) return false;

        //int totalHP = 0;

        //foreach(CharacterSheet cs in toCheck)
        //{
        //    if (cs != null) totalHP += cs.hitPoints;
        //}

        //return totalHP > 0;
    }
}