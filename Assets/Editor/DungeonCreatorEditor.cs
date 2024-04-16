using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonManager))]
public class DungeonCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DungeonManager dungeonCreator = (DungeonManager)target;
        if (GUILayout.Button("CreateNewDungeon"))
        {
            dungeonCreator.CreateDungeon();
        }
    }
}
