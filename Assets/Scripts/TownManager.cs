using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TownManager : MonoBehaviour
{
    public Transform TMTransform;
    public GameManager GM;
    public Transform GMTransform;
    public GameObject characterPrefab;

    public List<Image> clickableImages;

    //Player Characters
    public List<CharacterSheet> playerCharacters;
    public List<CharacterSheet> reserveCharacters;
    public List<CharacterSheet> deadCharacters;
    public List<CharacterSheet> injuredCharsInParty;

    //Recruitable Characters
    public List<CharacterSheet> recruitableCharacters;

    //Rest Menu
    public List<GameObject> restTiles;
    public List<GameObject> resurrectTiles;

    //Swap Party Menu ------------------------------------------------------------
    //Current Party Character Names
    public List<GameObject> charMenuTiles;
    public List<GameObject> reserveCharMenuTiles;
    int pageNumber = 0;
    public GameObject incrementPageButton;
    public GameObject decrementPageButton;

    //Recruit Character Menu -----------------------------------------------------
    //Recruitable Character Tiles
    public List<GameObject> recruitMenuTiles;


    //Store Menu -----------------------------------------------------------------
    public List<Item> storeItems;
    public List<GameObject> storeTiles;
    public List<GameObject> buyingTiles;
    public List<GameObject> playerItemTiles;
    public List<GameObject> sellingTiles;
    public GameObject itemInfo;
    public GameObject silverInfo;
    public GameObject buyForMenu;
    public int itemSelected;
    public List<int> previousLocations;
    public List<Item> cartItems;
    public List<Item> sellingItems;
    public List<CharacterSheet> charToBuyFor;
    public int cost = 0;
    public GameObject sellFromList;
    public List<Item> sellableItems;
    public CharacterSheet sellFromCharacter;
    public List<int> previousSellLocations;
    public List<CharacterSheet> charToSellFrom;

    public GameObject errorMessageWindow;
    public int silver;

    //
    //
    //---------------------------------------------------------------------------EXTRA METHODS--------------------------------------------------------------------------------------------
    //
    //

    //Set the GameManager references, generates recruitable characters and store items, and sets buttons alpha to non-clickable
    void Awake() {
        GM = GameManager.GM;
        GMTransform = GameManager.GM.transform;
        GenerateRandomChar();
        StoreGen();
        foreach (Image pic in clickableImages) {
            pic.alphaHitTestMinimumThreshold = 0.01f;
        }
    }

    // Sets character lists and all menu info, as well as checks for dead characters
    void Start()
    {
        playerCharacters = GM.playerCharacters;
        reserveCharacters = GM.reserveCharacters;
        foreach(CharacterSheet character in playerCharacters) {
            if (!character.GetCanBeHit()) {
                SetDead(character);
            }
        }
        silver = CalculateSilver();
        SetRecCharInfo();
        SetCharInfo();
        SetReserveCharInfo();
        SetStoreInfo();
        SetBuyForMenu();
        SelectSellFromCharacter(0);
        UpdateInjured();
        UpdateRezInfo();
    }

    //Changes whether an object is active or not
    public void ToggleActive(GameObject target) {
        target.SetActive(!target.activeSelf);
    }

    //Makes given object active
    public void ToggleOn(GameObject target) {
        target.SetActive(true);
        UpdateInjured();
        UpdateRezInfo();
    }

    //Makes given object not active
    public void ToggleOff(GameObject target) {
        target.SetActive(false);
    }

    //Makes given button not interactable
    public void TurnOffInteractable(Button target) {
        target.interactable = false;
    }

    //Makes given button interactable
    public void TurnOnInteractable(Button target) {
        target.interactable = true;
    }

    //Turns off object if parent is inactive (specific for character button usage)
    public void TurnOffIfLast(GameObject above) {
        if (!above.activeSelf) {
            above.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    //Pops up error message window with given message
    public void ErrorMessage(string message) {
        errorMessageWindow.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
        errorMessageWindow.SetActive(true);
    }

    //Ensures party is not empty and enters the rest of the dungeon
    public void EnterDungeon() {
        if (playerCharacters.Count > 0) {
            //FindObjectOfType<DungeonGenerator>().Start();
            SceneManager.LoadScene("DungeonGeneration", LoadSceneMode.Single);
            GameManager.PartySetActive(false);
        } else {
            ErrorMessage("Must have at least one character in party to enter the dungeon");
        }
    }

    //General method to set any character button in town menu
    private void SetCharacterButtons(List<GameObject> tiles, List<CharacterSheet> characters, bool reserveInfo, bool restInfo) {
        for (int i = 0; i < tiles.Count; ++i) {
            int index;
            if (reserveInfo) index = i + (pageNumber * 2);
            else index = i;
            if (reserveInfo && (i + (pageNumber * 2) < characters.Count) || !reserveInfo && (i < characters.Count)) {
                SetImage(tiles[i].transform.GetChild(0).gameObject, characters[i]);
                tiles[i].SetActive(true);
                if (restInfo) {
                GameObject infoTile = tiles[i].transform.GetChild(1).gameObject;
                infoTile.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = characters[i].GetCharacterName();
                infoTile.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Hit Points: " + characters[i].GetHitPoints().ToString();
                infoTile.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Max HP: " + characters[i].GetMaxHitPoints().ToString();
                } else if (!reserveInfo){
                    SetInfo(tiles[i].transform.GetChild(1).gameObject, characters[i]);
                } else {
                    SetImage(tiles[i].transform.GetChild(0).gameObject, characters[i + pageNumber * 2]);
                    SetInfo(tiles[i].transform.GetChild(1).gameObject, characters[i + pageNumber * 2]);
                }
            } else {
                tiles[i].SetActive(false);
            }
        } 
    }

    //
    //
    //---------------------------------------------------------------------------RESTING AND RESURRECTING METHODS----------------------------------------------------------------------------------
    //
    //

    //checks for injured characters in party
    private void UpdateInjured() {
        injuredCharsInParty = new List<CharacterSheet>();
        foreach (CharacterSheet character in playerCharacters) {
            if (character.GetHitPoints() < character.GetMaxHitPoints()) {
                injuredCharsInParty.Add(character);
            }
        }

        UpdateRestInfo();
    }

    //sets tile info for characters that can be rested
    private void UpdateRestInfo() {
        SetCharacterButtons(restTiles, injuredCharsInParty, false, true);
    }

    //Reset player health
    public void Rest(int target) {
        GetPayment(3);
        CharacterSheet character = injuredCharsInParty[target];
        character.RecoverDamage(new Damage(50, 20, 10, DamageType.Untyped));
        injuredCharsInParty.Remove(character);
        UpdateRestInfo();
    }

    //sets a given character to being dead (transfers them to dead list, and GameObject to townmanager to be removed on load)
    public void SetDead(CharacterSheet character) {
            if (playerCharacters.Remove(character)) {
                deadCharacters.Add(character);
                character.transform.parent = TMTransform;
                SetCharInfo();
                SetReserveCharInfo();
                SetRecCharInfo();
                SetBuyForMenu();
                SetStoreInfo();
                UpdateInjured();
                UpdateRezInfo();
                silver = CalculateSilver();
            }
    }

    //Updates tiles within the resurrect menu
    public void UpdateRezInfo() {
        SetCharacterButtons(resurrectTiles, deadCharacters, false, true);
    }

    //resurrects a character(transfers them to available spot in char list, and GameObject back to GM)
    public void ResurrectChar(int index) {
        GameObject character = deadCharacters[index].transform.gameObject;
        CharacterSheet charSheet = character.GetComponent<CharacterSheet>();
        GetPayment(100);
        character.transform.parent = GMTransform;
        deadCharacters.Remove(charSheet);
        if (playerCharacters.Count < 4) {
            playerCharacters.Add(charSheet);
            SelectSellFromCharacter(0);
        } else {
            reserveCharacters.Add(charSheet);
        }
        charSheet.RecoverDamage(new Damage(50, 20, 10, DamageType.Untyped));
        charSheet.ResetStateFromDead();
        silver = CalculateSilver();
        SetCharInfo();
        SetReserveCharInfo();
        SetRecCharInfo();
        SetBuyForMenu();
        SetStoreInfo();
        UpdateInjured();
        UpdateRezInfo();
    }

    
    //
    //
    //---------------------------------------------------------------------------PARTY CHANGE METHODS--------------------------------------------------------------------------------------------
    //
    //

    //Set info about characters in player party, if there are more spaces than characters deactivate unused spaces
    public void SetCharInfo() {
        //Set each tile with character info
        SetCharacterButtons(charMenuTiles, playerCharacters, false, false);
    }

    //Set info about characters in player party, if there are more spaces than characters deactivate unused spaces
    public void SetReserveCharInfo() {
        //if the current page is beyond the amount of reserve characters, go back a page
        if ((pageNumber + 1) * 2 > reserveCharacters.Count + 1 && pageNumber != 0) {
            --pageNumber;
        }

        //Check for decrement button
        if (pageNumber > 0) {
            decrementPageButton.SetActive(true);
        } else {
            decrementPageButton.SetActive(false);
        }

        //Check for increment button
        if ((pageNumber + 1) * 2 < reserveCharacters.Count) {
            incrementPageButton.SetActive(true);
        } else {
            incrementPageButton.SetActive(false);
        }

        //Set each tile with character info
        SetCharacterButtons(reserveCharMenuTiles, reserveCharacters, true, false);
    }

    //Set info about recruitable characters, if there are more spaces than characters deactivate unused spaces
    public void SetRecCharInfo() {
        SetCharacterButtons(recruitMenuTiles, recruitableCharacters, false, false);
    }

    //Sets the image of the button to the given character's sprite
    public void SetImage(GameObject button, CharacterSheet character) {
        Image buttonImage = button.GetComponent<Image>();
        buttonImage.sprite = character.GetSprite();
        buttonImage.SetNativeSize();
    }

    //Takes tile list, index of tile list, character list, and index of character list, and sets tile info to character info
    public void SetInfo(GameObject tile, CharacterSheet character) {
        tile.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = character.GetCharacterName();
        tile.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Max HP: " + character.GetMaxHitPoints().ToString();
        tile.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Strength: " + character.GetStrength().ToString();
        tile.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Agility: " + character.GetAgility().ToString();
        tile.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Presence: " + character.GetPresence().ToString();
        tile.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "Toughness: " + character.GetToughness().ToString();
    }
    

    //Swap a character from the recruitable character under TownManager to player characters under GameManager
    public void AddCharToPlayerChars(int index) {
        GameObject character = recruitableCharacters[index].transform.gameObject;
        CharacterSheet charSheet = character.GetComponent<CharacterSheet>();
        character.transform.parent = GMTransform;
        recruitableCharacters.Remove(charSheet);
        if (playerCharacters.Count < 4) {
            playerCharacters.Add(charSheet);
            SelectSellFromCharacter(0);
        } else {
            reserveCharacters.Add(charSheet);
        }
        silver = CalculateSilver();
        SetCharInfo();
        SetReserveCharInfo();
        SetRecCharInfo();
        SetBuyForMenu();
        SetStoreInfo();
        UpdateInjured();
    }

    //Swap the char at the index from the player party to reserve characters
    public void RemoveCharFromParty(int index) {
        CharacterSheet charSheet = playerCharacters[index];
        playerCharacters.Remove(charSheet);
        reserveCharacters.Add(charSheet);
        SetCharInfo();
        SetReserveCharInfo();
        SetBuyForMenu();
        SelectSellFromCharacter(0);
        UpdateInjured();
    }

    //Swap the char at the index from reserve characters to the player party
    public void AddCharToParty(int index) {
        if (playerCharacters.Count < 4) {
            CharacterSheet charSheet = reserveCharacters[index + (pageNumber * 2)];
            playerCharacters.Add(charSheet);
            reserveCharacters.Remove(charSheet);
            SetCharInfo();
            SetReserveCharInfo();
            SetBuyForMenu();
            SelectSellFromCharacter(0);
            UpdateInjured();
        } else {
            ErrorMessage("There can only be four characters in the party");
        }
    }

    //Generates random recruitable characters
    public void GenerateRandomChar() {
        for (int i = 0; i < 4; ++i) {
            GameObject temp = Instantiate(characterPrefab, TMTransform);
            recruitableCharacters.Add(temp.GetComponent<CharacterSheet>());
        }
        for (int i = 0; i < recruitableCharacters.Count; ++i) {
            recruitableCharacters[i].InitializeRandomClassless();
        }
        SetRecCharInfo();
    }

    //Increases page number and reloads reserve characters
    public void IncrementPageNumber() {
        ++pageNumber;
        SetReserveCharInfo();
    }

    //Decreases page number and reloads reserve characters
    public void DecrementPageNumber() {
        --pageNumber;
        SetReserveCharInfo();
    }

    //
    //
    //---------------------------------------------------------------------------STORE METHODS--------------------------------------------------------------------------------------------
    //
    //

    //Set info about both buyable items and items in the store cart within the store menu
    public void SetStoreInfo() {
        SetStoreTiles(storeTiles, storeItems, true);
        SetStoreTiles(buyingTiles, cartItems, false);
        SetStoreTiles(playerItemTiles, sellableItems, false);
        SetStoreTiles(sellingTiles, sellingItems, false);

        SetCharacterSellList();
        
        silverInfo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + silver;
        silverInfo.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "" + cost;
    }

    //Set info about both sellable items and items in the selling cart within the store menu
    public void SetStoreTiles(List<GameObject> tiles, List<Item> items, bool specific) {
        for (int i = 0; i < tiles.Count; ++i) {
            if (!specific && i < items.Count || specific && items[i] != null) {
                tiles[i].SetActive(true);
                tiles[i].GetComponent<Image>().sprite = items[i].GetSprite();
            } else {
                tiles[i].SetActive(false);
            }
            HideItemInfo();
        }
    }

    //Generates random items for the store to sell
    public void StoreGen() {
        for (int i = 0; i < storeTiles.Count; ++i) {
            int randomIndex = UnityEngine.Random.Range(0, 2);
            if (randomIndex == 0) {
                int randomList = UnityEngine.Random.Range(0, 6);
                Item temp = (ItemManager.RANDOM_ITEM(ItemManager.STARTING_WEAPONS).Copy());
                switch (randomList) {
                    case 0:
                        temp = (ItemManager.RANDOM_ITEM(ItemManager.BAGS).Copy());
                        break;
                    case 1:
                        temp = (ItemManager.RANDOM_ITEM(ItemManager.ADVENTURE_TOOLS).Copy());
                        break;
                    case 2:
                        temp = (ItemManager.RANDOM_ITEM(ItemManager.SPECIAL_ITEMS).Copy());
                        break;
                    case 3:
                        temp = (ItemManager.RANDOM_ITEM(ItemManager.STARTING_WEAPONS).Copy());
                        break;
                    case 4:
                        temp = (ItemManager.RANDOM_ITEM(ItemManager.STARTING_ARMORS).Copy());
                        break;
                    case 5:
                        temp = (ItemManager.RANDOM_ITEM(ItemManager.STARTING_ARMORS_LOWTIER).Copy());
                        break;
                }
                storeItems.Add(temp);
            } else {
                storeItems.Add(null);
            }
        }
    }

    //Set the item info for a buyable item
    public void SetBuyableInfo(GameObject button) {
        SetStoreItemInfo(button, storeTiles, storeItems, false, true);
    }

    //Set the item info for an item in the cart
    public void SetCartItemInfo(GameObject button) {
        SetStoreItemInfo(button, buyingTiles, cartItems, true, true);
    }

    //Set the item info for a sellable item
    public void SetSellableInfo(GameObject button) {
        SetStoreItemInfo(button, playerItemTiles, sellableItems, false, false);
    }

    //Set the item info for an item in the sell item cart
    public void SetItemToSellInfo(GameObject button) {
        SetStoreItemInfo(button, sellingTiles, sellingItems, true, false);
    }

    //Sets the item info for a given item from a given list
    private void SetStoreItemInfo(GameObject button, List<GameObject> tiles, List<Item> items, bool cart, bool buying) {
        int index = tiles.IndexOf(button);
        Item item = items[index];
        if (cart) DisplayItemInfo(item, index, buying);
        else DisplayItemInfo(item, -1, buying);
    }

    //Set item info in store menu display
    public void DisplayItemInfo(Item item, int cartNum, bool buying) {
        string type = "";
        string wielding = "";
        string extras = "";
        string actions = "";

        for (int i = 0; i < item.actions.Count; ++i) {
            actions += item.actions[i].actionName;
            if(i < item.actions.Count - 1) {
                actions += ", ";
            }
        }

        if (item.GetType() == typeof(Item)) {
            type = "Item";
        } else if (item.GetType() == typeof(Ammo)) {
            type = "Ammo";
        } else if (item.GetType() == typeof(Armor)) {
            Armor armor = (Armor) item;
            type = "Armor";
            extras = (armor.armorTier * 2).ToString() + " defense";
        } else if (item.GetType() == typeof(Bag)) {
            Bag bag = (Bag) item;
            type = "Bag";
            extras = bag.carryingCapacity.ToString() + " slots";
        } else if (item.GetType() == typeof(Consumable)) {
            type = "Consumable";
        } else if (item.GetType() == typeof(ProjectileWeapon)) {
            type = "Projectile Weapon";
        } else if (item.GetType() == typeof(Scroll)) {
            type = "Scroll";
        } else if (item.GetType() == typeof(Weapon)) {
            Weapon weapon = (Weapon) item;
            if (weapon.twoHanded) {
                wielding = "Two Handed";
            } else {
                wielding = "One Handed";
            }
            type = "Weapon";
            extras = (weapon.damage.dieCount * weapon.damage.dieSize).ToString() + " Max Damage";
        }

        itemInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemName;
        itemInfo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = wielding;
        itemInfo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = type;
        itemInfo.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = actions;
        itemInfo.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = extras;
        itemInfo.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = item.description;
        itemInfo.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = item.value + " silver";
        if (cartNum == -1) {
            itemInfo.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = "";
        } else if (buying) {
            itemInfo.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = "Buying for: " + charToBuyFor[cartNum].GetCharacterName();
        } else {
            itemInfo.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = "Selling from: " + charToSellFrom[cartNum].GetCharacterName();
        }

        itemInfo.SetActive(true);
    }

    //Hide the item info
    public void HideItemInfo() {
        itemInfo.SetActive(false);
    }

    //Calculate total silver for player (combined silver of all characters in both party and reserve)
    public int CalculateSilver() {
        int total = 0;
        foreach(CharacterSheet character in playerCharacters) {
            total += character.GetSilver();
        }
        foreach(CharacterSheet character in reserveCharacters) {
            total += character.GetSilver();
        }
        return total;
    }

    //Set the characters that you can buy for in the store to those in the party
    public void SetBuyForMenu() {
        for(int i = 0; i < 4; ++i) {
            GameObject temp = buyForMenu.transform.GetChild(4-i).gameObject;
            if (i < playerCharacters.Count) {
                temp.SetActive(true);
                temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerCharacters[i].GetCharacterName();
            } else {
                temp.SetActive(false);
            }
        }
    }

    //Open the menu to select what character to buy an item for
    public void OpenBuyForMenu(int itemNum) {
        if (playerCharacters.Count > 1) {
            itemSelected = itemNum;
            buyForMenu.transform.position = Input.mousePosition;
            buyForMenu.transform.position = Input.mousePosition + GetGUIElementOffset(buyForMenu.transform.GetChild(5-playerCharacters.Count).GetComponent<RectTransform>());
            buyForMenu.SetActive(true);
        } else if (playerCharacters.Count == 1) {
            itemSelected = itemNum;
            AddItemToCart(0);
        } else {
            ErrorMessage("Must have characters in the party to buy for");
        }
    }

    //Returns offset required to ensure buyFor menu is fully on the screen
    public static Vector3 GetGUIElementOffset(RectTransform rect) {
        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
        Vector3[] objectCorners = new Vector3[4];
        rect.GetWorldCorners(objectCorners);
 
        Vector3 offset = new Vector3(0, 0, 0);
 
        for (int i = 0; i < objectCorners.Length; i++) {
            if (objectCorners[i].x < screenBounds.xMin) {
                offset.x = screenBounds.xMin - objectCorners[i].x;
            }
            if (objectCorners[i].x > screenBounds.xMax) {
                offset.x = screenBounds.xMax - objectCorners[i].x;
            }
            if (objectCorners[i].y < screenBounds.yMin) {
                offset.y = screenBounds.yMin - objectCorners[i].y;
            }
            if (objectCorners[i].y > screenBounds.yMax) {
                offset.y = screenBounds.yMax - objectCorners[i].y;
            }
        }
 
        return offset;
    }

    //Moves the selected item from buyable items to items in cart
    public void AddItemToCart(int character) {
        Item temp = storeItems[itemSelected];
        storeItems.Insert(itemSelected, null);
        storeItems.Remove(temp);

        cartItems.Add(temp);
        previousLocations.Add(itemSelected);
        charToBuyFor.Add(playerCharacters[character]);
        cost += temp.value;

        buyForMenu.SetActive(false);
        SetStoreInfo();
    }

    //Moves the selected item from items in the cart to buyable items
    public void RemoveItemFromCart(int index) {
        Item temp = cartItems[index];
        storeItems.RemoveAt(previousLocations[index]);
        storeItems.Insert(previousLocations[index], temp);
        cost -= temp.value;
        cartItems.RemoveAt(index);
        previousLocations.RemoveAt(index);
        charToBuyFor.RemoveAt(index);
        SetStoreInfo();
    }

    //Move items from cart to respective character inventories, and remove silver cost from all characters equally
    public void PurchaseItems() {
        if (cartItems.Count == 0 && sellingItems.Count == 0) {
            ErrorMessage("Nothing to buy/sell!");
        } else if(GetPayment(cost)) {
            for (int i = 0; i < cartItems.Count; ++i) {
            charToBuyFor[i].GetInventory().AddItem(cartItems[i]);
            }

            cartItems.Clear();
            previousLocations.Clear();
            charToBuyFor.Clear();
            sellingItems.Clear();
            charToSellFrom.Clear();
            cost = 0;
        }
        SetStoreInfo();
    }

    //Pulls an equal amount of silver from each player owned character
    public bool GetPayment(int charge) {
        if (silver >= charge) {
            int totalChars = playerCharacters.Count + reserveCharacters.Count;
            CharacterSheet[] characters = new CharacterSheet[totalChars];
            playerCharacters.CopyTo(characters);
            reserveCharacters.CopyTo(characters, playerCharacters.Count);
            int j = 0;
            while (charge != 0) {
                if (charge > 0) {
                    if (characters[j % totalChars].GetSilver() > 0)
                        characters[j % totalChars].MakePayment(1);
                        --charge;
                    ++j;
                } else {
                    characters[j % totalChars].MakePayment(-1);
                    ++charge;
                    ++j;
                }
            }
            silver = CalculateSilver();
            SetStoreInfo();
            return true;
        } else {
            ErrorMessage("Not enough silver!");
            return false;
        }
    }

    //Sets the characters to sell from to characters currently in party
    public void SetCharacterSellList() {
        for(int i = 0; i < 4; ++i) {
            if (i < playerCharacters.Count) {
                sellFromList.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerCharacters[i].GetCharacterName();
                sellFromList.transform.GetChild(i).gameObject.SetActive(true);
            } else {
                sellFromList.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    //Change sellable items depending on which character is selected
    public void SelectSellFromCharacter(int target) {
        if(playerCharacters.Count <= target) {
            sellableItems = new List<Item>();
            sellFromCharacter = null;
        } else {
            sellFromCharacter = playerCharacters[target];
            sellableItems = sellFromCharacter.inventory.itemList;

        }
        SetStoreInfo();
    }
    
    //adds a single item to selling cart, links what character the item came from, and reduces the cost relevant to the value of item
    public void AddItemToSellCart(int item) {
        Item temp = sellableItems[item];
        sellableItems.Remove(temp);

        sellingItems.Add(temp);
        charToSellFrom.Add(sellFromCharacter);
        cost -= temp.value;
        SetStoreInfo();
    }

    //removes a single item from selling cart
    public void RemoveItemFromSellCart(int index) {
        Item temp = sellingItems[index];
        sellableItems.Add(temp);
        cost += temp.value;
        sellingItems.RemoveAt(index);
        charToSellFrom.RemoveAt(index);
        SetStoreInfo();
    }

    //removes all items from both buy and sell carts
    public void CancelTransaction() {
        RemoveAllFromCart();
        RemoveAllFromSellCart();
    }

    //removes all items from buying cart
    public void RemoveAllFromCart() {
        int numCartItems = cartItems.Count;
        for (int i = 0; i < numCartItems; ++i) {
            RemoveItemFromCart(0);
        }
    }

    //removes all items from selling cart
    public void RemoveAllFromSellCart() {
        int numSellingItems = sellingItems.Count;
        for (int i = 0; i < numSellingItems; ++i) {
            RemoveItemFromSellCart(0);
        }
    }
}