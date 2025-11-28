using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
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

        if (GUILayout.Button("First Weight Fail Test"))
        {
            TestWeight(puzzle.Weights[0], true);
        }
        
        if (GUILayout.Button("Second Weight Fail Test"))
        {
            TestWeight(puzzle.Weights[1], true);
        }
    }

    private void TestWeight(Weight weight, bool isFail = false)
    {
        WeakTypes bulletType = isFail ? WeakTypes.NONE : weight.Weaknesses[0].WeakType;
            
        weight.OnShot(weight.Weaknesses[0], bulletType);
    }
}
#endif