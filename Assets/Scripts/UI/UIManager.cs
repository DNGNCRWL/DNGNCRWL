using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        if (UI_PartyMenu.UI_PARTYMENU == null) {
            UI_PartyMenu partyMenu = null;
            var canvases = Resources.FindObjectsOfTypeAll<UI_PartyMenu>();
            if (canvases.Length > 0)
                partyMenu = canvases[0];

            if(partyMenu != null)
                partyMenu.OpenPartyUI();
        }
        
        if (UI_Inventory.UI_INVENTORY == null) {
            UI_Inventory inventoryMenu = null;
            var canvases = Resources.FindObjectsOfTypeAll<UI_Inventory>();
            if (canvases.Length > 0)
                inventoryMenu = canvases[0];

            if (inventoryMenu != null)
                inventoryMenu.OpenInventoryUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && UI_PartyMenu.UI_PARTYMENU != null)
        {
            Debug.Log("Party Menu Key Pressed");
            UI_PartyMenu.UI_PARTYMENU.OpenPartyUI();
        }
    }
}
