using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Puzzle Element Save Data", menuName = "Game SaveData", order = 0)]
public class PlayerSaveData : SaveData
{
    public List<Weakness> health;
}