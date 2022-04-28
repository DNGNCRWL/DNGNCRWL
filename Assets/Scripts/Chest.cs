using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; 
public class Chest : MonoBehaviour
{
    public Item[] Items = new Item[6];


    public IDictionary<Vector3, Inventory> allChests = new Dictionary<Vector3, Inventory>();
    public static Chest CHEST;
    private bool opened;

    //public GameObject chestUI;
    public List<Item> toFill = new List<Item>();


    //private Transform itemSlotContainer;
    //public GameObject itemSlotTemplate;


    private void Awake(){
        //itemSlotContainer = transform.Find("itemSlotContainer");
    }
    public void fillChest(){
        Inventory chestInventory = new Inventory();
        Debug.Log("inv count " + chestInventory.itemList.Count);
        if(!allChests.ContainsKey(Navigation.INSTANCE.transform.position)){
            int numItems = Random.Range(1,10);
            for(int i=0; i < numItems; i++)
            {
                toFill.Add(ItemManager.RANDOM_ITEM(Items));
                chestInventory.AddItem(ItemManager.RANDOM_ITEM(Items));
                Debug.Log("FILLING CHEST!!" + toFill[i]);
            }
            opened = true;
            allChests.Add(Navigation.INSTANCE.transform.position, chestInventory);
            Debug.Log(allChests.Count);
        }else{
            Debug.Log("already got!");
        }
    }

    public void OpenChestUI()
    {
        FindObjectOfType<UI_Chest>().OpenChestUI();
    }

    public List<Item> getChestItems(){
        return toFill;
    }
    public Inventory getChestInventory(Vector3 loc){
        Debug.Log(loc);
        Debug.Log("chest at loc !" + allChests[Navigation.INSTANCE.transform.position]);
        return allChests[loc];
    }
}
