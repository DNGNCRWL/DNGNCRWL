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
    public int difficultyRating;
    public int roll;

    public d20(int difficultyRating, CharacterSheet actor, Stat actorStat, CharacterSheet target, Stat targetStat)
    {
        int bonus = (actor) ? actor.GetStat(actorStat) : 0;
        int penalty = (target) ? target.GetStat(targetStat) : 0;

        difficultyRating += penalty - bonus;

        roll = GameManager.RollDie(20);
    }

    public bool Critical() { return roll == 20; }
    public bool Fumble() { return roll == 1; }
    public bool Success() {return roll > difficultyRating || Critical(); }
    public bool Failure() { return roll <= difficultyRating || Fumble(); }
}