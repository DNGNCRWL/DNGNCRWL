using UnityEngine;
using System.Collections;

[System.Serializable]
public struct MessagePackage
{
    public string[] messages;
    public float time;
    [SerializeField, Range(0, 1)]
    public float weight;
    [HideInInspector] public int index;
}