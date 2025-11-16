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
            if (!Application.isPlaying)
            {
                door.OpenDoor(door.openSpeed, true, true);
                return;
            }

            if (Application.isPlaying)
            {
                door.OpenDoor(door.openSpeed, true, false);
                return;
            }
        }

        if (GUILayout.Button("Close Door"))
        {
            if (!Application.isPlaying)
            {
                door.OpenDoor(door.openSpeed, false, true);
                return;
            }

            if (Application.isPlaying)
            {
                door.OpenDoor(door.openSpeed, false, false);
                return;
            }
        }

        if (GUILayout.Button("Clear Animation Queue"))
        {
            door.ClearAnimationQueue();
        }
    }
}