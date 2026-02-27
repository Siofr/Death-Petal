

using System.Collections;
using State_Machine;
using UnityEngine;

public class BossBaseState<T> : IState where T : BossBase
{
    protected readonly BossBase bossController;
    
    protected BossBaseState() {  }
    
    protected BossBaseState(T bossController) => this.bossController = bossController;
    
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
        
        var startValue = bossController.animator.GetFloat(blendVariableID);
        var lerpValue = 0f;

        while (lerpValue < 1)
        {
            bossController.animator.SetFloat(blendVariableID, Mathf.Lerp(startValue, targetValue, lerpValue));
            lerpValue += Time.deltaTime / time;
            yield return null;
        }

        bossController.animator.SetFloat(blendVariableID, Mathf.Lerp(startValue, targetValue, 1));
    }
}

