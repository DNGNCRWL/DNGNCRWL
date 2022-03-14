using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    public Item key;

    List<Cell> board;

    // Start is called before the first frame update
    
    public void Start()
    {
        if (!SAVED_DUNGEON)
        {
            MazeGenerator();
            SAVED_DUNGEON = gameObject;
            DontDestroyOnLoad(SAVED_DUNGEON);
        }
        else
        {
            SAVED_DUNGEON.SetActive(true);
        }

    }

    public void BuildMesh()
    {
        for (int i = 0; i < surfaces.Length; i++)
        {
            for (int i = 0; i < surfaces.Length; i++)
            {
                surfaces[i].BuildNavMesh();

            }
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

    void SaveDungeon()
    {
        saved = GenerateDungeon(spider);
    }

    void GenerateSavedDungeon(Rule[] saved)
    {
        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];
                if (currentCell.visited)
                {
                    int randomRoom = -1;
                    List<int> availableRooms = new List<int>();

                    for (int k = 0; k < saved.Length; k++)
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


                    var newRoom = Instantiate(saved[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += " " + i + "-" + j;
                }
            }
        }
    }

        Rule[] GenerateDungeon(GameObject enemy)
    {

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];
                if (currentCell.visited)
                {
                    int randomRoom = Random.Range(0, rooms.Length);
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


                    var newRoom = Instantiate(rooms[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += " " + i + "-" + j;
                    if (i+1 == size.x && j+1 == size.y)
                    {
                        BuildMesh();
                        GameObject spider = Instantiate(enemy, new Vector3(newRoom.transform.position.x, newRoom.transform.position.y, newRoom.transform.position.z), Quaternion.identity);
                        FindObjectOfType<EndTrigger>().gameHasEnded = false;
                    }
                }
            }
        }
        
        return rooms;

    }

    void MazeGenerator()
    {
        key = key.Copy();
        board = new List<Cell>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;

        while (k < 10000)
        {
            k++;

            board[currentCell].visited = true;

            if (currentCell == board.Count - 1)
            {
                break;
            }

            //Check the cell's neighbors
            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
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
        if (genSaved)
        {
            GenerateSavedDungeon(saved);
        }
        else if(wantSaved)
        {
            saved = GenerateDungeon(spider);
        }
        else
        {
            GenerateDungeon(spider);
        }

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

    public void Restarter()
    {
        GameObject.Destroy(GameObject.FindWithTag("Spider"));
        foreach (Transform child in transform) 
        {
            GameObject.Destroy(child.gameObject);
        }

        //GameObject.Destroy(spider.gameObject);
        //GameObject.Destroy(GameObject.FindWithTag("Spider").transform);
    }
    
}