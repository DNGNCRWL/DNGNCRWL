using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;

public class TownManager : MonoBehaviour
{
    public Transform TMTransform;
    public Transform GMTransform;
    public GameManager GM;
    public GameObject characterPrefab;

    //Player Characters
    public List<CharacterSheet> playerCharacters;
    public List<CharacterSheet> reserveCharacters;

    //Recruitable Characters
    public List<CharacterSheet> recruitableCharacters;

    //Swap Party Menu ------------------------------------------------------------
    //Current Party Character Names
    public List<GameObject> charMenuTiles;
    public List<GameObject> reserveCharMenuTiles;
    int pageNumber = 0;
    public GameObject incrementPageButton;
    public GameObject decrementPageButton;

    //Recruit Character Menu -----------------------------------------------------
    //Recruitable Character Tiles
    public List<GameObject> recruitMenuTiles;

    void Awake() {
        for (int i = 0; i < 4; ++i) {
            GameObject temp = Instantiate(characterPrefab, TMTransform);
            recruitableCharacters.Add(temp.GetComponent<CharacterSheet>());
        }
        for (int i = 0; i < recruitableCharacters.Count; ++i) {
            recruitableCharacters[i].InitializeRandomClassless();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCharacters = new List<CharacterSheet>(GM.playerCharacters);
        reserveCharacters = new List<CharacterSheet>(GM.reserveCharacters);
        setRecCharInfo();
        setCharInfo();
        setReserveCharInfo();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    //Changes whether an object is active or not
    public void toggleActive(GameObject target) {
        if (target.activeSelf) {
            target.SetActive(false);
        } else {
            target.SetActive(true);
        }
    }

    //Reset player health
    // public void rest() {
    //     foreach (CharacterSheet character in playerCharacters) {
    //         character.RecoverDamage(new Damage(50, 20, 10, new DamageType(Untyped)));
    //     }
    // }

    

    //Set info about characters in player party, if there are more spaces than characters deactivate unused spaces
    public void setCharInfo() {
        for (int i = 0; i < 4; ++i) {
            if (i < playerCharacters.Count) {
                charMenuTiles[i].SetActive(true);
                setInfo(charMenuTiles, i, playerCharacters, i);
            } else {
                charMenuTiles[i].SetActive(false);
            }
        }
    }

    //Set info about characters in player party, if there are more spaces than characters deactivate unused spaces
    public void setReserveCharInfo() {
        //Check for decrement button
        if (pageNumber > 0) {
            decrementPageButton.SetActive(true);
        } else {
            decrementPageButton.SetActive(false);
        }

        //Check for increment button
        if ((pageNumber + 1) * 2 < reserveCharacters.Count) {
            incrementPageButton.SetActive(true);
        } else {
            incrementPageButton.SetActive(false);
        }

        for (int i = 0; i < 2; ++i) {
            if (i + (pageNumber * 2) < reserveCharacters.Count) {
                reserveCharMenuTiles[i].SetActive(true);
                setInfo(reserveCharMenuTiles, i, reserveCharacters, i + pageNumber * 2);
            } else {
                reserveCharMenuTiles[i].SetActive(false);
            }
        }
    }

    //Set info about recruitable characters, if there are more spaces than characters deactivate unused spaces
    public void setRecCharInfo() {
        for (int i = 0; i < 4; ++i) {
            if (i < recruitableCharacters.Count) {
                recruitMenuTiles[i].SetActive(true);
                setInfo(recruitMenuTiles, i, recruitableCharacters, i);
                
            } else {
                recruitMenuTiles[i].SetActive(false);
            }
        }
    }


    public void setInfo(List<GameObject> tiles, int tIndex, List<CharacterSheet> characters, int cIndex) {
        tiles[tIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = characters[cIndex].GetCharacterName();
        tiles[tIndex].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Max HP: " + characters[cIndex].GetMaxHitPoints().ToString();
        tiles[tIndex].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Strength: " + characters[cIndex].GetStrength().ToString();
        tiles[tIndex].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Agility: " + characters[cIndex].GetAgility().ToString();
        tiles[tIndex].transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Presence: " + characters[cIndex].GetPresence().ToString();
        tiles[tIndex].transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "Toughness: " + characters[cIndex].GetToughness().ToString();
    }
    

    //Swap a character from the recruitable character under TownManager to player characters under GameManager
    public void addCharToPlayerChars(int index) {
        GameObject character = recruitableCharacters[index].transform.gameObject;
        CharacterSheet charSheet = character.GetComponent<CharacterSheet>();
        character.transform.parent = GMTransform;
        recruitableCharacters.Remove(charSheet);
        if (playerCharacters.Count < 4) {
            playerCharacters.Add(charSheet);
            GM.playerCharacters.Add(charSheet);
        } else {
            reserveCharacters.Add(charSheet);
            GM.reserveCharacters.Add(charSheet);
        }
        setCharInfo();
        setReserveCharInfo();
        setRecCharInfo();
    }

    //Swap the char at the index from the player party to reserve characters
    public void removeCharFromParty(int index) {
        CharacterSheet charSheet = playerCharacters[index];
        playerCharacters.Remove(charSheet);
        GM.playerCharacters.Remove(charSheet);
        reserveCharacters.Add(charSheet);
        GM.reserveCharacters.Add(charSheet);
        setCharInfo();
        setReserveCharInfo();
    }

    //Swap the char at the index from reserve characters to the player party
    public void addCharToParty(int index) {
        if (playerCharacters.Count < 4) {
            CharacterSheet charSheet = reserveCharacters[index + (pageNumber * 2)];
            playerCharacters.Add(charSheet);
            GM.playerCharacters.Add(charSheet);
            reserveCharacters.Remove(charSheet);
            GM.reserveCharacters.Remove(charSheet);
            setCharInfo();
            setReserveCharInfo();
        }
    }

    public void generateRandom() {
        for (int i = 0; i < 4; ++i) {
            GameObject temp = Instantiate(characterPrefab, TMTransform);
            recruitableCharacters.Add(temp.GetComponent<CharacterSheet>());
        }
        for (int i = 0; i < recruitableCharacters.Count; ++i) {
            recruitableCharacters[i].InitializeRandomClassless();
        }
        setRecCharInfo();
    }

    public void incrementPageNumber() {
        ++pageNumber;
        setReserveCharInfo();
    }
    public void decrementPageNumber() {
        --pageNumber;
        setReserveCharInfo();
    }

    public void enterDungeon() {
        if (playerCharacters.Count > 0) {
            SceneManager.LoadScene("DungeonNavigation", LoadSceneMode.Single);
        }
    }
}
