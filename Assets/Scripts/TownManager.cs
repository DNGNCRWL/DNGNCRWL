using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TownManager : MonoBehaviour
{
    public GameObject PartySwapButton;
    public GameObject RecruitButton;
    public GameObject RestButton;
    public GameObject StoreButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void toggleActive(GameObject target) {
        if (target.activeSelf) {
            target.SetActive(false);
        } else {
            target.SetActive(true);
        }
    }

    public void toggleBackground() {
        toggleActive(PartySwapButton);
        toggleActive(RecruitButton);
        toggleActive(RestButton);
        toggleActive(StoreButton);
    }
}
