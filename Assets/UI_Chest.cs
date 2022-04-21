using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Chest : MonoBehaviour
{

    public static UI_Chest UI_CHEST;

    private Transform charSlotContainer;

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

        charSlotContainer = transform.Find("CharSlotContainer");
        CloseChestUI();
        //Debug.Log("UI_PartyMenuAwake");
    }
    private void Start() {
        
    }

    public void OpenChestUI()
    {
        // Debug.Log("setting Active!");
        gameObject.SetActive(true);
        shownState = true;
        //Debug.Log("Open Party Menu");
    }
    public void CloseChestUI(){
        gameObject.SetActive(false);
        shownState = false;
    }
}
