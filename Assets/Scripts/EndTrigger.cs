using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using System.Collections;
public class EndTrigger : MonoBehaviour
{
    public bool gameHasEnded = false;
    public EnemyEncounter[] boss_encounters;
    public GameObject spider;
    public static bool UPSTAIRCOLLISION;
    public static bool DOWNSTAIRCOLLISION;

    public void OnTriggerEnter(Collider collider)
    {
        Debug.Log(gameObject.name + " has triggered end by " + collider.gameObject.name);
      
            if (gameObject.name.Equals("stairwell") && !collider.isTrigger)
            {
                if(DungeonGenerator.keys[DungeonGenerator.LEVEL]){
                    
                    DungeonGenerator.isSpider = true;
                    DungeonGenerator.LEVEL++;
                    FindObjectOfType<DungeonGenerator>().setSIZEUp();
                    FindObjectOfType<DungeonGenerator>().getLevel();
                    collider.isTrigger=false;
                    UPSTAIRCOLLISION = true;
                    Debug.Log("stair!");
                    Invoke("ResetPlayer", .1f);
                    return;
                }else{
                    FindObjectOfType<DungeonGenerator>().getBossMessage();
                    Debug.Log("YOU DONT HAVE THE KEY!");
                }
                return;
            }
            if (gameObject.name.Equals("downstairwell"))
            {
                DOWNSTAIRCOLLISION = true;
                if(DungeonGenerator.LEVEL == 0){
                    Navigation.INSTANCE.SetActive(false);
                    SceneManager.LoadScene("Town");

                }else{
                    DungeonGenerator.isSpider = false;
                    DungeonGenerator.LEVEL--;
                    FindObjectOfType<DungeonGenerator>().getLevel();
                    FindObjectOfType<DungeonGenerator>().setSIZEDown();
                    
                    Invoke("ResetPlayer", .1f);
                }


                return;
            }
            if (gameObject.name.Equals("Dungeon Spider(Clone)"))
            {
                DungeonGenerator.isSpider = false;

                DontDestroyOnLoad(Navigation.INSTANCE);

                DungeonGenerator.keys[DungeonGenerator.LEVEL] = true;
                DungeonGenerator.keys.Add(false);
                EnemyEncounter boss_encounter = boss_encounters[Random.Range(0, boss_encounters.Length)];
                BattleManager.SetENEMY_ENCOUNTER(boss_encounter);
                StartCoroutine(WaitForSceneLoad());

                Debug.Log("passed if");
                return;
            }


    }
    
    private IEnumerator WaitForSceneLoad()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Battle");
        Destroy(spider);
        Navigation.INSTANCE.SetActive(false);
    }

    private void ResetPlayer(){
        Debug.Log("HERE???");
        FindObjectOfType<DungeonGenerator>().DestroyAll();
        FindObjectOfType<DungeonGenerator>().Start();
    }

    private void setInactive(){
        Navigation.INSTANCE.SetActive(false); 
        Debug.Log("setting inactive");
    }
    
}