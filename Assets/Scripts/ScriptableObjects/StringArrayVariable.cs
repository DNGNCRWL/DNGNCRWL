using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "String Array", menuName = "Variables/String Array", order = 1)]
public class StringArrayVariable : ScriptableObject
{
    public string[] value;
}