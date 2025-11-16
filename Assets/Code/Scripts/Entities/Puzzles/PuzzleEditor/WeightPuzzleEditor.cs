using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeightPuzzle))]
public class WeightPuzzleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        WeightPuzzle puzzle = (WeightPuzzle)target;
        
        if (GUILayout.Button("First Weight Test"))
        {
            TestWeight(puzzle.Weights[0]);
        }
        
        if (GUILayout.Button("Second Weight Test"))
        {
            TestWeight(puzzle.Weights[1]);
        }
    }

    private void TestWeight(Weight weight)
    {
        weight.OnShot(weight.Weaknesses[0], weight.Weaknesses[0].WeakType);
    }
}