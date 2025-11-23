using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private LevelData _levelData;
}