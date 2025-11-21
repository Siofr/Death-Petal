using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Enemy Save Data", menuName = "Game SaveData", order = 1)]
public class EnemySaveData : SaveData
{
    public List<Weakness> health;
}