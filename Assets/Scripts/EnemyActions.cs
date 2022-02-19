using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActions : MonoBehaviour
{
// Controls enemy moves and actions. Currently basic, doesn't check situation or anything similar. Will eventually add more complexity.
    // Currently moves to back if health is low, or does random move if health is not low, will need to look at enemies stats and positions to decide what to do
    public static int enemyMove (CharacterSheet enemy) {
        if (enemy.GetHitPoints() < enemy.GetMaxHitPoints() / 3) {
            return 2;
        }
        return Random.Range(0, 4);
    }

    // Enemies will always attack for their action
    public static void enemyAction(CharacterSheet enemy, List<CharacterSheet> targets, int round) {
        CharacterSheet target = Fun.RandomFromArray(targets.ToArray());
        ActionManager.AM.LoadAction(enemy, target, null, enemy.fight);
        return;
    }
}
