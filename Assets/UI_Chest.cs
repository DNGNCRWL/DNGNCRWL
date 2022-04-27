using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Chest : MonoBehaviour
{

    public static UI_Chest UI_CHEST;

    private Transform itemSlotContainer;
    public GameObject itemSlotTemplate;
    private List<Item> chestItems = new List<Item>();

    private bool shownState = false;

    private void Awake()
    {
        Debug.Log("CHEST WOKE");
        if (UI_CHEST == null)
        {
            UI_CHEST = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start() {
        
    }

    public void OpenChestUI()
    {
        gameObject.SetActive(true);
        shownState = true;
    }
    public void CloseChestUI(){
        gameObject.SetActive(false);
        shownState = false;
    }

}
