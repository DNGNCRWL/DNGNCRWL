using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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

            if (partyMenu != null) {
                partyMenu.SetParty(GameManager.GM.playerCharacters);
                partyMenu.OpenPartyUI();
            }
        }
        
        if (UI_Inventory.UI_INVENTORY == null) {
            UI_Inventory inventoryMenu = null;
            var canvases = Resources.FindObjectsOfTypeAll<UI_Inventory>();
            if (canvases.Length > 0)
                inventoryMenu = canvases[0];

            if (inventoryMenu != null)
                inventoryMenu.OpenInventoryUI();
        }
        
        if (UI_LootMenu.UI_LOOTMENU == null) {
            UI_LootMenu lootMenu = null;
            var canvases = Resources.FindObjectsOfTypeAll<UI_LootMenu>();
            if (canvases.Length > 0)
                lootMenu = canvases[0];

            if (lootMenu != null)
                lootMenu.OpenLootUI();
        }
        Debug.Log("UI Manager start");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (UI_LootMenu.UI_LOOTMENU == null) {
                UI_LootMenu lootMenu = null;
                var canvases = Resources.FindObjectsOfTypeAll<UI_LootMenu>();
                if (canvases.Length > 0)
                    lootMenu = canvases[0];

                if (lootMenu != null)
                    lootMenu.OpenLootUI();
            }

            Debug.Log("Loot Menu Key Pressed");
            if (UI_LootMenu.UI_LOOTMENU != null)
                UI_LootMenu.UI_LOOTMENU.OpenLootUI();
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (UI_PartyMenu.UI_PARTYMENU == null) {
                UI_PartyMenu partyMenu = null;
                var canvases = Resources.FindObjectsOfTypeAll<UI_PartyMenu>();
                if (canvases.Length > 0)
                    partyMenu = canvases[0];

                if (partyMenu != null) {
                    partyMenu.SetParty(GameManager.GM.playerCharacters);
                    partyMenu.OpenPartyUI();
                }
            }

            Debug.Log("Party Menu Key Pressed");
            if (UI_PartyMenu.UI_PARTYMENU != null)
                UI_PartyMenu.UI_PARTYMENU.OpenPartyUI();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Exit Key Pressed");
            SceneManager.LoadScene("Town", LoadSceneMode.Additive);
        }
    }
}
