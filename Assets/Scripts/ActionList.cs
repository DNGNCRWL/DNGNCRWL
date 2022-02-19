using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionList : MonoBehaviour
{
//stores the set of actions for each type of character. Different types of characters can be used by applying one of the special sets.

    public static CharacterAction standGround;
    public static CharacterAction defend;
    public static CharacterAction returnToBack;
    public static CharacterAction sneak;
    public static CharacterAction fight;


    public static List<CharacterAction> genericMoveActions = new List<CharacterAction>();

    public static List<CharacterAction> genericBattleActions = new List<CharacterAction>();

    private static void Awake() {
    genericMoveActions.Add(standGround);
    genericMoveActions.Add(defend);
    genericMoveActions.Add(returnToBack);
    genericMoveActions.Add(sneak);

    genericBattleActions.Add(fight);
    }
}
