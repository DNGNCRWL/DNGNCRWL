using UnityEngine;
using UnityEditor;

[System.Serializable]
public class d20
{
    [HideInInspector] public int difficultyRating;
    [HideInInspector] public CharacterSheet actor;
    public StatSelector actorStat;
    [HideInInspector] public CharacterSheet target;
    public StatSelector targetStat;
    int roll;
    bool randomized;

    public d20(int difficultyRating, CharacterSheet actor, StatSelector actorStat, CharacterSheet target, StatSelector targetStat)
    {
        this.difficultyRating = difficultyRating;
        this.actor = actor;
        this.actorStat = actorStat;
        this.target = target;
        this.targetStat = targetStat;
        randomized = false;
    }

    public d20(int difficultyRating, CharacterSheet actor, Stat actorStat, CharacterSheet target, Stat targetStat)
    {
        this.difficultyRating = difficultyRating;
        this.actor = actor;
        this.actorStat = FromStatToStatSelector(actorStat);
        this.target = target;
        this.targetStat = FromStatToStatSelector(targetStat);
        randomized = false;
    }
    
    int Bonus() { return (actor) ? actor.GetStat(FromStatSelectorToStat(actorStat, actor)) : 0; }
    int Penalty() { return (target) ? target.GetStat(FromStatSelectorToStat(targetStat, target)) : 0; }
    int AdjustedDR() { return difficultyRating + Penalty() - Bonus(); }

    public d20(int difficultyRating, CharacterSheet actor, StatSelector actorStat) : this(difficultyRating, actor, actorStat, null, 0) { }

    void Randomize() { if (!randomized) roll = GameManager.RollDie(20); randomized = true; }

    public int Roll() { Randomize(); return roll; }
    public bool IsSuccess() { Randomize(); return roll > AdjustedDR() || IsCritical(); }
    public bool IsCritical() { Randomize(); return roll == 20; }
    public bool IsSuccessOnly() { Randomize(); return roll > AdjustedDR() && !IsCritical(); }
    public bool IsFailure() { Randomize(); return roll <= AdjustedDR() || IsFumble(); }
    public bool IsFumble() { Randomize(); return roll == 1; }
    public bool IsFailureOnly() { Randomize(); return roll <= AdjustedDR() && !IsFumble(); }

    static Stat FromStatSelectorToStat(StatSelector selectedStat, CharacterSheet character)
    {
        switch (selectedStat)
        {
            case StatSelector.Agility: return Stat.Agility;
            case StatSelector.Presence: return Stat.Presence;
            case StatSelector.Strength: return Stat.Strength;
            case StatSelector.Toughness: return Stat.Toughness;
            case StatSelector.Defense: return Stat.Defense;
            case StatSelector.WeaponStat:
                if (character) return character.GetWeapon().abilityToUse;
                else return 0;
        }

        return 0;
    }

    static StatSelector FromStatToStatSelector(Stat stat)
    {
        switch (stat)
        {
            case Stat.Agility: return StatSelector.Agility;
            case Stat.Defense: return StatSelector.Defense;
            case Stat.Presence: return StatSelector.Presence;
            case Stat.Strength: return StatSelector.Strength;
            case Stat.Toughness: return StatSelector.Toughness;
        }

        return 0;
    }

    public bool CompareTo(int newStatValue)
    {
        return roll > (difficultyRating - Bonus() + newStatValue);
    }
}