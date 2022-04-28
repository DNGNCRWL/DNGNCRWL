using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UI_ChestDisplay : MonoBehaviour
{
    public static UI_ChestDisplay UI_CHESTDISPLAY;
    [SerializeField]
    private Chest chestInventory;

    public GameObject UI_testDisplay;

    private List<Item> chestItemsList;
    public GameObject itemSlotTemplate;
    public GameObject itemSmallSlotTemplate;
    private Transform LootContainer;
    private Transform CharName1;


    private void Awake() {
        if (UI_CHESTDISPLAY == null) {
            UI_CHESTDISPLAY= this;
        } else {
            Debug.Log("destroying!");
            Destroy(gameObject);
        }

        chestItemsList = FindObjectOfType<Chest>().getChestItems();
        LootContainer = transform.Find("LootContainer");
        CharName1 = transform.Find("CharName1");

        //CloseChestDisplayUI();
        //Debug.Log(itemSlotContainer);
    }

    void Update()
    {
        if(gameObject.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.F)){
                CloseChestDisplayUI();
            }
        }
    }
    private void RefreshChestInventoryItems () {
        Debug.Log("inside refresh");
        foreach (Transform child in LootContainer) {
            if (child.name == "bg") continue;
            Destroy(child.gameObject);
        }

        int x = 0;
        int y = 0;
        float itemSlotCellSizeY = 90f;
        float itemSlotCellSizeX = 250f;
        
        foreach (Item item in chestItemsList) {
            Debug.Log("IN ITEMS!!!", item);
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
                //handler.inventory = chestInventory;

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
    public void OpenChestDisplayUI() {
        Debug.Log("OKAY BOSS OPENING UP!");
        // UI_testDisplay.SetActive(true);
        
        //UI_testDisplay = GameObject.FindGameObjectWithTag("Chest");
        //UI_testDisplay.SetActive(true);
        gameObject.SetActive(true);
        Debug.Log(gameObject);
        RefreshChestInventoryItems();
        //RefreshCharInventories();

    }

    public void CloseChestDisplayUI() {
        gameObject.SetActive(false);
        if (UI_ContextMenu.UI_CONTEXTMENU != null) {
            UI_ContextMenu.UI_CONTEXTMENU.HideContextMenu();
        }
    }    
}
