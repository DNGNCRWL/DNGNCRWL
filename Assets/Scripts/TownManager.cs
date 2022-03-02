using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class TownManager : MonoBehaviour
{
    public GameObject PartySwapButton;
    public GameObject RecruitButton;
    public GameObject RestButton;
    public GameObject StoreButton;

    //Player Characters
    public CharacterSheet Char1;
    public CharacterSheet Char2;
    public CharacterSheet Char3;
    public CharacterSheet Char4;

    //Recruitable Characters
    public CharacterSheet RecChar1;
    public CharacterSheet RecChar2;
    public CharacterSheet RecChar3;
    public CharacterSheet RecChar4;

    public TextMeshProUGUI RecChar1Name;
    public TextMeshProUGUI RecChar2Name;
    public TextMeshProUGUI RecChar3Name;
    public TextMeshProUGUI RecChar4Name;

    void Awake() {

    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("RecChar1 Name: " + RecChar1.GetCharacterName());
        setRecCharInfo();
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

    public void setRecCharInfo() {
        RecChar1Name.text = RecChar1.GetCharacterName();
        RecChar2Name.text = RecChar2.GetCharacterName();
        RecChar3Name.text = RecChar3.GetCharacterName();
        RecChar4Name.text = RecChar4.GetCharacterName();
    }
}
