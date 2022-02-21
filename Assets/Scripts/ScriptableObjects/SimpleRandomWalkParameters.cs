using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Simple Random Walk Parameters", menuName = "Proc Gen/Simple Random Walk Parameters", order = 1)]
public class SimpleRandomWalkParameters : ScriptableObject
{
    public int iterations = 10, walklength = 10;
    public bool startRandomlyEachIteration = true;
}
