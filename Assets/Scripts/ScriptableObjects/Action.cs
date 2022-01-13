using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Action", menuName = "Action", order = 1)]
public class Action : ScriptableObject
{
    /*
     * Actions are for doing things.
     * 
     * There is always an actor for an action.
     * There can be a
     *      -list of functions that operate on the actor
     *      -list of targets (other CharacterSheets)
     *      -list of actions for the targets to execute
     * 
     * e.g.
     * 
     * Drink Potion
     *      -Alter stats
     *      -
     * 
     * Fight:
     *      -No functions for it to do on its own
     *      -One target
     *      -Target executes take damage based on the character sheet's mainhand weapon
     *      
     *      
     */


}