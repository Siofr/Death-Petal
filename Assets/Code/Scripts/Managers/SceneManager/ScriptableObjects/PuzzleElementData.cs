using UnityEngine;

[CreateAssetMenu(fileName = "Puzzle Element Save Data", menuName = "Scriptable Objects/Game SaveData", order = 4)]
public class PuzzleElementData : SaveData
{
    public void Save(Vector3 pos)
    {
        base.Save(pos);
    }

    public void Load(Transform refTransform)
    {
        base.Load(refTransform);
    }
}