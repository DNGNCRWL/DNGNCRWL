using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject characterMenu;
    public GameObject[] characterButtons;
    public GameObject moveMenu;
    public GameObject actionMenu;
    public GameObject itemMenu;
    public GameObject environmentMenu;
    public GameObject targetMenu;
    public GameObject[] targetButtons;

    List<CharacterSheet> characters;

    void Awake()
    {

    }

    public void SetUpCharacterMenuLabels(List<CharacterSheet> characters)
    {
        this.characters = characters;
        SetUpCharacterMenuLabels();
    }

    public void SetUpCharacterMenuLabels()
    {
        for (int i = 0; i < 4; i++)
        {
            CharacterSheet current = null;
            if (i < characters.Count)
                current = characters[i];
            if (current)
            {
                characterButtons[i].SetActive(true);
                TextMeshProUGUI[] buttonTexts = characterButtons[i].GetComponentsInChildren<TextMeshProUGUI>();
                buttonTexts[0].text = current.GetCharacterName();
                buttonTexts[1].text = current.GetBattleOrderString();
            }
            else
            {
                characterButtons[i].SetActive(false);
            }
        }
    }


    public void OpenCharacterMenu(List<CharacterSheet> yetToAct)
    {
        characterMenu.SetActive(true);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        targetMenu.SetActive(false);

        SetUpCharacterMenuLabels();

        for(int i = 0; i < 4; i++){
            CharacterSheet current = null;
            if(i < characters.Count)
                current = characters[i];
            if(current){
                bool hasYetToAct = false;
                foreach(CharacterSheet character in yetToAct){
                    if (current == character)
                        hasYetToAct = true;
                }
                characterButtons[i].GetComponent<Image>().color = hasYetToAct ? Color.white : Color.grey;
            }
        }
    }
    public void OpenMoveMenu()
    {
        characterMenu.SetActive(false);
        moveMenu.SetActive(true);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        targetMenu.SetActive(false);
    }
    public void OpenActionMenu()
    {
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(true);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        targetMenu.SetActive(false);
    }
    public void OpenItemMenu()
    {
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(true);
        environmentMenu.SetActive(false);
        targetMenu.SetActive(false);
    }
    public void OpenEnvironmentMenu()
    {
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(true);
        targetMenu.SetActive(false);
    }
    public void OpenTargetMenu(List<CharacterSheet> targets)
    {
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        targetMenu.SetActive(true);

        GameObject[] buttons = targetMenu.GetComponent<Menu>().buttons;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < targets.Count)
            {
                buttons[i].SetActive(true);

                TextMeshProUGUI[] buttonTexts = targetButtons[i].GetComponentsInChildren<TextMeshProUGUI>();
                buttonTexts[0].text = targets[i].GetCharacterName();
                buttonTexts[1].text = targets[i].GetBattleOrderString();
            } else{
                buttons[i].SetActive(false);
            }
        }
    }
    public void CloseAllMenus()
    {
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        targetMenu.SetActive(false);
    }
}
