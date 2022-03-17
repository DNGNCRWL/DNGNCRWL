using UnityEngine;
using UnityEngine.SceneManagement;
public class EndTrigger : MonoBehaviour
{
    public bool gameHasEnded = false;
    public float dungeonGenerationDelay = 4f;
    public GameManager gameManager;
    public static bool COLLIDE;
    public void OnTriggerEnter (Collider collider)
    {
        Debug.Log(gameObject.name + " has triggered end by " + collider.gameObject.name);
         if(gameHasEnded == false)
        {
            //           GetComponent<Collider>().attachedRigidbody.useGravity = false;
         //   if(gameObject.name.Equals("stairwell"))
         //   {
//
           //     Debug.Log("stair~");
           //     SceneManager.LoadScene("Town");
           // }
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
