using System;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        Door door = (Door)target;
        
        if (GUILayout.Button("Open Door"))
        {
            door.OpenDoor();
        }

        if (GUILayout.Button("Close Door"))
        {
            door.CloseDoor();
        }
    }
}