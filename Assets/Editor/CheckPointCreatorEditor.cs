using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CheckPointCreator))]
public class CheckPointCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CheckPointCreator myScript = (CheckPointCreator)target;
        if (GUILayout.Button("Create Checkpoints"))
        {
            myScript.CreateCheckpoints();
        }
    }
}
