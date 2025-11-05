using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Enemy Attack Strategy", menuName = "Enemy Attack Strategy", order =0)]
public class EnemyAttackStrategy_SO : ScriptableObject
{
    public virtual void Attack()
    {
    }
}