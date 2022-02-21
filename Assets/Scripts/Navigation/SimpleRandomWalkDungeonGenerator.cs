using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    protected SimpleRandomWalkParameters parameters;

    protected override void RunProceduralGeneration(){
        HashSet<Vector2Int> floorPositions = RunRandomWalk(parameters, startPosition);
        dungeonVisualizer.ClearMap();
        dungeonVisualizer.CreateFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, dungeonVisualizer);
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkParameters parameters, Vector2Int position)
    {
        Vector2Int currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

        for(int i = 0; i < parameters.iterations; i++){
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walklength);
            floorPositions.UnionWith(path);
            if(parameters.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }

        return floorPositions;
    }
}
