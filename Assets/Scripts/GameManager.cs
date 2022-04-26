using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager GM;
    public static DungeonGenerator DG;
    public static Navigation NAV;
    public List<CharacterSheet> playerCharacters;
    public List<CharacterSheet> reserveCharacters;


    public static readonly int secondsPerRound = 6;

    public static ActionTextSpawner DEFAULT_TEXT_SPAWNER;

    public static CharacterSheet testC;


    private void Awake() {
        if (GM == null) GM = this;
        else { Destroy(gameObject); return; }
        GameObject.DontDestroyOnLoad(this.gameObject);

        DOTween.Init(null, null, null);

        //initialize menus if present and not active
        if (UI_PartyMenu.UI_PARTYMENU == null) {
            UI_PartyMenu partyMenu = null;
            var canvases = Resources.FindObjectsOfTypeAll<UI_PartyMenu>();
            if (canvases.Length > 0)
                partyMenu = canvases[0];

            if (partyMenu != null)
                partyMenu.OpenPartyUI();
        }

        if (UI_Inventory.UI_INVENTORY == null) {
            UI_Inventory inventoryMenu = null;
            var canvases = Resources.FindObjectsOfTypeAll<UI_Inventory>();
            if (canvases.Length > 0)
                inventoryMenu = canvases[0];

            if (inventoryMenu != null)
                inventoryMenu.OpenInventoryUI();
        }
        //Debug.Log("GM");
    }

    private void Start() {
        
    }

    private void Update() {
        if(Input.GetKeyDown("escape")) {
            ToggleEscapeMenu();
        }
    }

    public static void Reset() {
        Navigation.Respawn();
        GM.playerCharacters.Clear();
        GM.reserveCharacters.Clear();
    }

    public void CompleteLevel() {
        Debug.Log("Dungeon reload");
    }
    public static void StartGame() {

    }

    public static IEnumerator DisplayMessagePackage(MessagePackage mp, float target, string[] toInsert) {
        if (mp.messages.Length <= 0)
            yield break;

        if (mp.messages.Length > 0) {
            string toDisplay = string.Format(
                Fun.WeightedRandomFromArray(mp, target),
                toInsert);

            GM.AddText(toDisplay);
            yield return new WaitForSeconds(mp.time);
        }
    }

    public void AddText(string message) {
        if (DEFAULT_TEXT_SPAWNER)
            DEFAULT_TEXT_SPAWNER.AddText(message);
        else
            Debug.Log("No Default Text Spawner to add message: " + message);
    }

    public void SetText(string message) {
        if (DEFAULT_TEXT_SPAWNER)
            DEFAULT_TEXT_SPAWNER.SetText(message);
        else
            Debug.Log("No Default Text Spawner to set message: " + message);
    }

    public void ClearText() {
        if (DEFAULT_TEXT_SPAWNER)
            DEFAULT_TEXT_SPAWNER.ClearText();
        else
            Debug.Log("No Default Text Spawner to clear");
    }

    static public int RollDie(int dieSize) {
        if (dieSize < 0)
            return UnityEngine.Random.Range(0, -dieSize) - 1;
        else
            return UnityEngine.Random.Range(0, dieSize) + 1;
    }
    static public int RollDice(int number, int dieSize) {
        int result = 0;
        for (int i = 0; i < number; i++) {
            result += RollDie(dieSize);
        }
        return result;
    }
    public static bool Error(string message) {
        Debug.Log(message);
        return false;
    }

    public static CharacterSheet GetTarget() {
        //if in battle list enemies
        //if in status menu list allies
        return null;
    }

    //Enums to String???
    public static string DamageTypeToString(DamageType type) {
        switch (type) {
            case DamageType.Bludgeon: return "Bludgeon";
            case DamageType.Cut: return "Cut";
            case DamageType.Electric: return "Electric";
            case DamageType.Fire: return "Fire";
            case DamageType.Magic: return "Magic";
            case DamageType.Pierce: return "Pierce";
            case DamageType.Spirit: return "Spirit";
        }

        return "Untyped";
    }

    public  List<CharacterSheet> GetDeadCharacters () {
        List<CharacterSheet> output = new List<CharacterSheet>();

        foreach (CharacterSheet character in playerCharacters) {
            if (character.IsDead()) output.Add(character);
        }

        return output;
    }

    public void MoveCharToFront(CharacterSheet character) {
        Debug.Log("Move Char to Front");
        playerCharacters.Remove(character);
        playerCharacters.Insert(0, character);
    }

    public void CheckLoadLootMenu() {
        Debug.Log(BattleManager.BATTLE_LOOT);
        //loot menu
        if (BattleManager.BATTLE_LOOT != null) {
            Debug.Log("LootPresent");
            Inventory tempLootInv = new Inventory();
            tempLootInv.AddItem(BattleManager.BATTLE_LOOT, BattleManager.BATTLE_LOOT.amount);
            List<CharacterSheet> deadChars = GameManager.GM.GetDeadCharacters();
            foreach (CharacterSheet c in deadChars) {
                tempLootInv.AddItemsFromList(c.inventory.GetItemList());
                c.inventory.ClearInventory();
            }

            BattleManager.BATTLE_LOOT = null;

            //init menu
            if (UI_LootMenu.UI_LOOTMENU == null) {
                UI_LootMenu lootMenu = null;
                var canvases = Resources.FindObjectsOfTypeAll<UI_LootMenu>();
                if (canvases.Length > 0)
                    lootMenu = canvases[0];

                if (lootMenu != null)
                    lootMenu.OpenLootUI();
            }

            UI_LootMenu.UI_LOOTMENU.SetInventory(tempLootInv);
            UI_LootMenu.UI_LOOTMENU.OpenLootUI();
        }
    }
    

    //SCENE NAVIGATION
    public static void GoToDungeonNavigation() {
        SceneManager.LoadScene("DungeonGeneration");
        PartySetActive(false);
        Debug.Log("Load DungeonGeneration");
    }

    public static void PartySetActive(bool b){
        foreach(CharacterSheet charSheet in GM.playerCharacters)
            charSheet.gameObject.SetActive(b);
    }
    public static void GameOver() {
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
        Reset();
    }

    public bool PickupLoot (Item loot) {
        foreach (CharacterSheet character in playerCharacters) {
            if(character.PickupItem(loot)) return true;
        }
        return false;
    }

    public static void ToggleEscapeMenu() {
        GameObject escapeMenu = GameObject.FindWithTag("EscapeMenu").transform.GetChild(0).gameObject;
        escapeMenu.SetActive(!escapeMenu.activeSelf);

    }

    public static void QuitGame() {
        Application.Quit();
    }

}

[System.Serializable]
public enum StatSelector { Strength, Agility, Presence, Toughness, Defense, WeaponStat }

[System.Serializable]
public enum Stat { Strength, Agility, Presence, Toughness, Defense };

[System.Serializable]
public struct CharacterRollingPackage {
    public CharacterRollingPackage(int strength, int agility, int presence, int toughness, int hpDieSize,
        int powers, int omens, int silverDieCount, int silverDieSize, int food) {
        this.strength = strength;
        this.agility = agility;
        this.presence = presence;
        this.toughness = toughness;
        this.hpDieSize = hpDieSize;
        this.powers = powers;
        this.omens = omens;
        this.silverDieCount = silverDieCount;
        this.silverDieSize = silverDieSize;
        this.food = food;
    }

    public int strength;
    public int agility;
    public int presence;
    public int toughness;
    public int hpDieSize;
    public int powers;
    public int omens;
    public int silverDieCount;
    public int silverDieSize;
    public int food;
}