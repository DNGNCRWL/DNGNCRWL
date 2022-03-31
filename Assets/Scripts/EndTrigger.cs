using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;

public class EndTrigger : MonoBehaviour
{
    public bool gameHasEnded = false;
    public float dungeonGenerationDelay = 1f;
    public GameManager gameManager;
    public EnemyEncounter[] boss_encounters;
    public static bool COLLIDE;
    public GameObject spider;
    public void OnTriggerEnter (Collider collider)
    {
        Debug.Log(gameObject.name + " has triggered end by " + collider.gameObject.name);
         if(gameHasEnded == false)
        {
            //           GetComponent<Collider>().attachedRigidbody.useGravity = false;
            if(gameObject.name.Equals("stairwell"))
            {

                Debug.Log("stair!");
                SceneManager.LoadScene("Town");
                Invoke("RestartGenerateDungeon",0.1f);

                return;
            }
            if (gameObject.name.Equals("remodel_tarantula(export) Variant(Clone)"))
            {
                EnemyEncounter boss_encounter = boss_encounters[Random.Range(0, boss_encounters.Length)];
                BattleManager.SetENEMY_ENCOUNTER(boss_encounter);
                DungeonGenerator.SAVED_DUNGEON.SetActive(false);
                SceneManager.LoadScene("Battle");
                Destroy(spider);
                return;
            }
            gameHasEnded = true;
            COLLIDE = true;
            //gameManager.CompleteLevel();
            Debug.Log("endtrigger!");
            GameManager.GM.CompleteLevel();
            Invoke("RestartGenerateDungeon", dungeonGenerationDelay);
            gameHasEnded = false;
            
        }

    }
    public void RestartGenerateDungeon()
    {
        //SceneManager.LoadScene("Town");
        //DungeonGenerator.SAVED_DUNGEON = null;
        FindObjectOfType<DungeonGenerator>().Restarter();
        DungeonGenerator.genNewMesh = true;
        FindObjectOfType<Navigation>().respawn();
        FindObjectOfType<DungeonGenerator>().Start();

    }

}
