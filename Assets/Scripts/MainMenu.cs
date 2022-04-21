using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameManager GM;
    public Transform GMTransform;
    public GameObject characterPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameManager.GM;
        GMTransform = GameManager.GM.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartNewGame() {
        for (int i = 0; i < 4; ++i) {
            GameObject temp = Instantiate(characterPrefab, GMTransform);
            GM.playerCharacters.Add(temp.GetComponent<CharacterSheet>());
        }
        for (int i = 0; i < GM.playerCharacters.Count; ++i) {
            GM.playerCharacters[i].InitializeRandomClassless();
        }

        //this assigns sprites
        foreach(CharacterSheet charSheet in GM.playerCharacters)
            charSheet.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = charSheet.GetSprite();

        SceneManager.LoadScene("Town", LoadSceneMode.Single);
    }

    public void exitGame() {
        Application.Quit();
    }
}
