using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    int buttonSelected = -1;
    public GameObject[] buttons;

    public void ButtonSelected(int i) {buttonSelected = i;}

    public int PullSelected(){
        int toReturn = buttonSelected;
        buttonSelected = -1;
        return toReturn;
    }

    public bool IsSelected(){return buttonSelected >= 0;}
}
