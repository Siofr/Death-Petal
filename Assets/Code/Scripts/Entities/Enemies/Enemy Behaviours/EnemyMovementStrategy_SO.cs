using UnityEngine;

[CreateAssetMenu(fileName = "Default Movement Strategy", menuName = "Enemy Movement Strategy", order = 0)]
public class EnemyMovementStrategy_SO : ScriptableObject
{
    public virtual Vector3 Movement()
    {
        return new();
    }
}