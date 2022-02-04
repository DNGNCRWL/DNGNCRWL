using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryptGenerator : MonoBehaviour
{
    public struct Cell {
        public CellType cellType;
        public WallState north, west, south, east;
        public Vector2 direction;
    }

    public enum CellType
    {
        Empty = 0,
        NotEmpty = 1,
        Upstairs = 2,
        Downstairs = 3,
        ClosedDoor = 4,
        OpenDoor = 5
    };
    public enum WallState {Open, Wall, FakeWall, Door}

    public GameObject nav;
    public int width;
    public int depth;
    readonly static int GRID_SIZE = 4;
    public bool randomizeStart;

    public GameObject ground, stairs;

    public Cell[,] cells;

    void Awake(){
        if(!randomizeStart)
            return;

        cells = new Cell[depth, width];
        GenerateCrypt();
        BuildCrypt();
    }

    void GenerateCrypt(){
        Vector2 startCoord = new Vector2(Random.Range(0, depth), Random.Range(0,width));
        Cell startCell = new Cell();
        startCell.cellType = CellType.NotEmpty;
        AddCell(startCoord, startCell);

        if(randomizeStart){
            nav.transform.position = new Vector3(startCoord.x * GRID_SIZE, 1.75f, startCoord.y * GRID_SIZE);
            nav.transform.localEulerAngles = new Vector3(0, Random.Range(0, 4) * 90, 0);
        }

        Cell stairsCell = new Cell();

        bool addStairs = false;
        while(!addStairs){
            stairsCell.cellType = CellType.Upstairs;
            addStairs = AddCell(startCoord, RandomDirection(), stairsCell);
        }

        for(int i = 0; i < 10; i++){
            RandomRoom(
                Mathf.Max(2, Random.Range(0, i)),
                Mathf.Max(2, Random.Range(0, i))
                );
        }

    }

    static Vector2 RandomDirection(){
        int direction = Random.Range(0, 4);
        switch(direction){
            case 0: return new Vector2(1, 0);
            case 1: return new Vector2(0, 1);
            case 2: return new Vector2(-1, 0);
            case 3: return new Vector2(0, -1);
        }

        Debug.Log("lol what's happening?");
        return Vector2.zero;
    }

    bool AddCell(Vector2 coord, Cell c){
        if( coord.y < 0 ||
            coord.y >= depth ||
            coord.x < 0 ||
            coord.x >= width)
            return false;

        cells[(int)coord.y, (int)coord.x] = c;
        return true;
    }

    bool RandomRoom(int dimension, int dimension2){
        int x = dimension;
        int y = dimension2;
        if(Random.Range(0, 2)>0){
            x = dimension2;
            y = dimension;
        }

        Vector2 randomCoord = new Vector2(Random.Range(0, depth-y), Random.Range(0,width-x));
        Vector2 randomCoord2 = randomCoord + new Vector2(x, y);

        return AddRoom(randomCoord, randomCoord2);
    }

    bool AddRoom(Vector2 coord, Vector2 coord2){
        if( coord.y < 0 ||
            coord.y >= depth ||
            coord.x < 0 ||
            coord.x >= width)
            return false;

        if( coord2.y < 0 ||
            coord2.y >= depth ||
            coord2.x < 0 ||
            coord2.x >= width)
            return false;

        int minX = (int)Mathf.Min(coord.x, coord2.x);
        int maxX = (int)Mathf.Max(coord.x, coord2.x);
        int minY = (int)Mathf.Min(coord.y, coord2.y);
        int maxY = (int)Mathf.Max(coord.y, coord2.y);

        for(int y = minY; y <= maxY; y++){
            for(int x = minX; x <= maxX; x++){
                Cell current = new Cell();
                current.cellType = CellType.NotEmpty;
                cells[y, x] = current;
            }
        }

        return true;
    }

    bool AddCell(Vector2 coord, Vector2 direction, Cell cell){
        cell.direction = direction;
        return AddCell(coord + direction, cell);
    }

    void BuildCrypt(){
        for(int y = 0; y < depth; y++){
            for(int x = 0; x < width; x++){
                Cell current = cells[y, x];
                if(current.cellType != CellType.Empty){
                    BuildCell(x, y, current);
                }
            }
        }
    }

    void BuildCell(int x, int y, Cell cell){
        Vector3 location = new Vector3(x * GRID_SIZE, 0, y * GRID_SIZE);

        switch(cell.cellType){
            case CellType.Upstairs: 
                BuildUpStairs(location, cell);
                break;
            case CellType.Downstairs:
                BuildDownStairs(location, cell);
                break;
            default:
                Instantiate(ground, location, Quaternion.identity, transform);
                break;
        }
    }

    void BuildUpStairs(Vector3 location, Cell cell){
        BuildObject(location, cell, stairs);
    }
    void BuildDownStairs(Vector3 location, Cell cell){
        BuildObject(location + Vector3.down * GRID_SIZE, cell, stairs);
    }

    void BuildObject(Vector3 location, Cell cell, GameObject toBuild){
        GameObject nToBuild = Instantiate(toBuild, location, Quaternion.identity, transform);

        switch(cell.direction){
            case Vector2 v when v.Equals(Vector2.up):
                nToBuild.transform.localEulerAngles = new Vector3(0, 180, 0);
                break;
            case Vector2 v when v.Equals(Vector2.down):
                nToBuild.transform.localEulerAngles = new Vector3(0, 0, 0);
                break;
            case Vector2 v when v.Equals(Vector2.left):
                nToBuild.transform.localEulerAngles = new Vector3(0, 90, 0);
                break;
            case Vector2 v when v.Equals(Vector2.right):
                nToBuild.transform.localEulerAngles = new Vector3(0, -90, 0);
                break;
            default:
                break;
        }
    }

}
