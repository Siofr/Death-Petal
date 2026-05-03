using System.Collections;
using System.Collections.Generic;
using State_Machine;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemyBaseState<T> : IState where T : EnemyBase
{
    protected readonly T enemyController;

    protected EnemyBaseState()
    {
    }

    protected EnemyBaseState(T enemyController)
    {
        this.enemyController = enemyController;
    }
    
    public virtual void OnEnter()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void OnExit()
    {
    }
    
    public IEnumerator LerpBlendState(string blendName, float targetValue, float time)
    {
        var blendVariableID = Animator.StringToHash(blendName);
        
        var startValue = enemyController.animator.GetFloat(blendVariableID);
        var lerpValue = 0f;

        while (lerpValue < 1)
        {
            enemyController.animator.SetFloat(blendVariableID, Mathf.Lerp(startValue, targetValue, lerpValue));
            lerpValue += Time.deltaTime / time;
            yield return null;
        }

        enemyController.animator.SetFloat(blendVariableID, Mathf.Lerp(startValue, targetValue, 1));
    }
}