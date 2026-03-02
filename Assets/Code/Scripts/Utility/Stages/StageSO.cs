using UnityEngine;

[CreateAssetMenu(fileName = "StageSO", menuName = "Stage Info", order = 1)]
public class StageSO : ScriptableObject
{
    public float bestTime;
    public int enemyCount;
    public int urnCount;
    public int puzzleCount;
}
