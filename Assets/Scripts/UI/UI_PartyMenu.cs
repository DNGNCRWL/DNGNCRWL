using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PartyMenu : MonoBehaviour
{
    public static UI_PartyMenu UI_PARTYMENU;
    [SerializeField] public List<CharacterSheet> party;


    [SerializeField]
    private Transform charSlotContainer;
    public GameObject charSlotTemplate;
    //private bool shownState = true;


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

    public void SetParty(List<CharacterSheet> party)
    {
        this.party = party;

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

        List<CharacterSheet> deadCharList = new List<CharacterSheet>();
        foreach (CharacterSheet character in party) {
            if (character.IsDead()) {
                deadCharList.Add(character);
            }
        }
        
        foreach (CharacterSheet character in deadCharList) {
            if (character.IsDead()) {
                party.Remove(character);
                party.Add(character);
            }
        }

        int x = 0;
        //int y = 0;
        //float charSlotCellSizeY = 90f;
        float charSlotCellSizeX = 450f;
        float charSlotCellBufferX = 15f;
        foreach (CharacterSheet character in party)
        {
            Debug.Log(character);
            RectTransform charSlotRectTransform = Instantiate(charSlotTemplate, charSlotContainer).GetComponent<RectTransform>();
            charSlotRectTransform.gameObject.SetActive(true);
            charSlotRectTransform.anchoredPosition = new Vector2(x * charSlotCellSizeX + x*charSlotCellBufferX, 0);

            bool dead = false;

            if(character.IsDead()) dead = true;

            TextMeshProUGUI nameUI = charSlotRectTransform.Find("name").GetComponent<TextMeshProUGUI>();
            string nameText = character.characterName;
            if (dead) nameText += "(Dead)";
            nameUI.SetText(nameText);

            if (!dead) {
                Button invButton = charSlotRectTransform.Find("inventoryButton").GetComponent<Button>();
                invButton.onClick.AddListener(delegate { OpenCharInv(character); });


                Button moveFrontButton = charSlotRectTransform.Find("moveFrontButton").GetComponent<Button>();
                moveFrontButton.onClick.AddListener(delegate { MoveCharToFront(character); });
            } else {
                charSlotRectTransform.Find("inventoryButton").gameObject.SetActive(false);
                charSlotRectTransform.Find("moveFrontButton").gameObject.SetActive(false);
            }

            string InfoText = "";
            InfoText += "Class: " + character.GetCharacterClass();
            InfoText += "\nLevel: " + character.GetLevel();
            InfoText += "\nHealth: " + character.GetHitPoints().ToString()+  " / "+ character.GetMaxHitPoints().ToString();
            InfoText += "\nStrength: " + character.GetStrength().ToString();
            InfoText += "\nAgility: " + character.GetAgility().ToString();
            InfoText += "\nPresence: " + character.GetPresence().ToString();
            InfoText += "\nToughness: " + character.GetToughness().ToString();
            InfoText += "\nOmens: " + character.GetOmens().ToString();

            TextMeshProUGUI infoUI = charSlotRectTransform.Find("text").GetComponent<TextMeshProUGUI>();
            infoUI.SetText(InfoText);

            Image charImage = charSlotRectTransform.Find("image").gameObject.GetComponent<Image>();
            charImage.sprite = character.GetSprite();
            charImage.preserveAspect = true;
            if (dead) charImage.color = Color.red;

            Image progBar = charSlotRectTransform.Find("xpBar").Find("Fill").gameObject.GetComponent<Image>();
            progBar.fillAmount = character.GetExperience() / character.CalcExperienceForNextLevel();

            TextMeshProUGUI progBarXP = charSlotRectTransform.Find("xpBar").Find("text").gameObject.GetComponent<TextMeshProUGUI>();
            progBarXP.SetText("XP: " + character.GetExperience() + " / " + character.CalcExperienceForNextLevel());


            x++;
        }
    }

    private void OpenCharInv (CharacterSheet character) {
        //Debug.Log(character);
        if (UI_Inventory.UI_INVENTORY == null) return;

        UI_Inventory.UI_INVENTORY.SetCharacterTarget(character);
        UI_Inventory.UI_INVENTORY.OpenInventoryUI();
    }

    private void MoveCharToFront(CharacterSheet character) {
        //Debug.Log(character);
        GameManager.GM.MoveCharToFront(character);
        RefreshPartyList();
    }

    public void OpenPartyUI()
    {
        if (party == null)
            party = GameManager.GM.playerCharacters;
        gameObject.SetActive(true);
        RefreshPartyList();
        //shownState = true;
        //Debug.Log("Open Party Menu");
    }
    public void ClosePartyUI()
    {
        gameObject.SetActive(false);
        //shownState = false;
        //Debug.Log("Close Party Menu");
    }
}
