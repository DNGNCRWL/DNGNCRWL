using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Encounter", menuName = "Enemy/Enemy Encounter", order = 1)]
public class EnemyEncounter : ScriptableObject
{
    [SerializeField] string encounterName;
    [SerializeField] GameObject[] enemies;

    public string GetEncounterName(){return encounterName;}
    public GameObject[] GetEnemies(){return enemies;}
}
