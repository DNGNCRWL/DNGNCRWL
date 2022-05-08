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

    public static bool BOSSBOOL;
    public static bool TEAMWIN;
    //private float delay = 10f;
    //private float timeElapsed;
    public void OnTriggerEnter(Collider collider)
    {
        Debug.Log(gameObject.name + " has triggered end by " + collider.gameObject.name);
        if (gameHasEnded == false)
        {
            // if(gameObject.name.Equals("box"))
            // {
            //     FindObjectOfType<Chest>().fillChest();
            //     Debug.Log("inside box");
            // }
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
                    FindObjectOfType<DungeonGenerator>().getBoss();
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
                    
                    // FindObjectOfType<DungeonGenerator>().setSizeDown();
                    Invoke("ResetPlayer", .1f);
                }


                return;
            }
            if (gameObject.name.Equals("remodel_tarantula(export) Variant(Clone)"))
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
        Navigation.INSTANCE.SetActive(false);
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

    private void setInactive(){
        Navigation.INSTANCE.SetActive(false); 
        Debug.Log("setting inactive");
    }
    
}