using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    public GameObject itemSlotTemplate;

    void Awake() {
        itemSlotContainer = transform.Find("itemSlotContainer");
    }

    public void SetInventory (Inventory inventory) {
        this.inventory = inventory;

        inventory.OnItemListChanged += Inventory_OnItemListChanged;

        RefreshInventoryItems();
        //Debug.Log("SetInv");

    }

    private void Inventory_OnItemListChanged (object sender, System.EventArgs e) {
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
                RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
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
                Debug.Log("Name: " + item.itemName + " Stackable: " + item.IsStackable().ToString()+ " StackLimit: " + item.stackLimit +  "\n  amount: " + item.amount+ " remaining: " + remaining);

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
    }

    public List<Item> GetItemList() {
        return inventory.GetItemList();
    }

}
