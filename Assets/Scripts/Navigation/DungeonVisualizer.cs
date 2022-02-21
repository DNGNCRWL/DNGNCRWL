using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonVisualizer : MonoBehaviour
{
    public static int GRID_SIZE = 4;

    Dictionary<Vector2Int, GameObject> floorTilemap, wallTilemap;

    [SerializeField]
    private GameObject floorTile;
    [SerializeField]
    private GameObject wallTile;

    public void ClearMap()
    {
        while (transform.childCount > 0)
        {
            GameObject tile = transform.GetChild(0).gameObject;
            if (tile)
            {
                if (Application.isEditor)
                    DestroyImmediate(tile);
                else
                    Destroy(tile);
            }
        }

        floorTilemap = new Dictionary<Vector2Int, GameObject>();
        wallTilemap = new Dictionary<Vector2Int, GameObject>();
    }

    internal void PaintSingleBasicWall(Vector2Int position)
    {
        PaintSingleTile(wallTilemap, wallTile, position);
    }

    public void CreateFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Dictionary<Vector2Int, GameObject> map, GameObject tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(map, tile, position);
        }
    }

    private void PaintSingleTile(Dictionary<Vector2Int, GameObject> map, GameObject tile, Vector2Int position)
    {
        Vector3 spawnPoint = new Vector3(position.x * GRID_SIZE, 0, position.y * GRID_SIZE);
        GameObject spawned = Instantiate(tile, spawnPoint, Quaternion.identity, transform);
        map.Add(position, spawned);
    }
}
