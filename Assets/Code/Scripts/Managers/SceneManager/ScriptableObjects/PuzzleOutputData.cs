using UnityEngine;

[CreateAssetMenu(fileName = "Puzzle Output Save Data")]
public class PuzzleOutputData : SaveData
{
    public bool isSolved;

    public void Save(Vector3 pos, bool solvedCondition)
    {
        Save(pos);
        isSolved = solvedCondition;
    }

    public void Load(Transform refTransform, ref bool solvedCondition)
    {
        Load(refTransform);
        solvedCondition = isSolved;
    }
}