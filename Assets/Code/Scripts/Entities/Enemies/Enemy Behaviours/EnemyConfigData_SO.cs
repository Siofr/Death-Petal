using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfigData")]
public class EnemyConfigData_SO : ScriptableObject
{
    public GameObject prefab;
    public float movementSpeed;
    public float reloadMovementSpeed;
}   