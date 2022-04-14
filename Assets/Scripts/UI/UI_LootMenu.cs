using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_LootMenu : MonoBehaviour
{
    public static UI_LootMenu UI_LOOTMENU;
    [SerializeField]
    private Inventory lootInventory;    
    public GameObject itemSlotTemplate;

    public GameObject itemSmallSlotTemplate;

    private Transform LootContainer;

    private Transform Inv1Container;

    private Transform Inv2Container;

    private Transform Inv3Container;

    private Transform Inv4Container;
    private Transform CharName1;
    private Transform SlotLimit1;
    private Transform CharName2;
    private Transform SlotLimit2;
    private Transform CharName3;
    private Transform SlotLimit3;
    private Transform CharName4;
    private Transform SlotLimit4;

    private void Awake() {
        if (UI_LOOTMENU == null) {
            UI_LOOTMENU = this;
        } else {
            Destroy(gameObject);
        }

        LootContainer = transform.Find("LootContainer");
        Inv1Container = transform.Find("Inv1Container");
        Inv2Container = transform.Find("Inv2Container");
        Inv3Container = transform.Find("Inv3Container");
        CharName1 = transform.Find("CharName1");
        SlotLimit1 = transform.Find("SlotLimit1");
        CharName2 = transform.Find("CharName2");
        SlotLimit2 = transform.Find("SlotLimit2");
        CharName3 = transform.Find("CharName3");
        SlotLimit3 = transform.Find("SlotLimit3");
        CharName4 = transform.Find("CharName4");
        SlotLimit4 = transform.Find("SlotLimit4");

        CloseLootUI();
        //Debug.Log(itemSlotContainer);
    }

    private void OnDestroy() {
        if (lootInventory != null)
            lootInventory.OnItemListChanged -= LootInventory_OnItemListChanged;
    }

    private void LootInventory_OnItemListChanged(object sender, System.EventArgs e) {
        Debug.Log("Inventory Refreshed");
        if (GetComponent<RectTransform>()) {
            RefreshLootInventoryItems();
            RefreshCharInventories();
        }
    }


    private void RefreshLootInventoryItems () {

        foreach (Transform child in LootContainer) {
            if (child != itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        int x = 0;
        int y = 0;
        float itemSlotCellSizeY = 90f;
        float itemSlotCellSizeX = 250f;
        foreach (Item item in lootInventory.GetItemList()) {
            int count = item.amount;
            int countShown = 0;

            do {
                GameObject itemSlotNew = Instantiate(itemSlotTemplate, LootContainer);
                itemSlotHandler handler = itemSlotNew.GetComponentInChildren<itemSlotHandler>();
                RectTransform itemSlotRectTransform = itemSlotNew.GetComponent<RectTransform>();
                itemSlotRectTransform.gameObject.SetActive(true);
                itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSizeX + 110f, -y * itemSlotCellSizeY - 20f);
                UnityEngine.UI.Image image = itemSlotRectTransform.Find("icon").gameObject.GetComponent<UnityEngine.UI.Image>();

                if (item.GetSprite() != null) {  //Default empty sprite if not set
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
                handler.inventory = lootInventory;

                if (item.amount > 1 && item.IsStackable()) {
                    if (remaining > item.stackLimit) {
                        uiCount.SetText(item.stackLimit.ToString());
                        countShown += item.stackLimit;
                    } else {
                        uiCount.SetText(remaining.ToString());
                        countShown += remaining;
                    }
                } else {
                    countShown++;
                    uiCount.SetText("");
                }


                x++;
                if (x > 6) {
                    x = 0;
                    y++;
                }
            } while (countShown < count);

        }
    }



    private void RefreshCharInventories() {
        int charCount = GameManager.GM.playerCharacters.Count;

        if (charCount>=1) {
            RefreshChar1Inventory(GameManager.GM.playerCharacters[0]);
        }
        if (charCount>=2) {
            RefreshChar2Inventory(GameManager.GM.playerCharacters[1]);
        }
        if (charCount>=3) {
            RefreshChar3Inventory(GameManager.GM.playerCharacters[2]);
        }
        if (charCount>=4) {
            RefreshChar4Inventory(GameManager.GM.playerCharacters[3]);
        }
    }

    private void RefreshChar1Inventory(CharacterSheet character) {
        foreach (Transform child in Inv1Container) {
            if (child != itemSmallSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        CharName1.GetComponent<TextMeshProUGUI>().SetText(character.characterName);

        string slotLimText = string.Format(character.inventory.GetSlotsUsed() + " / " + character.inventory.GetSlotsLimit() + " Slots");
        SlotLimit1.GetComponent<TextMeshProUGUI>().SetText(slotLimText);

        int x = 0;
        int y = 0;
        float itemSlotCellSizeY = 90f;
        float itemSlotCellSizeX = 90f;
        foreach (Item item in character.inventory.GetItemList()) {
            int count = item.amount;
            int countShown = 0;

            do {
                GameObject itemSlotNew = Instantiate(itemSmallSlotTemplate, Inv1Container);
                itemSlotHandler handler = itemSlotNew.GetComponentInChildren<itemSlotHandler>();
                RectTransform itemSlotRectTransform = itemSlotNew.GetComponent<RectTransform>();
                itemSlotRectTransform.gameObject.SetActive(true);
                itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSizeX + 10f, -y * itemSlotCellSizeY - 10f);
                UnityEngine.UI.Image image = itemSlotRectTransform.Find("icon").gameObject.GetComponent<UnityEngine.UI.Image>();

                if (item.GetSprite() != null) {  //Default empty sprite if not set
                    image.sprite = item.GetSprite();
                } else {
                    image.color = Color.white;
                }

                TextMeshProUGUI uiCount = itemSlotRectTransform.Find("count").GetComponent<TextMeshProUGUI>();
                int remaining = count - countShown;
                //Debug.Log("Name: " + item.itemName + " Stackable: " + item.IsStackable().ToString()+ " StackLimit: " + item.stackLimit +  "\n  amount: " + item.amount+ " remaining: " + remaining);

                handler.item = item;
                handler.inventory = lootInventory;

                if (item.amount > 1 && item.IsStackable()) {
                    if (remaining > item.stackLimit) {
                        uiCount.SetText(item.stackLimit.ToString());
                        countShown += item.stackLimit;
                    } else {
                        uiCount.SetText(remaining.ToString());
                        countShown += remaining;
                    }
                } else {
                    countShown++;
                    uiCount.SetText("");
                }


                x++;
                if (x > 3) {
                    x = 0;
                    y++;
                }
            } while (countShown < count);

        }
    }

    private void RefreshChar2Inventory(CharacterSheet character) {
        foreach (Transform child in Inv2Container) {
            if (child != itemSmallSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        CharName2.GetComponent<TextMeshProUGUI>().SetText(character.characterName);

        string slotLimText = string.Format(character.inventory.GetSlotsUsed() + " / " + character.inventory.GetSlotsLimit() + " Slots");
        SlotLimit2.GetComponent<TextMeshProUGUI>().SetText(slotLimText);

        int x = 0;
        int y = 0;
        float itemSlotCellSizeY = 90f;
        float itemSlotCellSizeX = 90f;
        foreach (Item item in character.inventory.GetItemList()) {
            int count = item.amount;
            int countShown = 0;

            do {
                GameObject itemSlotNew = Instantiate(itemSmallSlotTemplate, Inv2Container);
                itemSlotHandler handler = itemSlotNew.GetComponentInChildren<itemSlotHandler>();
                RectTransform itemSlotRectTransform = itemSlotNew.GetComponent<RectTransform>();
                itemSlotRectTransform.gameObject.SetActive(true);
                itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSizeX + 10f, -y * itemSlotCellSizeY - 10f);
                UnityEngine.UI.Image image = itemSlotRectTransform.Find("icon").gameObject.GetComponent<UnityEngine.UI.Image>();

                if (item.GetSprite() != null) {  //Default empty sprite if not set
                    image.sprite = item.GetSprite();
                } else {
                    image.color = Color.white;
                }

                TextMeshProUGUI uiCount = itemSlotRectTransform.Find("count").GetComponent<TextMeshProUGUI>();
                int remaining = count - countShown;
                //Debug.Log("Name: " + item.itemName + " Stackable: " + item.IsStackable().ToString()+ " StackLimit: " + item.stackLimit +  "\n  amount: " + item.amount+ " remaining: " + remaining);

                handler.item = item;
                handler.inventory = lootInventory;

                if (item.amount > 1 && item.IsStackable()) {
                    if (remaining > item.stackLimit) {
                        uiCount.SetText(item.stackLimit.ToString());
                        countShown += item.stackLimit;
                    } else {
                        uiCount.SetText(remaining.ToString());
                        countShown += remaining;
                    }
                } else {
                    countShown++;
                    uiCount.SetText("");
                }


                x++;
                if (x > 2) {
                    x = 0;
                    y++;
                }
            } while (countShown < count);

        }
    }

    private void RefreshChar3Inventory(CharacterSheet character) {
        foreach (Transform child in Inv3Container) {
            if (child != itemSmallSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        CharName3.GetComponent<TextMeshProUGUI>().SetText(character.characterName);

        string slotLimText = string.Format(character.inventory.GetSlotsUsed() + " / " + character.inventory.GetSlotsLimit() + " Slots");
        SlotLimit3.GetComponent<TextMeshProUGUI>().SetText(slotLimText);

        int x = 0;
        int y = 0;
        float itemSlotCellSizeY = 90f;
        float itemSlotCellSizeX = 90f;
        foreach (Item item in character.inventory.GetItemList()) {
            int count = item.amount;
            int countShown = 0;

            do {
                GameObject itemSlotNew = Instantiate(itemSmallSlotTemplate, Inv3Container);
                itemSlotHandler handler = itemSlotNew.GetComponentInChildren<itemSlotHandler>();
                RectTransform itemSlotRectTransform = itemSlotNew.GetComponent<RectTransform>();
                itemSlotRectTransform.gameObject.SetActive(true);
                itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSizeX + 10f, -y * itemSlotCellSizeY - 10f);
                UnityEngine.UI.Image image = itemSlotRectTransform.Find("icon").gameObject.GetComponent<UnityEngine.UI.Image>();

                if (item.GetSprite() != null) {  //Default empty sprite if not set
                    image.sprite = item.GetSprite();
                } else {
                    image.color = Color.white;
                }

                TextMeshProUGUI uiCount = itemSlotRectTransform.Find("count").GetComponent<TextMeshProUGUI>();
                int remaining = count - countShown;
                //Debug.Log("Name: " + item.itemName + " Stackable: " + item.IsStackable().ToString()+ " StackLimit: " + item.stackLimit +  "\n  amount: " + item.amount+ " remaining: " + remaining);

                handler.item = item;
                handler.inventory = lootInventory;

                if (item.amount > 1 && item.IsStackable()) {
                    if (remaining > item.stackLimit) {
                        uiCount.SetText(item.stackLimit.ToString());
                        countShown += item.stackLimit;
                    } else {
                        uiCount.SetText(remaining.ToString());
                        countShown += remaining;
                    }
                } else {
                    countShown++;
                    uiCount.SetText("");
                }


                x++;
                if (x > 2) {
                    x = 0;
                    y++;
                }
            } while (countShown < count);

        }
    }

    private void RefreshChar4Inventory(CharacterSheet character) {
        foreach (Transform child in Inv4Container) {
            if (child != itemSmallSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        CharName4.GetComponent<TextMeshProUGUI>().SetText(character.characterName);

        string slotLimText = string.Format(character.inventory.GetSlotsUsed() + " / " + character.inventory.GetSlotsLimit() + " Slots");
        SlotLimit4.GetComponent<TextMeshProUGUI>().SetText(slotLimText);

        int x = 0;
        int y = 0;
        float itemSlotCellSizeY = 90f;
        float itemSlotCellSizeX = 90f;
        foreach (Item item in character.inventory.GetItemList()) {
            int count = item.amount;
            int countShown = 0;

            do {
                GameObject itemSlotNew = Instantiate(itemSmallSlotTemplate, Inv4Container);
                itemSlotHandler handler = itemSlotNew.GetComponentInChildren<itemSlotHandler>();
                RectTransform itemSlotRectTransform = itemSlotNew.GetComponent<RectTransform>();
                itemSlotRectTransform.gameObject.SetActive(true);
                itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSizeX + 10f, -y * itemSlotCellSizeY - 10f);
                UnityEngine.UI.Image image = itemSlotRectTransform.Find("icon").gameObject.GetComponent<UnityEngine.UI.Image>();

                if (item.GetSprite() != null) {  //Default empty sprite if not set
                    image.sprite = item.GetSprite();
                } else {
                    image.color = Color.white;
                }

                TextMeshProUGUI uiCount = itemSlotRectTransform.Find("count").GetComponent<TextMeshProUGUI>();
                int remaining = count - countShown;
                //Debug.Log("Name: " + item.itemName + " Stackable: " + item.IsStackable().ToString()+ " StackLimit: " + item.stackLimit +  "\n  amount: " + item.amount+ " remaining: " + remaining);

                handler.item = item;
                handler.inventory = lootInventory;

                if (item.amount > 1 && item.IsStackable()) {
                    if (remaining > item.stackLimit) {
                        uiCount.SetText(item.stackLimit.ToString());
                        countShown += item.stackLimit;
                    } else {
                        uiCount.SetText(remaining.ToString());
                        countShown += remaining;
                    }
                } else {
                    countShown++;
                    uiCount.SetText("");
                }


                x++;
                if (x > 2) {
                    x = 0;
                    y++;
                }
            } while (countShown < count);

        }
    }

    public void SetInventory(Inventory inventory) {
        if (this.lootInventory != null)
            this.lootInventory.OnItemListChanged -= LootInventory_OnItemListChanged;

        this.lootInventory = inventory;

        inventory.OnItemListChanged += LootInventory_OnItemListChanged;

        RefreshLootInventoryItems();
        RefreshCharInventories();
        Debug.Log("SetInv");

    }
    

    public void OpenLootUI() {
        gameObject.SetActive(true);
        RefreshLootInventoryItems();
        RefreshCharInventories();
    }
    public void CloseLootUI() {
        gameObject.SetActive(false);

        if (UI_ContextMenu.UI_CONTEXTMENU != null) {
            UI_ContextMenu.UI_CONTEXTMENU.HideContextMenu();
        }
    }
}
