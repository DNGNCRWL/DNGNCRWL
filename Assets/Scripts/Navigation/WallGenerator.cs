using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, DungeonVisualizer dungeonVisualizer){
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionsList);

        foreach(var position in basicWallPositions){
            dungeonVisualizer.PaintSingleBasicWall(position);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> cardinalDirectionsList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        foreach(var position in floorPositions){
            foreach(var direction in cardinalDirectionsList){
                var neighborPosition = position + direction;
                if(!floorPositions.Contains(neighborPosition)){
                    wallPositions.Add(neighborPosition);
                }
            }
        }

        return wallPositions;
    }
}
