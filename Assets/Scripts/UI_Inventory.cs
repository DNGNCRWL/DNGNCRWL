using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    public GameObject itemSlotTemplate;

    void Awake() {
        itemSlotContainer = transform.Find("itemSlotContainer");
       // itemSlotTemplate = ;
    }

    public void SetInventory (Inventory inventory) {
        this.inventory = inventory;

        inventory.OnItemListChanged += Inventory_OnItemListChanged;

        RefreshInventoryItems();
        Debug.Log("SetInv");

    }

    private void Inventory_OnItemListChanged (object sender, System.EventArgs e) {
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems() {
        Debug.Log("Refresh");
        foreach (Transform child in itemSlotContainer){
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }
        int x = 0;
        int y = 0;
        float itemSlotCellSize = 90f;
        foreach(Item item in inventory.GetItemList()) {
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            // GameObject newItemSlot = Instantiate(itemSlotTemplate, itemSlotContainer);
            // RectTransform itemSlotRectTransform = newItemSlot.GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);
            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, -y * itemSlotCellSize);
            UnityEngine.UI.Image image = itemSlotRectTransform.Find("icon").gameObject.GetComponent<UnityEngine.UI.Image>();
            Debug.Log(item.GetSprite());
            image.sprite = item.GetSprite();

            x++;
            if (x>4) {
                x=0;
                y++;
            }

        }
    }

    public List<Item> GetItemList() {
        return inventory.GetItemList();
    }

}
