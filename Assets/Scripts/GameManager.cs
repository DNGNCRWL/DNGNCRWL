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
        else { Destroy(gameObject); return; }

        playerCharacters = new List<GameObject>();
        foreach (GameObject go in characters) playerCharacters.Add(go);
    }

    static public int RollDie(int dieSize)
    {
        if (dieSize < 1) return 1;
        else return UnityEngine.Random.Range(0, dieSize) + 1;
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

public enum DamageType { Untyped, Bludgeon, Cut, Electric, Fire, Magic, Pierce, Spirit }

[System.Serializable]
public struct Damage
{
    public Damage(int dieCount, int dieSize, DamageType damageType)
    {
        this.dieCount = dieCount;
        this.dieSize = dieSize;
        this.damageType = damageType;
    }

    public int dieCount;
    public int dieSize;
    public DamageType damageType;
}