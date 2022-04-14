using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random=UnityEngine.Random;
public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        // public GameObject[] endRooms;
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
    public Vector2Int size;
    public int startPos = 0;
    public Rule[] rooms;
    public Rule[] saved;
    public Vector2 offset;
    public NavMeshSurface[] surfaces;

    public static bool genNewMesh = true;
    public static bool genSaved = false;
    public static bool wantSaved = false;
    public static int i = 0;

    public static GameObject SAVED_DUNGEON;
    public static bool NEW_DUNGEON = false;

    const int initialSeed = 1234;

    public static int LEVEL = 0;
    public static List<Random.State> SEEDS = new List<Random.State>();

    //public Item key;

    List<Cell> board;
    List<int> tBoard;
    int[] seeds;

    // Start is called before the first frame update
    
    public void Start()
    {
        Debug.Log("back to start! LEVEL = " + LEVEL);
        //Navigation.INSTANCE.SetActive(false);
        if(EndTrigger.UPSTAIRCOLLISION){
            Debug.Log("upstair!");
            //SAVED_DUNGEON = gameObject;
            Navigation.INSTANCE.SetActive(true);
            Debug.Log("NAV?? " + Navigation.INSTANCE);
            Navigation.INSTANCE.transform.position = new Vector3(0,1,0);
            //DontDestroyOnLoad(SAVED_DUNGEON);
            EndTrigger.UPSTAIRCOLLISION = false;
            MazeGenerator();
        }
        else if(EndTrigger.DOWNSTAIRCOLLISION){
            Debug.Log("saved Dungeon");
            //SAVED_DUNGEON.SetActive(true);
            Navigation.INSTANCE.SetActive(true);
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
        //Navigation.INSTANCE.SetActive(true);
        // if (!EndTrigger.COLLIDE) //new level, boss collision
        // {
        //     if (!SAVED_DUNGEON)
        //     {
        //         Debug.Log("No saved Dunegon, now save");
        //         MazeGenerator();
        //         SAVED_DUNGEON = gameObject;
        //         Navigation.INSTANCE.SetActive(true);
        //         Navigation.INSTANCE.transform.position = new Vector3(0,1,0);
        //         DontDestroyOnLoad(SAVED_DUNGEON);
        //     }
        //     else
        //     {
        //         Debug.Log("saved Dungeon");
        //         SAVED_DUNGEON.SetActive(true);
        //         Navigation.INSTANCE.SetActive(true);
        //         Destroy(gameObject);
        //         //MazeGenerator(); 
        //     }
        // }
        // else
        // {
        //     Debug.Log("new level");
        //     EndTrigger.COLLIDE = false;
        //     MazeGenerator();
        //     SAVED_DUNGEON = gameObject;
        //     DontDestroyOnLoad(SAVED_DUNGEON);
        //     SceneManager.LoadScene("Town");
        // }
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

    // void GenerateSavedDungeon(Rule[] saved, GameObject enemy)
    // {
    //     for (int i = 0; i < surfaces.Length; i++)
    //     {
    //         surfaces[i].BuildNavMesh();
    //     }
    //     for (int i = 0; i < size.x; i++)
    //     {
    //         for (int j = 0; j < size.y; j++)
    //         {
    //             Cell currentCell = board[(i + j * size.x)];
    //             if (currentCell.visited)
    //             {
    //                 int randomRoom = -1;
    //                 List<int> availableRooms = new List<int>();

    //                 for (int k = 0; k < saved.Length; k++)
    //                 {
    //                     int p = rooms[k].ProbabilityOfSpawning(i, j);

    //                     if (p == 2)
    //                     {
    //                         randomRoom = k;
    //                         break;
    //                     }
    //                     else if (p == 1)
    //                     {
    //                         availableRooms.Add(k);
    //                     }
    //                 }

    //                 if (randomRoom == -1)
    //                 {
    //                     if (availableRooms.Count > 0)
    //                     {
    //                         randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
    //                     }
    //                     else
    //                     {
    //                         randomRoom = 0;
    //                     }
    //                 }


    //                 var newRoom = Instantiate(saved[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
    //                 newRoom.UpdateRoom(currentCell.status);
    //                 newRoom.name += " " + i + "-" + j;
    //                 if (i + 1 == size.x && j + 1 == size.y)
    //                 {
    //                     BuildMesh();
    //                     GameObject spider = Instantiate(enemy, new Vector3(newRoom.transform.position.x, newRoom.transform.position.y, newRoom.transform.position.z), Quaternion.identity);
    //                     //FindObjectOfType<EndTrigger>().gameHasEnded = false;
    //                 }
    //             }
    //         }
    //     }
    // }

        Rule[] GenerateDungeon(GameObject enemy)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];
                Debug.Log(tBoard[0] == (i + j * size.x));
                if (currentCell.visited)
                {

                    // int randomRoom = Random.Range(0, rooms.Length);
                    int randomRoom = 0;
                    List<int> availableRooms = new List<int>();
                    for (int k = 0; k < rooms.Length; k++)
                    {
                        int p = rooms[k].ProbabilityOfSpawning(i, j);

                        if (p == 2)
                        {
                            randomRoom = k;
                            break;
                        }
                        else if (p == 1)
                        {
                            availableRooms.Add(k);
                        }
                    }

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
                    if(tBoard.Contains(i + j * size.x) && !(i + 1 == size.x && j + 1 == size.y)){
                       
                        var newRoom = Instantiate(rooms[1].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                        newRoom.UpdateRoom(currentCell.status);
                        newRoom.name += " " + i + "-" + j;
                    }else if (i + 1 == size.x && j + 1 == size.y)
                    {
                        var newRoom = Instantiate(rooms[2].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                        newRoom.UpdateRoom(currentCell.status);
                        newRoom.name += " " + i + "-" + j;
                        BuildMesh();
                        GameObject spider = Instantiate(enemy, new Vector3(newRoom.transform.position.x, newRoom.transform.position.y, newRoom.transform.position.z), Quaternion.identity, GENtransform);
                        //FindObjectOfType<EndTrigger>().gameHasEnded = false;

                    }
                    else
                    {
                        var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                        newRoom.UpdateRoom(currentCell.status);
                        newRoom.name += " " + i + "-" + j;
                    }
                }
            }
        }
        return rooms;

    }

    async void MazeGenerator()
    {
        //key = key.Copy();
        //seeds = new List<int>();
        //seeds[0] = initialSeed;
        Debug.Log("SEED COUNT!!!" + SEEDS.Count);
        if(LEVEL==0){ //load previous
            Debug.Log("seed count 0");
            Random.InitState(initialSeed);
            //load second seed instantly (TRY SWITCHING FROM ENDTRIGGER)
            // Random.State tmp = Random.state;
            // LEVEL.Add(tmp);
        }
        else if(LEVEL < SEEDS.Count){
            Random.state = SEEDS[LEVEL-1];
        }
        else{ //new level
            Debug.Log("seed count > 0");
            Random.State tmp = Random.state;
            SEEDS.Add(tmp);
            Random.state = tmp;
        }
        Debug.Log("passed seed check!");

        board = new List<Cell>();
        tBoard = new List<int>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }


        int currentCell = startPos;

        Stack<int> path = new Stack<int>();
        Stack<int> tPath = new Stack<int>();

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

            if (neighbors.Count == 0)
            {
                Debug.Log(path.Count);
                if (path.Count == 0)
                {
                    //tPath.Push(currentCell);
                    //Debug.Log(currentCell);
                   
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
                    currentCell = path.Pop();
                    
                   
                    //GenerateDungeon(spider);
                    //break;
                }
            }
            else
            {
                endpoints=0;
                path.Push(currentCell);
                int newCell = neighbors[Random.Range(0, neighbors.Count)];

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
        //saved = GenerateDungeon(spider);
        // if (genSaved)
        // {
        //     GenerateSavedDungeon(saved, spider);
        // }
        // else if(wantSaved)
        // {
        //     saved = GenerateDungeon(spider);
        // }
        // else
        // {
        //     GenerateDungeon(spider);
        // }
        GenerateDungeon(spider);
      //  saved = GenerateDungeon(spider);
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //check up neighbor
        if (cell - size.x >= 0 && !board[(cell - size.x)].visited)
        {
            neighbors.Add((cell - size.x));
        }

        //check down neighbor
        if (cell + size.x < board.Count && !board[(cell + size.x)].visited)
        {
            neighbors.Add((cell + size.x));
        }

        //check right neighbor
        if ((cell + 1) % size.x != 0 && !board[(cell + 1)].visited)
        {
            neighbors.Add((cell + 1));
        }

        //check left neighbor
        if (cell % size.x != 0 && !board[(cell - 1)].visited)
        {
            neighbors.Add((cell - 1));
        }

        return neighbors;
    }

    public void DestroyAll()
    {
        GameObject.Destroy(GameObject.FindWithTag("Spider"));
        foreach (Transform child in transform) 
        {
            GameObject.Destroy(child.gameObject);
        }
        NEW_DUNGEON = true;

        //GameObject.Destroy(spider.gameObject);
        //GameObject.Destroy(GameObject.FindWithTag("Spider").transform);
    }
    //     public void GenEnd()
    // {
    //      for (int i = 0; i < rooms.Length; i++)
    //     {
    //         if (rooms[i].endRooms == null)
    //         {
    //             rooms[i].endRooms = GameObject.FindGameObjectsWithTag("endroom");
    //         }
    //     }
    // }
    public void Clear(){
        SceneManager.LoadScene(SceneManager.GetActiveScene);
    }
}