using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Item[] Items = new Item[6];

    public static Chest CHEST;

    public GameObject chestUI;
    private List<Item> toFill = new List<Item>();
    private Transform itemSlotContainer;
    public GameObject itemSlotTemplate;


    private void Awake(){
        // if (UI_INVENTORY == null)
        // {
        //     UI_INVENTORY = this;
        // } else {
        //     Destroy(gameObject);
        // }

        itemSlotContainer = transform.Find("itemSlotContainer");
        // armorSlotContainer = transform.Find("CharacterContainer").Find("armorSlotContainer");
        // bagSlotContainer = transform.Find("CharacterContainer").Find("bagSlotContainer");
        // mainhandSlotContainer = transform.Find("CharacterContainer").Find("mainhandSlotContainer");
        // offhandSlotContainer = transform.Find("CharacterContainer").Find("offhandSlotContainer");
        // charName = transform.Find("CharacterContainer").Find("charName");
        // slotLimit = transform.Find("CharacterContainer").Find("slotLimit");
        // CloseInventoryUI();
    }
    public void fillChest(){
        int numItems = Random.Range(0,10);
        Debug.Log("ITEMS??" + Items[0]);
        Debug.Log("GENERATING " + numItems + " ITEMS");
        for(int i=0; i < numItems; i++)
        {
            toFill.Add(ItemManager.RANDOM_ITEM(Items));
            Debug.Log("FILLING CHEST!!" + toFill[i]);
        }
        OpenChestUI();
    }

    public void OpenChestUI()
    {
        FindObjectOfType<UI_Chest>().OpenChestUI();
    }
}
