using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    int buttonSelected = -1;
    public GameObject[] buttons;
    int page = 0;
    static int OPTIONS_PER_PAGE = 8;

    public void ButtonSelected(int i)
    {
        //figure out page thing
        buttonSelected = i;
    }

    public int PullSelected(){
        int toReturn = buttonSelected;
        buttonSelected = -1;
        return toReturn;
    }

    public bool IsSelected(){return buttonSelected >= 0;}
}
