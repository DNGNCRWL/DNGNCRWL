using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Item[] Items = new Item[6];

    public static Chest CHEST;

    public GameObject chestUI;
    public List<Item> toFill = new List<Item>();
    private Transform itemSlotContainer;
    public GameObject itemSlotTemplate;


    private void Awake(){
        itemSlotContainer = transform.Find("itemSlotContainer");
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

    }

    public void OpenChestUI()
    {
        FindObjectOfType<UI_Chest>().OpenChestUI();
    }

    public List<Item> getChestItems(){
        return toFill;
    }
}
