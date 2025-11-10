using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        Door doorScript = (Door)target;
        
        if(GUILayout.Button("Open Door"))
        {
            doorScript.OpenDoor(true); 
        }
    }
}