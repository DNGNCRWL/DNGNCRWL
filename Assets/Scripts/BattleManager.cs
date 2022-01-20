using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class BattleManager : MonoBehaviour
{
    public static BattleManager BM;
    public GameObject[] characters, enemies;
    public Transform[] playerSidePositions, enemySidePositions;

    public GameObject[] playerObjects, enemyObjects; //the object's index is the position on the map
    public BattleHUD[] characterDisplays;

    [System.Serializable]
    struct PositionMapping
    {
        public Transform[] positions;
        public GameObject[] objects;
    }
    [System.Serializable]
    struct Party
    {
        public Party(string name, List<BattleCharacter> characters)
        {
            this.name = name;
            this.characters = characters;
        }
        public string name;
        public List<BattleCharacter> characters;
    }
    [System.Serializable]
    struct BattleCharacter
    {
        public BattleCharacter(CharacterSheet character, bool sneaking)
        {
            this.name = character.GetCharacterName();
            this.character = character;
            this.sneaking = sneaking;
        }
        public string name;
        public CharacterSheet character;
        public bool sneaking;
    }

    PositionMapping playerMap;
    PositionMapping enemyMap;

    enum BattleState
    {
        Start,
        SelectCharacter,
        ChooseMove, SelectSwapPosition,
        ChooseAction, SelectTarget, ChooseItem, ChooseHazard,
        EnemyTurn,
        Won, Lost
    };
    //    BattleState currentState = BattleState.Start;

    [SerializeField]
    Party playerParty;
    [SerializeField]
    Party enemyParty;

    //UI Stuff
    float snapTime = 0.05f; //3f
    float quickTime = 0.25f; //15f
    float mediumTime = 0.75f; //45f
    float longTime = 1.5f; //90f
    WaitForSeconds snapWait;
    WaitForSeconds quickWait;
    WaitForSeconds mediumWait;
    WaitForSeconds longWait;

    public GameObject actionText;
    public RectTransform actionTextSpawnPosition;
    public float actionTextSpawnOffset;
    public float actionTextTime;
    [SerializeField] List<ActionText> actionTexts;

    public bool test;
    public bool test2;
    int counter = 0;

    private void Update()
    {
        if (test)
        {
            test = false;
            SetDialogueText("This string is: " + counter);
            counter++;
        }

        if (test2)
        {
            test2 = false;
            AddDialogueText("This string is: " + counter);
            counter++;
        }
    }


    private void Awake()
    {
        if (!BM) { BM = this; }
        else { Destroy(gameObject); return; }

        snapWait = new WaitForSeconds(snapTime);
        quickWait = new WaitForSeconds(quickTime);
        mediumWait = new WaitForSeconds(mediumTime);
        longWait = new WaitForSeconds(longTime);

        actionTexts = new List<ActionText>();

        //Set up Parties
        List<BattleCharacter> playerPartyBCs = playerParty.characters = new List<BattleCharacter>();
        characters = GameManager.GM.characters;
        foreach(GameObject go in characters)
        {
            playerPartyBCs.Add(new BattleCharacter(go.GetComponent<CharacterSheet>(), false));
        }

        List<BattleCharacter> enemyPartyBCs = enemyParty.characters = new List<BattleCharacter>();
        foreach(GameObject go in enemies)
        {
            enemyPartyBCs.Add(new BattleCharacter(go.GetComponent<CharacterSheet>(), false));
        }

        //Set up HUDs
        for(int i = 0; i < 4; i++)
        {
            CharacterSheet current = null;
            if (i < playerPartyBCs.Count)
                current = playerPartyBCs[i].character;
            characterDisplays[i].UpdateText(current);
        }

        //// old code vvvvv
        playerObjects = new GameObject[8];
        enemyObjects = new GameObject[8];

        playerMap.positions = playerSidePositions;
        playerMap.objects = playerObjects;
        enemyMap.positions = enemySidePositions;
        enemyMap.objects = enemyObjects;

        for(int i = 0; i < 4; i++)
        {
            if (characters[i])
            {
                playerObjects[i] = characters[i];
            }
            else
            {

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

        //start the first round of combat
    }

    static public void SetDialogueText(string s)
    {
        if (!BM) return;
        //BM.dialogueText.text = s; //make nicer

        BM.ClearDialogueText();

        if (s.Length == 0)
            return;

        GameObject g = Instantiate(BM.actionText, BM.actionTextSpawnPosition);
        g.GetComponent<RectTransform>().Rotate(0, 0, Random.Range(2.0f, 6.0f));
        ActionText newAT = g.GetComponent<ActionText>();
        TextMeshProUGUI newTMP = g.GetComponent<TextMeshProUGUI>();
        newTMP.text = s;

        BM.actionTexts.Add(newAT);
    }
    static public void AddDialogueText(string s)
    {
        if (!BM) return;

        GameObject g = Instantiate(BM.actionText, BM.actionTextSpawnPosition);
        g.GetComponent<RectTransform>().Rotate(0, 0, Random.Range(2.0f, 6.0f));
        RectTransform gRectT = g.GetComponent<RectTransform>();
        gRectT.anchoredPosition = gRectT.anchoredPosition - Vector2.up * BM.actionTextSpawnOffset * BM.actionTexts.Count;
        ActionText newAT = g.GetComponent<ActionText>();
        TextMeshProUGUI newTMP = g.GetComponent<TextMeshProUGUI>();
        newTMP.text = s;
        BM.actionTexts.Add(newAT.GetComponent<ActionText>());

        if (BM.actionTexts.Count > 3)
        {
            ActionText head = BM.actionTexts[0];
            head.MoveRelative(Random.Range(BM.actionTextSpawnOffset, -BM.actionTextSpawnOffset),
                Random.Range(-BM.actionTextSpawnOffset, -2 * BM.actionTextSpawnOffset), BM.actionTextTime);
            head.FadeAndDestroy(BM.actionTextTime);
            BM.actionTexts.Remove(head);

            foreach(ActionText eachAT in BM.actionTexts)
            {
                eachAT.MoveRelative(0, BM.actionTextSpawnOffset, BM.actionTextTime);
            }
        }
    }
    void ClearDialogueText()
    {
        foreach(ActionText at in actionTexts)
        {
            at.MoveRelative(Random.Range(actionTextSpawnOffset, -actionTextSpawnOffset), Random.Range(-actionTextSpawnOffset, -2*actionTextSpawnOffset), actionTextTime);
            at.FadeAndDestroy(actionTextTime);
        }
        actionTexts.Clear();
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
    public static CharacterSheet GetOppositeLead(CharacterSheet c)
    {
        if (!BM)
            return null;

        GameObject cGO = c.gameObject;

        return BM.FindOppositeLead(cGO);
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

    public CharacterSheet GetOpposingLeader(CharacterSheet cs) { return null; }
    public CharacterSheet[] GetAllOpponents(CharacterSheet cs) { return null; }
    public bool SameSide(CharacterSheet actor, CharacterSheet target) { return false; }

    public void Push(CharacterSheet cs) { } //do this
    public void Sneak(CharacterSheet cs)
    {
        GameObject g = cs.gameObject;
        PositionMapping pm = PositionMappingFromGameObject(g);
        int l = Location(g);
        SwapAndMove(pm, l, FindRandomEmptyOpposingLocation(pm.objects), quickTime);
    }
    public bool Return(CharacterSheet cs)
    {
        GameObject g = cs.gameObject;

        PositionMapping pm = PositionMappingFromGameObject(g);
        int l = Location(g);
        if (l > 3)
        {
            SwapAndMove(pm, l, FindRandomHomeLocation(pm.objects), quickTime);
            SetDialogueText(cs.GetCharacterName() + " comes back to the party");
            return true;
        }
        else
        {
            SetDialogueText(cs.GetCharacterName() + " is already with the party");
            return false;
        }
    }

    IEnumerator Combat()
    {
        yield return null;
    }

    IEnumerator PlayerTurn()
    {
        yield return null;
    }

    IEnumerator EnemyTurn()
    {
        yield return null;
    }

    void LoadSideIntoList(CharacterSheet[] side, List<CharacterSheet> list)
    {

    }

    bool SideIsAlive(CharacterSheet[] toCheck)
    {
        return false;
    }
}

