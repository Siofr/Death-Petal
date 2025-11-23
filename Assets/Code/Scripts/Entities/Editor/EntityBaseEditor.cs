using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntityBase), true)]
public class EntityBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Initialise Weights"))
        {
            EntityBase entityBase = (EntityBase) target;
            
            entityBase.InitialiseWeaknesses();
        }
    }
}