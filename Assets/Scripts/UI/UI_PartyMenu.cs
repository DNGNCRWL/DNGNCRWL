using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PartyMenu : MonoBehaviour
{
    public static UI_PartyMenu UI_PARTYMENU;
    [SerializeField] public Party party;


    [SerializeField]
    private Transform charSlotContainer;
    public GameObject charSlotTemplate;
    private bool shownState = true;


    private void Awake()
    {
        if (UI_PARTYMENU == null)
        {
            UI_PARTYMENU = this;
        }
        else
        {
            Destroy(gameObject);
        }

        charSlotContainer = transform.Find("CharSlotContainer");
        ClosePartyUI();
        //Debug.Log("UI_PartyMenuAwake");
    }
    private void Start() {

        if (UI_Inventory.UI_INVENTORY == null) {
            UI_Inventory inventoryMenu = null;
            var canvases = Resources.FindObjectsOfTypeAll<UI_Inventory>();
            if (canvases.Length > 0)
                inventoryMenu = canvases[0];

            if (inventoryMenu != null)
                inventoryMenu.OpenInventoryUI();
        }

        RefreshPartyList();
    }

    private void Update() {
    }

    public void SetParty(Party party)
    {
        this.party = party;

        party.OnPartyListChanged += Party_OnPartyListChanged;

        RefreshPartyList();
        //Debug.Log("SetParty");

    }

    private void Party_OnPartyListChanged(object sender, System.EventArgs e)
    {
        RefreshPartyList();
    }

    private void RefreshPartyList() {
        foreach (Transform child in charSlotContainer)
        {
            if (child == charSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        int x = 0;
        //int y = 0;
        //float charSlotCellSizeY = 90f;
        float charSlotCellSizeX = 300f;
        float charSlotCellBufferX = 10f;
        foreach (CharacterSheet character in party.characters)
        {
            Debug.Log(character);
            RectTransform charSlotRectTransform = Instantiate(charSlotTemplate, charSlotContainer).GetComponent<RectTransform>();
            charSlotRectTransform.gameObject.SetActive(true);
            charSlotRectTransform.anchoredPosition = new Vector2(x * charSlotCellSizeX + x*charSlotCellBufferX, 0);

            TextMeshProUGUI nameUI = charSlotRectTransform.Find("name").GetComponent<TextMeshProUGUI>();
            nameUI.SetText(character.characterName);

            Button invButton = charSlotRectTransform.Find("inventoryButton").GetComponent<Button>();
            invButton.onClick.AddListener(delegate { OpenCharInv(character); });

            string InfoText = "";
            InfoText += "Class: " + character.GetCharacterClass();
            InfoText += "\nHealth: " + character.GetHitPoints().ToString()+  " / "+ character.GetMaxHitPoints().ToString();
            InfoText += "\nStrength: " + character.GetStrength().ToString();
            InfoText += "\nAgility: " + character.GetAgility().ToString();
            InfoText += "\nPresence: " + character.GetPresence().ToString();
            InfoText += "\nToughness: " + character.GetToughness().ToString();
            InfoText += "\nOmens: " + character.GetOmens().ToString();

            TextMeshProUGUI infoUI = charSlotRectTransform.Find("text").GetComponent<TextMeshProUGUI>();
            infoUI.SetText(InfoText);

            x++;
        }
    }

    private void OpenCharInv (CharacterSheet character) {
        Debug.Log(character);
        if (UI_Inventory.UI_INVENTORY == null) return;

        UI_Inventory.UI_INVENTORY.SetCharacterTarget(character);
        UI_Inventory.UI_INVENTORY.OpenInventoryUI();
    }

    public void OpenPartyUI()
    {
        gameObject.SetActive(true);
        RefreshPartyList();
        shownState = true;
        //Debug.Log("Open Party Menu");
    }
    public void ClosePartyUI()
    {
        gameObject.SetActive(false);
        shownState = false;
        //Debug.Log("Close Party Menu");
    }
}
