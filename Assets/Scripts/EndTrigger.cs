using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    public bool gameHasEnded = false;
    public float dungeonGenerationDelay = 1f;
    public GameManager gameManager;
    public void OnTriggerEnter ()
    {
         if(gameHasEnded == false)
        {
            gameHasEnded = true;
            gameManager.CompleteLevel();
            Invoke("RestartGenerateDungeon", dungeonGenerationDelay);
            
        }

    }
    public void RestartGenerateDungeon()
    {
        FindObjectOfType<DungeonGenerator>().Restarter();
        FindObjectOfType<DungeonGenerator>().Start();
    }
}
