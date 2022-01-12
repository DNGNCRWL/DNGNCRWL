using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;
    List<GameObject> playerCharacters;

    public GameObject[] characters;

    private void Awake()
    {
        if (GM == null) GM = this;
        else Destroy(gameObject);

        playerCharacters = new List<GameObject>();
        foreach (GameObject go in characters) playerCharacters.Add(go);
    }

    static public int RollDie(int dieSize)
    {
        return Mathf.Max(1, UnityEngine.Random.Range(1, dieSize+1));
    }

    static public int RollDice(int number, int dieSize)
    {
        int result = 0;
        for (int i = 0; i < number; i++)
        {
            result += RollDie(dieSize);
        }
        return result;
    }

    public static bool Error(string message)
    {
        Debug.Log(message);
        return false;
    }
}

public class d20
{
    public bool fumble;
    public bool critical;
    public int value;

    public d20()
    {
        value = GameManager.RollDie(20);
        if (value == 1) fumble = true;
        else if (value == 20) critical = true;
    }

    public bool Normal() { return !fumble && !critical; }
}