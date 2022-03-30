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
    public float lootRate;
    public Item[] loot;

    public Item GetLoot(){
        Item ret = null;
        float random = Random.Range(0, 1);
        if(random < lootRate && loot.Length > 0){
            ret = loot[Random.Range(0, loot.Length)].Copy();
        }
        return ret;
    }

    public int GetSilver(){
        int silver = 0;
        foreach(GameObject enemy in enemies){
            CharacterSheet charSheet = enemy.GetComponent<CharacterSheet>();
            silver += charSheet.GetSilver();
        }
        return silver;
    }

    public float GetExperience(){
        float exp = 0;
        foreach(GameObject enemy in enemies){
            CharacterSheet charSheet = enemy.GetComponent<CharacterSheet>();
            exp += charSheet.GetExperience();
        }
        return exp;
    }
}
