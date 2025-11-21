using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Puzzle Input Save Data", menuName = "Game SaveData", order = 2)]
public class PuzzleInputData : SaveData
{
    public List<Weakness> health;
}