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
    public int roll;

    public d20(int difficultyRating, CharacterSheet actor, Stat actorStat, CharacterSheet target, Stat targetStat)
    {
        this.difficultyRating = difficultyRating;
        this.actor = actor;
        this.actorStat = actorStat;
        this.target = target;
        this.targetStat = targetStat;

        roll = GameManager.RollDie(20);
    }
    
    int Bonus() { return (actor) ? actor.GetStat(actorStat) : 0; }
    int Penalty() { return (target) ? target.GetStat(targetStat) : 0; }
    int AdjustedDR() { return difficultyRating + Penalty() - Bonus(); }

    public d20(int difficultyRating, CharacterSheet actor, Stat actorStat) : this(difficultyRating, actor, actorStat, null, 0) { }
    public d20(int difficultyRating) : this(difficultyRating, null, 0, null, 0) { }
    public d20() : this(10) { }

    public bool IsSuccess() { return roll > AdjustedDR() || IsCritical(); }
    public bool IsCritical() { return roll == 20; }
    public bool IsSuccessOnly() { return roll > AdjustedDR() && !IsCritical(); }
    public bool IsFailure() { return roll <= AdjustedDR() || IsFumble(); }
    public bool IsFumble() { return roll == 1; }
    public bool IsFailureOnly() { return roll <= AdjustedDR() && !IsFumble(); }
}