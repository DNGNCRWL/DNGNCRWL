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

    public GameObject PartySwapButton;
    public GameObject RecruitButton;
    public GameObject RestButton;
    public GameObject StoreButton;

    //Player Characters
    public List<CharacterSheet> playerCharacters;

    //Recruitable Characters
    public List<CharacterSheet> recruitableCharacters;

    //Swap Party Menu ------------------------------------------------------------
    //Current Party Character Names
    public List<TextMeshProUGUI> charMenuNames;


    //Recruit Character Menu -----------------------------------------------------
    //Recruitable Characters Menu Name
    public List<TextMeshProUGUI> recCharMenuNames;

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

    //Changes all background button to inactive at once
    public void toggleBackground() {
        toggleActive(PartySwapButton);
        toggleActive(RecruitButton);
        toggleActive(RestButton);
        toggleActive(StoreButton);
    }

    //Reset player health
    public void rest() {
        //Char1.RecoverDamage(10);
    }

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

    //Set info about recruitable characters
    public void setRecCharInfo() {
        for (int i = 0; i < 4; ++i) {
            if (i < recruitableCharacters.Count) {
                recCharMenuNames[i].text = recruitableCharacters[i].GetCharacterName();
            } else {
                recCharMenuNames[i].text = "";
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
