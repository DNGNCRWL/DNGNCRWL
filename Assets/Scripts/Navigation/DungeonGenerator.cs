using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random=UnityEngine.Random;
using TMPro;
public class DungeonGenerator : MonoBehaviour
{
    // Tells us if the cell in the grid has been visited and which cell was open
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;

        public bool obligatory;

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn

            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;
            }

            return 0;
        }
    
    }
    public Transform GENtransform;
    public GameObject spider;
    public static Vector2Int SIZE = new Vector2Int(3,3);
    public int startPos = 0;

    public GameObject levelMessageWindow;
    public Rule[] rooms;
    public Vector2 offset;
    public NavMeshSurface[] surfaces;

    public static bool genNewMesh = true;
    public static bool wantSaved = false;

    public static GameObject SAVED_DUNGEON;
    public static bool NEW_DUNGEON = false;

    const int initialSeed = 1234;

    public static int LEVEL = 0;

    public static List<bool> keys = new List<bool>(){false};
    public static List<Random.State> SEEDS = new List<Random.State>();
    public static bool isSpider = true;

    List<Cell> board;
    List<int> tBoard;
    int[] seeds;

    // Start is called before the first frame update
    public void Start()
    {
        Debug.Log("back to start! LEVEL = " + LEVEL);
        navigatorKiller();
        collisionChecker();
        GameManager.GM.CheckLoadLootMenu();
    }

    public void BuildMesh()
    {
            for (int i = 0; i < surfaces.Length; i++)
            {
                surfaces[i].BuildNavMesh();

            }
        
    }
    public void Update()
    {
        if (genNewMesh)
        {
            for (int i = 0; i < surfaces.Length; i++)
            {
                surfaces[i].BuildNavMesh();
            }
            genNewMesh = false;
        }
    }

        // Creates dungeon using maze generator and update room
        Rule[] GenerateDungeon(GameObject enemy) {
            
        int ranSpider = Random.Range(0, tBoard.Count); // random spider locations

        for (int i = 0; i < SIZE.x; i++)
        {
            for (int j = 0; j < SIZE.y; j++)
            {
                Cell currentCell = board[(i + j * SIZE.x)];
                Debug.Log(tBoard[0] == (i + j * SIZE.x));
                if (currentCell.visited)
                {

                    int ran = Random.Range(2, rooms.Length);
                    int randomRoom = 0;
                    List<int> availableRooms = new List<int>();

                    if (randomRoom == -1)
                    {
                        if (availableRooms.Count > 0)
                        {
                            randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
                        }
                        else
                        {
                            randomRoom = 0;
                        }
                    }

                    if(i==0 && j==0){
                        var newRoom = Instantiate(rooms[0].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                        newRoom.UpdateRoom(currentCell.status);
                        newRoom.name = "First Room ";
                    }else if(tBoard.Contains(i + j * SIZE.x) && !(i + 1 == SIZE.x && j + 1 == SIZE.y)){
                        var newRoom = Instantiate(rooms[ran].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                        newRoom.UpdateRoom(currentCell.status);
                        newRoom.name = "End Room " + i + "-" + j;

                        if(isSpider && ranSpider == 0){
                            Debug.Log("tboard test");
                            BuildMesh();
                            GameObject spiderObject = Instantiate(enemy, new Vector3(newRoom.transform.position.x, newRoom.transform.position.y, newRoom.transform.position.z), Quaternion.identity, GENtransform);
                        }
                        ranSpider--;
                    }else if (i + 1 == SIZE.x && j + 1 == SIZE.y)
                    {
                        var newRoom = Instantiate(rooms[3].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                        newRoom.UpdateRoom(currentCell.status);
                        newRoom.name = "Up Stairs Room " + i + "-" + j;

                    }
                    else
                    {
                        var newRoom = Instantiate(rooms[1].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                        newRoom.UpdateRoom(currentCell.status);
                        newRoom.name = "Hall Room " + i + "-" + j;
                    }
                }
            }
        }
        BuildMesh();
        return rooms;

    }

    // creates our grid of cells
    async void MazeGenerator()
    {
        seedGenerator();

        board = new List<Cell>();
        tBoard = new List<int>();

        for (int i = 0; i < SIZE.x; i++)
        {
            for (int j = 0; j < SIZE.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;
        int endpoints = 0;
        while (k < 10000)
        {
            k++;
            Debug.Log(currentCell);
            board[currentCell].visited = true;

            if (currentCell == board.Count - 1)
            {
                //break;
            }

            //Check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0) //means no avaliable neigbors 
            {
                Debug.Log(path.Count);
                if (path.Count == 0) // break out loop b/c last cell
                {
                   
                    break;
                }
                else
                {
                    endpoints++;
                    if(endpoints==1){
                        tBoard.Add(currentCell);
                        Debug.Log("wanted endpoints " + currentCell);
                    }

                    Debug.Log("all endpoints " + currentCell);
                    currentCell = path.Pop(); //adds last cell to our path
                }
            }
            else
            {
                endpoints=0;
                path.Push(currentCell);
                int newCell = neighbors[Random.Range(0, neighbors.Count)]; // chooses randomn neighbor

                if (newCell > currentCell)
                {
                    //down or right
                    if (newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    //up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }

            }

        }
        GenerateDungeon(spider);
    }

    //Returns a list of all neighbors
    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - SIZE.x >= 0 && !board[(cell - SIZE.x)].visited)
        {
            neighbors.Add((cell - SIZE.x));
        }

        //check down neighbor
        if (cell + SIZE.x < board.Count && !board[(cell + SIZE.x)].visited)
        {
            neighbors.Add((cell + SIZE.x));
        }

        //check right neighbor
        if ((cell + 1) % SIZE.x != 0 && !board[(cell + 1)].visited)
        {
            neighbors.Add((cell + 1));
        }

        //check left neighbor
        if (cell % SIZE.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell - 1));
        }

        return neighbors;
    }

    // Destroys the dungeon rooms
    public void DestroyAll()
    {
        GameObject.Destroy(GameObject.FindWithTag("Spider"));
        foreach (Transform child in transform) 
        {
            GameObject.Destroy(child.gameObject);
        }
        NEW_DUNGEON = true;
    }

    public void LevelMessage(string message) {
        levelMessageWindow.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
    }

     public void getBossMessage() {
        LevelMessage("Defeat The Spider");
        StartCoroutine(waiter(levelMessageWindow));
    }
    public void getLevel() {
        LevelMessage("Level "+ LEVEL.ToString());
        StartCoroutine(waiter(levelMessageWindow));
    }
    public void setSIZEUp() {
        SIZE.x = SIZE.x + 2;
        SIZE.y = SIZE.y + 2;
    }

    public void setSIZEDown() {
        SIZE.x = SIZE.x - 2;
        SIZE.y = SIZE.y - 2;
    }

    // Checks if player is going up or down stairs
    public void collisionChecker() {
         if(EndTrigger.UPSTAIRCOLLISION){
            Debug.Log("upstair!");
            //SAVED_DUNGEON = gameObject;
            // Navigation.INSTANCE.SetActive(true);
            Debug.Log("NAV?? " + Navigation.INSTANCE);
            Navigation.INSTANCE.transform.position = new Vector3(0,1,0);
            //DontDestroyOnLoad(SAVED_DUNGEON);
            EndTrigger.UPSTAIRCOLLISION = false;
            MazeGenerator();
        }
        else if(EndTrigger.DOWNSTAIRCOLLISION){
            Debug.Log("saved Dungeon");
            //SAVED_DUNGEON.SetActive(true);
            // Navigation.INSTANCE.SetActive(true);
            Navigation.INSTANCE.transform.position = new Vector3(0,1,0);
            //Destroy(gameObject);
            EndTrigger.DOWNSTAIRCOLLISION = false;
            MazeGenerator(); 
        }else{
            Debug.Log("nice!");
            Navigation.INSTANCE.SetActive(true);
            SAVED_DUNGEON = gameObject;
            MazeGenerator();
        }
    }

    // Destroys navigator after battle
    public void navigatorKiller() {
        Navigation.INSTANCE.SetActive(true);
        SceneManager.MoveGameObjectToScene(Navigation.INSTANCE, SceneManager.GetActiveScene());
    }

    // Generates a seed in order to save Dungeon level
    public void seedGenerator() {
        if(LEVEL==0){ //load previous
            Debug.Log("AA seed count 0");
            Random.InitState(initialSeed);
            //load second seed instantly (TRY SWITCHING FROM ENDTRIGGER)
            // Random.State tmp = Random.state;
            // LEVEL.Add(tmp);
        }
        else if(LEVEL <= SEEDS.Count){
            Debug.Log("AA here we are!");
            Random.state = SEEDS[LEVEL-1];
        }
        else{ //new level
            Debug.Log("AA seed count > 0");
            Random.State tmp = Random.state;
            SEEDS.Add(tmp);
            Random.state = tmp;
        }
        Debug.Log("AA passed seed check!");
    }

    IEnumerator waiter(GameObject obj)
    {

        //Wait for 4 seconds
        obj.SetActive(true);
        yield return new WaitForSeconds(2f);
        obj.SetActive(false);
    }

}