using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class TownManager : MonoBehaviour
{
    public Transform TM;
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
            GameObject temp = Instantiate(characterPrefab, TM);
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
    
    public void toggleActive(GameObject target) {
        if (target.activeSelf) {
            target.SetActive(false);
        } else {
            target.SetActive(true);
        }
    }

    public void toggleBackground() {
        toggleActive(PartySwapButton);
        toggleActive(RecruitButton);
        toggleActive(RestButton);
        toggleActive(StoreButton);
    }

    public void rest() {
        //Char1.RecoverDamage(10);
    }

    public void setCharInfo() {
        for (int i = 0; i < playerCharacters.Count; ++i) {
            charMenuNames[i].text = playerCharacters[i].GetCharacterName();
        }
    }

    public void setRecCharInfo() {
        for (int i = 0; i < recruitableCharacters.Count; ++i) {
            recCharMenuNames[i].text = recruitableCharacters[i].GetCharacterName();
        }
    }
}
