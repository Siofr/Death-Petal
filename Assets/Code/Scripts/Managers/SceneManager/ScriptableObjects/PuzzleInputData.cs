using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Puzzle Input Save Data")]
public class PuzzleInputData : SaveData
{
    public List<WeakTypes> health;
}