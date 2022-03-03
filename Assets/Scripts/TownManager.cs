using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class TownManager : MonoBehaviour
{
    public Transform TMTransform;
    public Transform GMTransform;
    public GameManager GM;
    public GameObject characterPrefab;

    //Player Characters
    public List<CharacterSheet> playerCharacters;

    //Recruitable Characters
    public List<CharacterSheet> recruitableCharacters;

    //Swap Party Menu ------------------------------------------------------------
    //Current Party Character Names
    public List<TextMeshProUGUI> charMenuNames;


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
        setRecCharInfo();
        setCharInfo();
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

    //Set info about characters in player party
    public void setCharInfo() {
        for (int i = 0; i < 6; ++i) {
            if (i < playerCharacters.Count) {
                charMenuNames[i].text = playerCharacters[i].GetCharacterName();
            } else {
                charMenuNames[i].text = "";
            }
        }
    }

    //Set info about recruitable characters, if there are more spaces than characters deactivate unused spaces
    public void setRecCharInfo() {
        for (int i = 0; i < 4; ++i) {
            if (i < recruitableCharacters.Count) {
                recruitMenuTiles[i].SetActive(true);
                recruitMenuTiles[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = recruitableCharacters[i].GetCharacterName();
            } else {
                recruitMenuTiles[i].SetActive(false);
            }
        }
    }

    //Swap a character from the recruitable character under TownManager to player characters under GameManager
    public void addCharToParty(int index) {
        GameObject character = recruitableCharacters[index].transform.gameObject;
        character.transform.parent = GMTransform;
        recruitableCharacters.Remove(character.GetComponent<CharacterSheet>());
        playerCharacters.Add(character.GetComponent<CharacterSheet>());
        GM.playerCharacters.Add(character.GetComponent<CharacterSheet>());
        setCharInfo();
        setRecCharInfo();
    }
}
