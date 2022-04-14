using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ContextMenu : MonoBehaviour
{

    public static UI_ContextMenu UI_CONTEXTMENU;

    [SerializeField]
    private Camera uiCamera;
    private Transform optionContainer;
    private Transform expandContainer;
    public GameObject optionContainerBG;
    public GameObject expandContainerBG;
    public GameObject button;
    public GameObject expandableButton;

    public CharacterSheet targetCharacter;
    public Item targetItem;
    public Inventory targetInventory;

    private float expandOffset;

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

        //HideContextMenu();
    }

    private void Update()
    {
        

    }

    private void RefreshContextMenu() {
        foreach (Transform child in optionContainer) {
            if (child.name == "background") continue;
            Destroy(child.gameObject);
        }

        foreach (Transform child in expandContainer) {
            if (child.name == "backgroundExpanded") continue;
            Destroy(child.gameObject);
        }

        float backgroundXBuffer = 5f;
        float backgroundYBuffer = 5f;
        float buttonYBuffer = 5f;

        //OptionMenu
        float largestBtnWidth = 0;
        float largestBtnHeight = 0;
        int y = 0;
        //Debug.Log(targetItem.inventoryActions);

        GameObject optionContainerBackground = Instantiate(optionContainerBG, optionContainer);
        RectTransform optionBackgroundTransform = optionContainerBackground.GetComponent<RectTransform>();
        optionBackgroundTransform.gameObject.SetActive(true);

        foreach (CharacterAction action in  targetItem.inventoryActions) {
            //Debug.Log("Button Added at" + y);
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
            if (btnWidth > largestBtnWidth) largestBtnWidth = btnWidth;
            float btnheight = newBtnRectTransform.rect.height;
            if (btnheight > largestBtnHeight) largestBtnHeight = btnheight;
            //set position
            
            newBtnRectTransform.anchoredPosition = new Vector2((btnWidth/4) + backgroundXBuffer + 5, -(y * (btnheight + buttonYBuffer)  + backgroundYBuffer));

            Button invButton = newBtnRectTransform.Find("Button").GetComponent<Button>();
            if (action.targetingType == CharacterAction.TargetingType.Allies) {
                invButton.onClick.AddListener(delegate { ExpandContextMenu(action); });
            } else {
                invButton.onClick.AddListener(delegate { DoAction(action); });
            }

            TextMeshProUGUI newBtnText = newBtnRectTransform.Find("Button").Find("text").GetComponent<TextMeshProUGUI>();
            newBtnText.SetText(action.actionName);

            y++;
        }

        GameObject closeBtn = Instantiate(button, optionContainer);
        RectTransform closeBtnRectTransform = closeBtn.GetComponent<RectTransform>();
        closeBtnRectTransform.gameObject.SetActive(true);
        float closeBtnWidth = closeBtnRectTransform.rect.width;
        float closeBtnheight = closeBtnRectTransform.rect.height;
        closeBtnRectTransform.anchoredPosition = new Vector2((closeBtnWidth / 4) + backgroundXBuffer + 5, -(y * (closeBtnheight + buttonYBuffer) + backgroundYBuffer));

        Button closeInvButton = closeBtnRectTransform.Find("Button").GetComponent<Button>();
        closeInvButton.onClick.AddListener(delegate { HideContextMenu(); });

        TextMeshProUGUI closeBtnText = closeBtnRectTransform.Find("Button").Find("text").GetComponent<TextMeshProUGUI>();
        closeBtnText.SetText("Close");

        y++;
        
        optionBackgroundTransform.sizeDelta = new Vector2(largestBtnWidth + backgroundXBuffer * 2, y * (largestBtnHeight + buttonYBuffer) + backgroundYBuffer*2);
        optionBackgroundTransform.anchoredPosition = new Vector2( largestBtnWidth/4 + backgroundXBuffer + 5 , -(y*largestBtnHeight)/2 + backgroundYBuffer);
        //Debug.Log(-(y * largestBtnHeight) / 2);

        expandOffset = largestBtnWidth + backgroundXBuffer * 2;

    }

    public void ShowContextMenu(Item item, Inventory inv)
    {
        targetItem = item;
        targetInventory = inv;
        //Debug.Log(item);
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

    private void ExpandContextMenu(CharacterAction action)
    {
        if (action.targetingType == CharacterAction.TargetingType.Allies) {

            GameObject expandContainerBackground = Instantiate(expandContainerBG, optionContainer);
            RectTransform expandBackgroundTransform = expandContainerBackground.GetComponent<RectTransform>();
            expandBackgroundTransform.gameObject.SetActive(true);

            float backgroundXBuffer = 5f;
            float backgroundYBuffer = 5f;
            float buttonYBuffer = 5f;

            int y = 0;
            float btnWidth = 0;
            float btnheight = 0;
            foreach (CharacterSheet p  in GameManager.GM.playerCharacters) {
                GameObject newBtn = Instantiate(button, optionContainer);
                
                RectTransform newBtnRectTransform = newBtn.GetComponent<RectTransform>();
                newBtnRectTransform.gameObject.SetActive(true);

                btnWidth = newBtnRectTransform.rect.width;
                btnheight = newBtnRectTransform.rect.height;
                //set position

                newBtnRectTransform.anchoredPosition = new Vector2(expandOffset + (btnWidth / 4) + backgroundXBuffer + 10, -(y * (btnheight + buttonYBuffer) + backgroundYBuffer));

                Button invButton = newBtnRectTransform.Find("Button").GetComponent<Button>();
                invButton.onClick.AddListener(delegate { DoTargetedAction(action, p); });

                TextMeshProUGUI newBtnText = newBtnRectTransform.Find("Button").Find("text").GetComponent<TextMeshProUGUI>();
                newBtnText.SetText(p.characterName);
                y++;
            }

            //Debug.Log(-(y * btnheight) / 2);
            expandBackgroundTransform.sizeDelta = new Vector2(btnWidth + backgroundXBuffer * 2, y * (btnheight + buttonYBuffer) + backgroundYBuffer * 2);
            expandBackgroundTransform.anchoredPosition = new Vector2(expandOffset + (btnWidth / 4) + backgroundXBuffer + 10, -(y * btnheight) / 2 + backgroundYBuffer*2);
        }
    }

    private void DoAction(CharacterAction action) {
        Debug.Log("Action: " + action.actionName);
        if (action.actionName == "Use On Self") {
            targetCharacter.UseConsumable((Consumable)targetItem);
        } else if (action.actionName == "Discard") {
            Debug.Log("Discard");
            Debug.Log(targetInventory.RemoveItem(targetItem));
        }
        HideContextMenu();
    }

    private void DoTargetedAction(CharacterAction action, CharacterSheet target) {
        if (action.actionName == "Use On") {
            targetCharacter.UseConsumableOn(target, (Consumable)targetItem);
        } else if (action.actionName == "Transfer") {
            target.inventory.AddItem(targetItem.Copy());
            targetInventory.RemoveItem(targetItem);
            
        }
        HideContextMenu();
    }

}
