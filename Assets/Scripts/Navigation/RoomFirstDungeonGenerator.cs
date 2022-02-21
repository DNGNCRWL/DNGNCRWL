using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{

    [SerializeField]
    private int minRoomWidth = 3, minRoomHeight = 3;

    [SerializeField]
    private int dungeonWidth = 32, dungeonHeight = 32;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;

    [SerializeField]
    [Range(0, 1f)]
    public float corridorWobble;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms(){
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if(randomWalkRooms)
            floor = CreateRoomsRandomly(roomsList);
        else
            floor = CreateSimpleRooms(roomsList);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList){
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        dungeonVisualizer.ClearMap();
        dungeonVisualizer.CreateFloorTiles(floor);
        WallGenerator.CreateWalls(floor, dungeonVisualizer);
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for(int i = 0; i < roomsList.Count; i++){
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(parameters, roomCenter);   
            foreach(var position in roomFloor){
                if(
                    position.x >= (roomBounds.xMin + offset) &&
                    position.x <= (roomBounds.xMax -offset) &&
                    position.y >= (roomBounds.yMin + offset) &&
                    position.y <= (roomBounds.yMax -offset))
                    floor.Add(position);
            }         
        }

        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while(roomCenters.Count > 0){
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int closest)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        Vector2Int position = currentRoomCenter;
        corridor.Add(position);

        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        
        while(position != closest){
            if(Random.value < corridorWobble)
                possibleMoves.AddRange(Direction2D.cardinalDirectionsList);
                
            if(closest.y > position.y)
                possibleMoves.Add(Vector2Int.up);
            else if(closest.y < position.y){
                possibleMoves.Add(Vector2Int.down);
            }
            if(closest.x > position.x)
                possibleMoves.Add(Vector2Int.right);
            else if(closest.x < position.x){
                possibleMoves.Add(Vector2Int.left);
            }

            position += possibleMoves[Random.Range(0, possibleMoves.Count)];
            possibleMoves.Clear();

            corridor.Add(position);
        }

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float minDistance = float.MaxValue;

        foreach(Vector2Int center in roomCenters){
            float currentDistance = Vector2.Distance(center, currentRoomCenter);
            if(currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closest = center;
            }
        }

        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList){
            for(int col = offset; col < room.size.x - offset; col++){
                for(int row = offset; row < room.size.y - offset; row++){
                    Vector2Int pos = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(pos);
                }
            }
        }
        return floor;
    }
}
