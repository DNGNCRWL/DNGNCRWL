using UnityEngine;
using UnityEditor;

[System.Serializable]
public class d20
{
    public int difficultyRating;
    public CharacterSheet actor;
    public Stat actorStat;
    public CharacterSheet target;
    public Stat targetStat;
    int roll;
    bool randomized;

    public d20(int difficultyRating, CharacterSheet actor, Stat actorStat, CharacterSheet target, Stat targetStat)
    {
        this.difficultyRating = difficultyRating;
        this.actor = actor;
        this.actorStat = actorStat;
        this.target = target;
        this.targetStat = targetStat;
        randomized = false;
    }
    
    int Bonus() { return (actor) ? actor.GetStat(actorStat) : 0; }
    int Penalty() { return (target) ? target.GetStat(targetStat) : 0; }
    int AdjustedDR() { return difficultyRating + Penalty() - Bonus(); }

    public d20(int difficultyRating, CharacterSheet actor, Stat actorStat) : this(difficultyRating, actor, actorStat, null, 0) { }
    public d20(int difficultyRating) : this(difficultyRating, null, 0, null, 0) { }
    public d20() : this(10) { }

    void Randomize() { if (!randomized) roll = GameManager.RollDie(20); randomized = true; }

    public int Roll() { Randomize(); return roll; }
    public bool IsSuccess() { Randomize(); return roll > AdjustedDR() || IsCritical(); }
    public bool IsCritical() { Randomize(); return roll == 20; }
    public bool IsSuccessOnly() { Randomize(); return roll > AdjustedDR() && !IsCritical(); }
    public bool IsFailure() { Randomize(); return roll <= AdjustedDR() || IsFumble(); }
    public bool IsFumble() { Randomize(); return roll == 1; }
    public bool IsFailureOnly() { Randomize(); return roll <= AdjustedDR() && !IsFumble(); }

    public bool CompareTo(int newStatValue)
    {
        return roll > (difficultyRating - Bonus() + newStatValue);
    }
}