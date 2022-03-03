using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject characterMenu;
    GameObject[] characterButtons;
    public GameObject moveMenu;
    public GameObject actionMenu;
    public GameObject itemMenu;
    GameObject[] itemButtons;
    public GameObject environmentMenu;
    public GameObject subActionMenu;
    GameObject[] subActionButtons;
    public GameObject targetMenu;
    GameObject[] targetButtons;

    List<CharacterSheet> characters;

    void Awake()
    {
        characterButtons = characterMenu.GetComponent<Menu>().buttons;
        itemButtons = itemMenu.GetComponent<Menu>().buttons;
        targetButtons = targetMenu.GetComponent<Menu>().buttons;
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


    public void OpenCharacterMenu(List<CharacterSheet> yetToAct, List<CharacterSheet> characters)
    {
        characterMenu.SetActive(true);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        subActionMenu.SetActive(false);
        targetMenu.SetActive(false);

        SetUpCharacterMenuLabels(characters);

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
        subActionMenu.SetActive(false);
        targetMenu.SetActive(false);
    }
    public void OpenActionMenu()
    {
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(true);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        subActionMenu.SetActive(false);
        targetMenu.SetActive(false);
    }
    public void OpenItemMenu(List<Item> items)
    {
        Debug.Log("Open Item Menu");

        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(true);
        environmentMenu.SetActive(false);
        subActionMenu.SetActive(false);
        targetMenu.SetActive(false);

        GameObject[] buttons = itemMenu.GetComponent<Menu>().buttons;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < items.Count)
            {
                buttons[i].SetActive(true);

                TextMeshProUGUI[] buttonTexts = itemButtons[i].GetComponentsInChildren<TextMeshProUGUI>();
                buttonTexts[0].text = items[i].itemName;
                buttonTexts[1].text = items[i].description;

                buttons[i].GetComponent<Image>().color = items[i].actions.Count > 0 ? Color.white : Color.grey;
            } else{
                buttons[i].SetActive(false);
            }
        }
    }
    public void OpenEnvironmentMenu()
    {
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(true);
        subActionMenu.SetActive(false);
        targetMenu.SetActive(false);
    }

    public void OpenSubActionMenu(List<CharacterAction> actions){
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        subActionMenu.SetActive(true);
        targetMenu.SetActive(false);

        GameObject[] buttons = subActionMenu.GetComponent<Menu>().buttons;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < actions.Count)
            {
                buttons[i].SetActive(true);

                TextMeshProUGUI[] buttonTexts = itemButtons[i].GetComponentsInChildren<TextMeshProUGUI>();
                buttonTexts[0].text = actions[i].actionName;
                buttonTexts[1].text = "";//actions[i].description;
            } else{
                buttons[i].SetActive(false);
            }
        }
    }
    public void OpenTargetMenu(List<CharacterSheet> targets)
    {
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        subActionMenu.SetActive(false);
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
        subActionMenu.SetActive(false);
        targetMenu.SetActive(false);
    }
}
