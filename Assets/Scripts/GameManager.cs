using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;
    List<GameObject> playerCharacters;

    public GameObject[] characters;

    public static readonly int secondsPerRound = 6;

    public static ActionTextSpawner DEFAULT_TEXT_SPAWNER;

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

    //Enums to String???
    public static string DamageTypeToString(DamageType type)
    {
        switch (type)
        {
            case DamageType.Bludgeon: return "Bludgeon";
            case DamageType.Cut: return "Cut";
            case DamageType.Electric: return "Electric";
            case DamageType.Fire: return "Fire";
            case DamageType.Magic: return "Magic";
            case DamageType.Pierce: return "Pierce";
            case DamageType.Spirit: return "Spirit";
        }

        return "Untyped";
    }

}

[System.Serializable]
public enum StatSelector { Strength, Agility, Presence, Toughness, Defense, WeaponStat }

[System.Serializable]
public enum Stat { Strength, Agility, Presence, Toughness, Defense };

[System.Serializable]
public struct CharacterRollingPackage
{
    public CharacterRollingPackage(int strength, int agility, int presence, int toughness, int hpDieSize,
        int powers, int omens, int silverDieCount, int silverDieSize, int food)
    {
        this.strength = strength;
        this.agility = agility;
        this.presence = presence;
        this.toughness = toughness;
        this.hpDieSize = hpDieSize;
        this.powers = powers;
        this.omens = omens;
        this.silverDieCount = silverDieCount;
        this.silverDieSize = silverDieSize;
        this.food = food;
    }

    public int strength;
    public int agility;
    public int presence;
    public int toughness;
    public int hpDieSize;
    public int powers;
    public int omens;
    public int silverDieCount;
    public int silverDieSize;
    public int food;
}