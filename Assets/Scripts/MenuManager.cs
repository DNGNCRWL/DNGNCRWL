using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public GameObject characterMenu;
    public GameObject[] characterButtons;
    public GameObject moveMenu;
    public GameObject actionMenu;
    public GameObject itemMenu;
    public GameObject environmentMenu;
    public GameObject targetMenu;

    void Awake(){

    }

    public void SetUpCharacterMenuLabels(List<CharacterSheet> characters){
        for(int i = 0; i < 4; i++){
            CharacterSheet current = null;
            if (i < characters.Count)
                current = characters[i];
            if(current){
                characterButtons[i].SetActive(true);
                TextMeshProUGUI buttonText = characterButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = current.GetCharacterName();
            } else {
                characterButtons[i].SetActive(false);
            }
        }
    }

    public void OpenCharacterMenu(){
        characterMenu.SetActive(true);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        targetMenu.SetActive(false);
    }
    public void OpenMoveMenu(){
        characterMenu.SetActive(false);
        moveMenu.SetActive(true);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        targetMenu.SetActive(false);  
    }
    public void OpenActionMenu(){
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(true);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        targetMenu.SetActive(false);  
    }
    public void OpenItemMenu(){
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(true);
        environmentMenu.SetActive(false);
        targetMenu.SetActive(false);  
    }
    public void OpenEnvironmentMenu(){
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(true);
        targetMenu.SetActive(false);  
    }
    public void OpenTargetMenu(){
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        targetMenu.SetActive(true);  
    }
    public void CloseAllMenus(){
        characterMenu.SetActive(false);
        moveMenu.SetActive(false);
        actionMenu.SetActive(false);
        itemMenu.SetActive(false);
        environmentMenu.SetActive(false);
        targetMenu.SetActive(false);  
    }
}
