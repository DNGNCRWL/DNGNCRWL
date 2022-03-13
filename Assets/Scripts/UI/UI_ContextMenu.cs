using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class UI_ContextMenu : MonoBehaviour
{

    public static UI_ContextMenu UI_CONTEXTMENU;

    [SerializeField]
    private Camera uiCamera;
    private Transform optionContainer;
    private Transform expandContainer;
    private Transform optionContainerBG;
    private Transform expandContainerBG;
    public GameObject button;
    public GameObject expandableButton;

    public CharacterSheet targetCharacter;
    public Item targetItem;

    void Awake()
    {
        if (UI_CONTEXTMENU == null)
        {
            UI_CONTEXTMENU = this;
        }
        else
        {
            Destroy(gameObject);
        }
        optionContainer = transform.Find("optionContainer");
        expandContainer = transform.Find("expandContainer");
        optionContainerBG = optionContainer.Find("background");
        expandContainerBG = expandContainer.Find("background");

        HideContextMenu();
    }

    private void Update()
    {
        

    }

    private void RefreshContextMenu() {
        float backgroundXBuffer = 90f;
        float backgroundYBuffer = 30f;
        float buttonYBuffer = 5f;

        float largestBtnWidth = 0;
        int y = 0;
        Debug.Log(targetItem.inventoryActions);
        foreach (CharacterAction action in  targetItem.inventoryActions) {
            GameObject newBtn;
            if (action.targetingType == CharacterAction.TargetingType.Allies) {
                newBtn = Instantiate(expandableButton, optionContainer);
                
            }
            else {
                newBtn = Instantiate(button, optionContainer);
            }
            RectTransform newBtnRectTransform = newBtn.GetComponent<RectTransform>();
            newBtnRectTransform.gameObject.SetActive(true);
            
            float btnWidth = newBtnRectTransform.rect.width;
            if (btnWidth<largestBtnWidth) largestBtnWidth = btnWidth;
            float btnheight = newBtnRectTransform.rect.height;
            //set position
            newBtnRectTransform.anchoredPosition = new Vector2((btnWidth / 2) + backgroundXBuffer, (-y * (btnheight + buttonYBuffer)) + backgroundYBuffer);
        }
    }

    public void ShowContextMenu(Item item)
    {
        targetItem = item;
        Debug.Log(item);
        gameObject.SetActive(true);
        RefreshContextMenu();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, uiCamera, out localPoint);
        transform.localPosition = localPoint;
    }

    public void HideContextMenu()
    {
        gameObject.SetActive(false);
    }

    public void ExpandContextMenu(CharacterAction.TargetingType targetingType)
    {

    }



}
