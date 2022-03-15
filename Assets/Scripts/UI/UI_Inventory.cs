using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class UI_Inventory : MonoBehaviour
{
    public static UI_Inventory UI_INVENTORY;
    public CharacterSheet targetCharacter;
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private Transform itemSlotContainer;
    public GameObject itemSlotTemplate;

    private Transform armorSlotContainer;

    private Transform bagSlotContainer;

    private Transform mainhandSlotContainer;

    private Transform offhandSlotContainer;


    void Awake() {
        if (UI_INVENTORY == null)
        {
            UI_INVENTORY = this;
        } else {
            Destroy(gameObject);
        }

        itemSlotContainer = transform.Find("itemSlotContainer");
        armorSlotContainer = transform.Find("CharacterContainer").Find("armorSlotContainer");
        bagSlotContainer = transform.Find("CharacterContainer").Find("bagSlotContainer");
        mainhandSlotContainer = transform.Find("CharacterContainer").Find("mainhandSlotContainer");
        offhandSlotContainer = transform.Find("CharacterContainer").Find("offhandSlotContainer");
        CloseInventoryUI();
        //Debug.Log(itemSlotContainer);
    }

    public void SetCharacterTarget (CharacterSheet character){
        targetCharacter = character;
        SetInventory(character.GetInventory());
        if (UI_ContextMenu.UI_CONTEXTMENU != null) {
            UI_ContextMenu.UI_CONTEXTMENU.targetCharacter = character;
        }
    }

    public void SetInventory (Inventory inventory) {
        if(this.inventory!=null)
            this.inventory.OnItemListChanged -= Inventory_OnItemListChanged;

        this.inventory = inventory;

        inventory.OnItemListChanged += Inventory_OnItemListChanged;

        RefreshInventoryItems();
        Debug.Log("SetInv");

    }

    private void Inventory_OnItemListChanged (object sender, System.EventArgs e) {
        Debug.Log("Inventory Refreshed");
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems() {
        //Debug.Log("Refresh");
        foreach (Transform child in itemSlotContainer){
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        int x = 0;
        int y = 0;
        float itemSlotCellSizeY = 90f;
        float itemSlotCellSizeX = 250f;
        foreach(Item item in inventory.GetItemList()) {
            int count = item.amount;
            int countShown = 0;
            do
            {
                GameObject itemSlotNew = Instantiate(itemSlotTemplate, itemSlotContainer);
                itemSlotHandler handler = itemSlotNew.GetComponentInChildren<itemSlotHandler>();
                RectTransform itemSlotRectTransform = itemSlotNew.GetComponent<RectTransform>();
                itemSlotRectTransform.gameObject.SetActive(true);
                itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSizeX, -y * itemSlotCellSizeY);
                UnityEngine.UI.Image image = itemSlotRectTransform.Find("icon").gameObject.GetComponent<UnityEngine.UI.Image>();

                if (item.GetSprite() != null)
                {  //Default empty sprite if not set
                    image.sprite = item.GetSprite();
                } else {
                    image.color = Color.white; 
                }

                TextMeshProUGUI nameUI = itemSlotRectTransform.Find("name").GetComponent<TextMeshProUGUI>();
                nameUI.SetText(item.itemName);

                TextMeshProUGUI uiCount = itemSlotRectTransform.Find("count").GetComponent<TextMeshProUGUI>();
                int remaining = count - countShown;
                //Debug.Log("Name: " + item.itemName + " Stackable: " + item.IsStackable().ToString()+ " StackLimit: " + item.stackLimit +  "\n  amount: " + item.amount+ " remaining: " + remaining);

                handler.item = item;
                handler.inventory = inventory;

                if (item.amount > 1 && item.IsStackable())
                {
                    if (remaining > item.stackLimit) {
                        uiCount.SetText(item.stackLimit.ToString());
                        countShown += item.stackLimit;
                    }
                    else
                    {
                        uiCount.SetText(remaining.ToString());
                        countShown += remaining;
                    }
                }
                else
                {
                    countShown++;
                    uiCount.SetText("");
                }


                x++;
                if (x > 1)
                {
                    x = 0;
                    y++;
                }
            } while (countShown<count);

        }

        RefreshCharacterUI();
    }

    private void RefreshCharacterUI() {
        RectTransform armorSlot = armorSlotContainer.Find("itemSlot").GetComponent<RectTransform>();
       
        if (inventory.armor != null) {
            SetSlotItem(armorSlot, inventory.armor);
        } else {
            //Debug.Log("Armor Slot Empty");
            SetSlotBlank(armorSlot);
        }

        RectTransform bagSlot = bagSlotContainer.Find("itemSlot").GetComponent<RectTransform>();
        if (inventory.storage != null) {
            SetSlotItem(bagSlot, inventory.storage);
        } else {
            //Debug.Log("Bag Slot Empty");
            SetSlotBlank(bagSlot);
        }

        RectTransform mainhandSlot = mainhandSlotContainer.Find("itemSlot").GetComponent<RectTransform>();
        if (inventory.mainHand != null) {
            SetSlotItem(mainhandSlot, inventory.mainHand);
        } else {
            //Debug.Log("MainHand Slot Empty");
            SetSlotBlank(mainhandSlot);
        }

        RectTransform offhandSlot = offhandSlotContainer.Find("itemSlot").GetComponent<RectTransform>();
        if (inventory.offHand != null) {
            SetSlotItem(offhandSlot, inventory.offHand);
        } else {
           // Debug.Log("OffHand Slot Empty");
            SetSlotBlank(offhandSlot);
        }

    }

    private void SetSlotItem(RectTransform itemSlot, Item item) {
        itemSlotHandler handler = itemSlot.gameObject.GetComponentInChildren<itemSlotHandler>();
        itemSlot.gameObject.SetActive(true);
        UnityEngine.UI.Image image = itemSlot.Find("icon").gameObject.GetComponent<UnityEngine.UI.Image>();

        if (item.GetSprite() != null) {  //Default empty sprite if not set
            image.sprite = item.GetSprite();
        } else {
            image.color = Color.white;
        }

        TextMeshProUGUI nameUI = itemSlot.Find("name").GetComponent<TextMeshProUGUI>();
        nameUI.SetText(item.itemName);

        TextMeshProUGUI uiCount = itemSlot.Find("count").GetComponent<TextMeshProUGUI>();
        uiCount.SetText("");

        handler.item = item;
        handler.inventory = inventory;
    }

    private void SetSlotBlank (RectTransform itemSlot) {
        itemSlot.Find("icon").gameObject.SetActive(false);
        itemSlot.Find("name").gameObject.SetActive(false);
        itemSlot.Find("count").gameObject.SetActive(false);
        itemSlot.Find("border").gameObject.SetActive(false);
        UnityEngine.UI.Image background = itemSlot.Find("background").GetComponent<UnityEngine.UI.Image>();
        var color = Color.black;
        color.a = 0.5f;
        background.color = color;
    }
    public List<Item> GetItemList() {
        return inventory.GetItemList();
    }

    public void OpenInventoryUI() {
        gameObject.SetActive(true);
    }
    public void CloseInventoryUI() {
        gameObject.SetActive(false);

        if(UI_ContextMenu.UI_CONTEXTMENU != null ) {
            UI_ContextMenu.UI_CONTEXTMENU.HideContextMenu();
        }
    }

}
