using UnityEngine;

[CreateAssetMenu(fileName = "Default Enemy Config Data", menuName = "Enemy Config", order = 0)]
public class EnemyDefaultConfig_SO : EnemyConfig_SO
{
}

[CreateAssetMenu(fileName = "Archer Enemy Config Data", menuName = "Enemy Config", order = 0)]
public class EnemyArcherConfig_SO : EnemyConfig_SO
{
    [Header("Archer Config")] 
    public float targetTime;
    public float shotTime;
}