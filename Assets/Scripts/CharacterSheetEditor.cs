using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterSheet), true)]
public class CharacterSheetEditor : Editor
{
    CharacterSheet charSheet;

    private void Awake(){
        charSheet = (CharacterSheet)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Initialize"))
            charSheet.InitializeCharacter();
        if(GUILayout.Button("Random Classless"))
            charSheet.InitializeRandomClassless();
    }
}
