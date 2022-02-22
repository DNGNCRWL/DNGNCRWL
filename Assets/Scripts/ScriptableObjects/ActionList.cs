using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Behavior", menuName = "Enemy Behavior", order = 1)]
public class ActionList : ScriptableObject
{
    //add variables here to use for stuff like

    public float backOffPercent;

    public List<CharacterAction> BattleActions;
}
