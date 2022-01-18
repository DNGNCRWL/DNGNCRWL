using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;
    List<GameObject> playerCharacters;

    public GameObject[] characters;

    private void Awake()
    {
        if (GM == null) GM = this;
        else { Destroy(gameObject); return; }
        GameObject.DontDestroyOnLoad(this.gameObject);

        DOTween.Init(null, null, null);

        playerCharacters = new List<GameObject>();
        foreach (GameObject go in characters) playerCharacters.Add(go);
    }

    static public int RollDie(int dieSize)
    {
        if (dieSize < 0)
            return UnityEngine.Random.Range(0, -dieSize) - 1;
        else
            return UnityEngine.Random.Range(0, dieSize) + 1;
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


    public static CharacterSheet GetTarget()
    {
        //if in battle list enemies
        //if in status menu list allies
        return null;
    }


}

[System.Serializable]
public enum DescriptorSelector { Default, Fight }

[System.Serializable]
public enum StatSelector { Strength, Agility, Presence, Toughness, Defense, WeaponStat }

[System.Serializable]
public enum Stat { Strength, Agility, Presence, Toughness, Defense };

[System.Serializable]
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