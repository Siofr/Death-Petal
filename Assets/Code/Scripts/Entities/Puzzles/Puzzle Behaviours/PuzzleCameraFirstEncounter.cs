using System;
using UnityEngine;

[CreateAssetMenu(fileName = "First Encounter Camera Pan Condition", menuName = "Camera Pan Conditions", order = 0)]
public class PuzzleCameraFirstEncounter : PuzzleCameraCondition_SO
{
    [SerializeField] private EnemyType _typeOfEnemy;
    private EnemyBase _targetEnemy;
    
    private void OnEnable()
    {
        Debug.Log("hi :)");
        
        var type = _typeOfEnemy switch
        {
            EnemyType.Lurker => typeof(EnemyLurker),
            EnemyType.Mother => typeof(EnemyMother),
            _ => typeof(EnemyBase)
        };

        var allEnemies = FindObjectsOfType<EnemyBase>();
        
        Debug.Log($"Enemies Found: {allEnemies.Length}, Looking for {type}");
        
        for (int i = 0; i < allEnemies.Length; i++)
        {
            Debug.Log($"Enemies Type: {allEnemies[i].GetType()}, Looking for {type}");
            
            if (allEnemies[i].GetType() != type) continue;
            
            Debug.Log("Reached Condition");
            
            _targetEnemy =  allEnemies[i];
            break;
        }
        
        exitCondition = new Func<bool>(ExitCondition);
    }

    private bool ExitCondition()
    {
        return !_targetEnemy.animator.GetBool("Spawning");
    }
}