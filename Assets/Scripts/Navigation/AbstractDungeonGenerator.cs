using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected DungeonVisualizer dungeonVisualizer;

    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    public void GenerateDungeon(){
        RunProceduralGeneration();
    }

    public void Clear(){
        dungeonVisualizer.ClearMap();
    }

    protected abstract void RunProceduralGeneration();
}
