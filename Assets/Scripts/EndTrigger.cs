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
        FindObjectOfType<DungeonGenerator>().Start();
    }
}
