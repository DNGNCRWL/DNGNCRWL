using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using System.Collections;
public class EndTrigger : MonoBehaviour
{
    public bool gameHasEnded = false;
    public float dungeonGenerationDelay = 1f;
    public GameManager gameManager;
    public EnemyEncounter[] boss_encounters;
    public static bool COLLIDE;
    public Navigation player;
    public GameObject spider;
    public static bool UPSTAIRCOLLISION;
    public static bool DOWNSTAIRCOLLISION;
    //private float delay = 10f;
    //private float timeElapsed;
    public void OnTriggerEnter(Collider collider)
    {
        Debug.Log(gameObject.name + " has triggered end by " + collider.gameObject.name);
        if (gameHasEnded == false)
        {
            if (gameObject.name.Equals("stairwell") && !collider.isTrigger)
            {
                collider.isTrigger=false;
                //FindObjectOfType<Navigation>().respawn();
                UPSTAIRCOLLISION = true;
                //FindObjectOfType<Navigation>().respawn();
                Debug.Log("stair!");
                //SceneManager.LoadScene("Town");
                // DungeonGenerator.LevelIncreased();
                DungeonGenerator.LEVEL++ ;
                FindObjectOfType<DungeonGenerator>().getLevel();
                DungeonGenerator.isSpider = false;
                // FindObjectOfType<DungeonGenerator>().DestroyAll();
                //Invoke("ResetPlayer", .1f);
                
                //WaitForTownLoad();
                //FindObjectOfType<DungeonGenerator>().Start();
                Invoke("ResetPlayer", .1f);
                //ResetPlayer();
                //FindObjectOfType<DungeonGenerator>().DestroyAll();
                
                //StartCoroutine(WaitForTownLoad());
                //Invoke("RestartGenerateDungeon", .1f);

                return;
            }
            if (gameObject.name.Equals("downstairwell"))
            {
                Debug.Log("downstairwell!");
                DOWNSTAIRCOLLISION = true;
                Debug.Log(DungeonGenerator.LEVEL);
                //Invoke("RestartGenerateDungeon", .1f);
                if(DungeonGenerator.LEVEL == 0){
                    Navigation.INSTANCE.SetActive(false);
                    SceneManager.LoadScene("Town");

                }else{
                    FindObjectOfType<DungeonGenerator>().getLevel();
                    DungeonGenerator.LEVEL--;
                    Invoke("ResetPlayer", .1f);
                }
                //WaitForTownLoad();
                
                //WaitForTownLoad();
                
                Debug.Log("passed it!!");
                return;
            }
            if (gameObject.name.Equals("remodel_tarantula(export) Variant(Clone)"))
            {
                EnemyEncounter boss_encounter = boss_encounters[Random.Range(0, boss_encounters.Length)];
                BattleManager.SetENEMY_ENCOUNTER(boss_encounter);

                StartCoroutine(WaitForSceneLoad());

                Debug.Log("passed if");
                return;
            }
            //            gameHasEnded = true;
            //            COLLIDE = true;
            //           //gameManager.CompleteLevel();
            //          Debug.Log("endtrigger!");
            //          GameManager.GM.CompleteLevel();
            //          Invoke("RestartGenerateDungeon", dungeonGenerationDelay);
            //          gameHasEnded = false;

        }

    }
    public void RestartGenerateDungeon()
    {
        //SceneManager.LoadScene("Town");
        //DungeonGenerator.SAVED_DUNGEON = null;
        Debug.Log("restarting dungeon!");
        //FindObjectOfType<DungeonGenerator>().DestroyAll();
        DungeonGenerator.genNewMesh = true;
        //Navigation.INSTANCE.SetActive(false);
        Destroy(DungeonGenerator.SAVED_DUNGEON);
        FindObjectOfType<DungeonGenerator>().Start();


    }

    private IEnumerator WaitForSceneLoad()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Battle");
        Destroy(spider);
        Navigation.INSTANCE.SetActive(false);
        // DungeonGenerator.SAVED_DUNGEON.SetActive(false);
    }
    private IEnumerator WaitForTownLoad()
    {
        Debug.Log("HERE???");
        yield return new WaitForSeconds(1);
            Debug.Log("BRO???");
            FindObjectOfType<DungeonGenerator>().Start();
    }

    private void ResetPlayer(){
        Debug.Log("HERE???");
        FindObjectOfType<DungeonGenerator>().DestroyAll();
        //Navigation.INSTANCE.SetActive(false);
        FindObjectOfType<DungeonGenerator>().Start();
        //Navigation.INSTANCE.SetActive(false);
        //FindObjectOfType<DungeonGenerator>().DestroyAll();
        //Navigation.INSTANCE.SetActive(false);
        //FindObjectOfType<DungeonGenerator>().Start();
    }
}