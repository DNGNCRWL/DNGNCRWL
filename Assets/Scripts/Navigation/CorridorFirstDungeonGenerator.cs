using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int corridorLengthMin = 2, corridorLengthMax = 5, corridorCount = 5;
    [SerializeField]
    [Range(0.01f, 1)]
    private float roomPercent = 0.8f;
    [SerializeField]
    private bool generateRoomsAtDeadEnds = false;

    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();
        
        CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
        if(generateRoomsAtDeadEnds)
            CreateRoomsAtDeadEnds(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        dungeonVisualizer.ClearMap();
        dungeonVisualizer.CreateFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, dungeonVisualizer);
    }

    private void CreateRoomsAtDeadEnds(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomPositions)
    {
        int count = 0;

        foreach(var position in deadEnds){
            if(!roomPositions.Contains(position)){
                var room = RunRandomWalk(parameters, position);
                roomPositions.UnionWith(room);
                count += room.Count;
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int>deadEnds = new List<Vector2Int>();

        foreach(var position in floorPositions){
            int neighborCount = 0;

            foreach(var direction in Direction2D.cardinalDirectionsList){
                if(floorPositions.Contains(position + direction))
                    neighborCount ++;
            }

            if(neighborCount ==1)
                deadEnds.Add(position);
        }

        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();

        int howManyToCreate = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);
        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(howManyToCreate).ToList();

        foreach (var roomPosition in roomsToCreate){
            var roomFloor = RunRandomWalk(parameters, roomPosition);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);

        for(int i = 0; i < corridorCount; i++){
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, Random.Range(corridorLengthMin, corridorLengthMax));
            currentPosition = corridor[corridor.Count-1];
            potentialRoomPositions.Add(currentPosition);

            

            floorPositions.UnionWith(corridor);
        }
    }
}
