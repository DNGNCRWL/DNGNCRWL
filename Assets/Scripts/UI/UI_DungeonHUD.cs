using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DungeonHUD : MonoBehaviour
{

    public static UI_DungeonHUD UI_DUNGEONHUD;

    private Transform partyBtn;

    private void Awake() {
        if (UI_DUNGEONHUD == null) {
            UI_DUNGEONHUD = this;
        } else {
            Destroy(gameObject);
        }

        partyBtn = transform.Find("PartyButton");
    }

    public void OpenPartyMenu() {
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

        Debug.Log("Party Menu HUD Button Clicked");
        if (UI_PartyMenu.UI_PARTYMENU != null)
            UI_PartyMenu.UI_PARTYMENU.OpenPartyUI();
    }
}
