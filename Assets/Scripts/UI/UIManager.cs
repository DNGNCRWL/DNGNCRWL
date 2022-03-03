using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
