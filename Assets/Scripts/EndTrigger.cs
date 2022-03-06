using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    public bool gameHasEnded = false;
    public float dungeonGenerationDelay = 4f;
    public GameManager gameManager;
    public void OnTriggerEnter ()
    {
         if(gameHasEnded == false)
        {
            //           GetComponent<Collider>().attachedRigidbody.useGravity = false;
            gameHasEnded = true;
            gameManager.CompleteLevel();
            Invoke("RestartGenerateDungeon", dungeonGenerationDelay);
            gameHasEnded = false;
            
        }

    }
    public void RestartGenerateDungeon()
    {
        FindObjectOfType<DungeonGenerator>().Restarter();
        DungeonGenerator.genNewMesh = true;
        FindObjectOfType<Navigation>().respawn();
        FindObjectOfType<DungeonGenerator>().Start();

/* 
 *      if (DungeonGenerator.i == 0)
        {
            DungeonGenerator.i++;
            FindObjectOfType<DungeonGenerator>().Start();
        }
        else if (DungeonGenerator.i == 1)
        {
            DungeonGenerator.wantSaved = true;
            DungeonGenerator.i++;
            FindObjectOfType<DungeonGenerator>().Start();
        }
        else if(DungeonGenerator.i == 2)
        {
            DungeonGenerator.genSaved = true;
            DungeonGenerator.i++;
            FindObjectOfType<DungeonGenerator>().Start();
        }
*/
        
        
    }
}
