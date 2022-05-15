using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    // Decides which wall based off of order passed in from unity
    public GameObject[] walls; // 0 - Up 1 -Down 2 - Right 3- Left (our current order passed in )
    // Decides which door based off of order passed in from unity
    public GameObject[] doors; // 0 - Up 1 -Down 2 - Right 3- Left (our current order passed in)

    // Opens (sets active) the door and sets wall inactive
    public void UpdateRoom(bool[] status)
    {
        for (int i = 0; i < status.Length; i++)
        {
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
    }
}