using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action", menuName = "Action", order = 1)]
public class CharacterAction : ScriptableObject
{
    public string actionName;
    public bool needsTarget;
    public int difficultyRating;
    public int numberOfDice;
    public int dieSize;
}