using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Puzzle Input Save Data")]
public class PuzzleInputData : SaveData
{
    public List<Weakness> health;
    
    public void Save(Vector3 pos, List<Weakness> weaknesses)
    {
        Save(@position);
        this.health = health;
    }

    public void Load(Transform refTransform, List<Weakness> weaknesses)
    {
        Load(refTransform);
        weaknesses = this.health;
    }
}