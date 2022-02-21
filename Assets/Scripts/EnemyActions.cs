using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActions : MonoBehaviour
{
    [SerializeField]
    ActionList actionList;
    int move; //this stores the last move and can be used as part of the CalculateAction function

    // Controls enemy moves and actions. Currently basic, doesn't check situation or anything similar. Will eventually add more complexity.
    // Currently moves to back if health is low, or does random move if health is not low, will need to look at enemies stats and positions to decide what to do
    public int CalculateMove (CharacterSheet character) {
        /*
        0 = stand ground
        1 = advance to front
        2 = retreat to back
        3 = sneak to enemy lines
        */
 
        //ActionList should probably be thought of as "Calculation Parameters" or "Behavior Tendencies"
        //It can (and should) contain more than just the list of actions, but the reasons why
        //Add variables to ActionList that we can modify for each character type
        if (character.GetHitPoints() < character.GetMaxHitPoints() / actionList.backOffPercent) {
            //should we also check if they're the leader? why would rank 2 move back?
            move = 2;
        } else {
            //then randomize from amongst all four moves?
            move = Random.Range(0, 4);
        }

        return move;
    }

    // Enemies will always attack for their action
    // Assumes actionList.BattleActions[0] is fight
    public CharacterAction CalculateAction(BattleManager battleManager, CharacterSheet actor) {
        return actionList.BattleActions[0];
    }

    public CharacterSheet CalculateTarget(BattleManager battleManager, CharacterSheet actor, CharacterAction charAction){

        CharacterSheet target = null;
        List<CharacterSheet> targets = new List<CharacterSheet>();

        switch(charAction.targetingType){
            case CharacterAction.TargetingType.Allies:
                targets = battleManager.AllyTargets(actor);
                break;
            case CharacterAction.TargetingType.Any:
                targets = battleManager.AnyTargets(actor);
                break;
            case CharacterAction.TargetingType.Enemies:
                targets = battleManager.EnemyTargets(actor);
                break;
            case CharacterAction.TargetingType.NearbyAllies:
                targets = battleManager.NearbyAllyTargets(actor);
                break;
            case CharacterAction.TargetingType.WeaponAttack:
                targets = battleManager.WeaponAttackTargets(actor);
                break;
        }

        target = Fun.RandomFromArray(targets.ToArray());

        return target;
    }
}
