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

    //Player Characters
    public List<CharacterSheet> playerCharacters;
    public List<CharacterSheet> reserveCharacters;

    //Recruitable Characters
    public List<CharacterSheet> recruitableCharacters;

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
    public GameObject itemInfo;
    public TextMeshProUGUI silverCount;
    public GameObject buyForMenu;
    public int itemToBuy;

    public int silver;

    void Awake() {
        GM = GameManager.GM;
        GMTransform = GameManager.GM.transform;
        generateRandom();
        StoreGen();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCharacters = GM.playerCharacters;
        reserveCharacters = GM.reserveCharacters;
        setRecCharInfo();
        setCharInfo();
        setReserveCharInfo();
        setStoreInfo();
        SetBuyForMenu();
        silver = CalculateSilver();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    //Changes whether an object is active or not
    public void toggleActive(GameObject target) {
        if (target.activeSelf) {
            target.SetActive(false);
        } else {
            target.SetActive(true);
        }
    }

    //Reset player health
    // public void rest() {
    //     foreach (CharacterSheet character in playerCharacters) {
    //         character.RecoverDamage(new Damage(50, 20, 10, new DamageType(Untyped)));
    //     }
    // }

    

    //Set info about characters in player party, if there are more spaces than characters deactivate unused spaces
    public void setCharInfo() {
        //Set each tile with character info
        for (int i = 0; i < 4; ++i) {
            if (i < playerCharacters.Count) {
                charMenuTiles[i].SetActive(true);
                setInfo(charMenuTiles, i, playerCharacters, i);
            } else {
                charMenuTiles[i].SetActive(false);
            }
        }
    }

    //Set info about characters in player party, if there are more spaces than characters deactivate unused spaces
    public void setReserveCharInfo() {
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
        for (int i = 0; i < 2; ++i) {
            if (i + (pageNumber * 2) < reserveCharacters.Count) {
                reserveCharMenuTiles[i].SetActive(true);
                setInfo(reserveCharMenuTiles, i, reserveCharacters, i + pageNumber * 2);
            } else {
                reserveCharMenuTiles[i].SetActive(false);
            }
        }
    }

    //Set info about recruitable characters, if there are more spaces than characters deactivate unused spaces
    public void setRecCharInfo() {
        for (int i = 0; i < 4; ++i) {
            if (i < recruitableCharacters.Count) {
                recruitMenuTiles[i].SetActive(true);
                setInfo(recruitMenuTiles, i, recruitableCharacters, i);
                
            } else {
                recruitMenuTiles[i].SetActive(false);
            }
        }
    }

    //Takes tile list, index of tile list, character list, and index of character list, and sets tile info to character info
    public void setInfo(List<GameObject> tiles, int tIndex, List<CharacterSheet> characters, int cIndex) {
        tiles[tIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = characters[cIndex].GetCharacterName();
        tiles[tIndex].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Max HP: " + characters[cIndex].GetMaxHitPoints().ToString();
        tiles[tIndex].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Strength: " + characters[cIndex].GetStrength().ToString();
        tiles[tIndex].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Agility: " + characters[cIndex].GetAgility().ToString();
        tiles[tIndex].transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Presence: " + characters[cIndex].GetPresence().ToString();
        tiles[tIndex].transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "Toughness: " + characters[cIndex].GetToughness().ToString();
    }
    

    //Swap a character from the recruitable character under TownManager to player characters under GameManager
    public void addCharToPlayerChars(int index) {
        GameObject character = recruitableCharacters[index].transform.gameObject;
        CharacterSheet charSheet = character.GetComponent<CharacterSheet>();
        character.transform.parent = GMTransform;
        recruitableCharacters.Remove(charSheet);
        if (playerCharacters.Count < 4) {
            playerCharacters.Add(charSheet);
        } else {
            reserveCharacters.Add(charSheet);
        }
        setCharInfo();
        setReserveCharInfo();
        setRecCharInfo();
        SetBuyForMenu();
        silver = CalculateSilver();
    }

    //Swap the char at the index from the player party to reserve characters
    public void removeCharFromParty(int index) {
        CharacterSheet charSheet = playerCharacters[index];
        playerCharacters.Remove(charSheet);
        reserveCharacters.Add(charSheet);
        setCharInfo();
        setReserveCharInfo();
        SetBuyForMenu();
    }

    //Swap the char at the index from reserve characters to the player party
    public void addCharToParty(int index) {
        if (playerCharacters.Count < 4) {
            CharacterSheet charSheet = reserveCharacters[index + (pageNumber * 2)];
            playerCharacters.Add(charSheet);
            reserveCharacters.Remove(charSheet);
            setCharInfo();
            setReserveCharInfo();
            SetBuyForMenu();
        }
    }

    //Generates random recruitable characters
    public void generateRandom() {
        for (int i = 0; i < 4; ++i) {
            GameObject temp = Instantiate(characterPrefab, TMTransform);
            recruitableCharacters.Add(temp.GetComponent<CharacterSheet>());
        }
        for (int i = 0; i < recruitableCharacters.Count; ++i) {
            recruitableCharacters[i].InitializeRandomClassless();
        }
        setRecCharInfo();
    }

    //Increases page number and reloads reserve characters
    public void incrementPageNumber() {
        ++pageNumber;
        setReserveCharInfo();
    }

    //Decreases page number and reloads reserve characters
    public void decrementPageNumber() {
        --pageNumber;
        setReserveCharInfo();
    }

    //ensures party is not empty and enters the rest of the dungeon
    public void enterDungeon() {
        if (playerCharacters.Count > 0) {
            SceneManager.LoadScene("DungeonGeneration", LoadSceneMode.Single);
        }
    }

    public void setStoreInfo() {
        for (int i = 0; i < storeTiles.Count; ++i) {
            if (storeItems[i] != null) {
                storeTiles[i].GetComponent<Image>().sprite = storeItems[i].GetSprite();
            } else {
                storeTiles[i].SetActive(false);
            }
            HideItemInfo();
        }
    }

    public void StoreGen() {
        for (int i = 0; i < storeTiles.Count; ++i) {
            int randomIndex = UnityEngine.Random.Range(0, 2);
            if (randomIndex == 0) {
                int randomList = UnityEngine.Random.Range(0, 6);
                Item temp = (ItemManager.RANDOM_ITEM(ItemManager.STARTING_WEAPONS).Copy());
                switch (randomList) {
                    case 0:
                        temp = (ItemManager.RANDOM_ITEM(ItemManager.BAGS).Copy());
                        Debug.Log("check case 0");
                        break;
                    case 1:
                        temp = (ItemManager.RANDOM_ITEM(ItemManager.ADVENTURE_TOOLS).Copy());
                        Debug.Log("check case 1");
                        break;
                    case 2:
                        temp = (ItemManager.RANDOM_ITEM(ItemManager.SPECIAL_ITEMS).Copy());
                        Debug.Log("check case 2");
                        break;
                    case 3:
                        temp = (ItemManager.RANDOM_ITEM(ItemManager.STARTING_WEAPONS).Copy());
                        Debug.Log("check case 3");
                        break;
                    case 4:
                        temp = (ItemManager.RANDOM_ITEM(ItemManager.STARTING_ARMORS).Copy());
                        Debug.Log("check case 4");
                        break;
                    case 5:
                        temp = (ItemManager.RANDOM_ITEM(ItemManager.STARTING_ARMORS_LOWTIER).Copy());
                        Debug.Log("check case 5");
                        break;
                }
                Debug.Log("end of switch check");
                //temp.gameObject.parent = TMTransform;
                storeItems.Add(temp);
            } else {
                storeItems.Add(null);
            }
        }
        silverCount.text = silver.ToString();
    }

    public void DisplayItemInfo(GameObject button) {
        int index = storeTiles.IndexOf(button);
        Item item = storeItems[index];
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

        itemInfo.SetActive(true);
    }

    public void HideItemInfo() {
        itemInfo.SetActive(false);
    }

    public int CalculateSilver() {
        int total = 0;
        foreach(CharacterSheet character in playerCharacters) {
            silver += character.GetSilver();
        }
        foreach(CharacterSheet character in reserveCharacters) {
            silver += character.GetSilver();
        }
        return total;
    }

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

    public void OpenBuyForMenu(int itemNum) {
        itemToBuy = itemNum;
        buyForMenu.transform.position = Input.mousePosition;
        if (playerCharacters.Count != 0) {
            buyForMenu.transform.position = Input.mousePosition + GetGUIElementOffset(buyForMenu.transform.GetChild(5-playerCharacters.Count).GetComponent<RectTransform>());
        } else {
            buyForMenu.transform.position = Input.mousePosition + GetGUIElementOffset(buyForMenu.transform.GetChild(0).GetComponent<RectTransform>());
        }
        buyForMenu.SetActive(true);
    }

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
}
