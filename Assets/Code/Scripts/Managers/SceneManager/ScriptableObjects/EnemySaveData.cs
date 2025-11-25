using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Save Data")]
public class EnemySaveData : SaveData
{
    public List<WeakTypes> health;
}