using System.Collections;
using State_Machine;
using UnityEngine;

public class ProjectileBaseState : IState
{
    protected readonly ProjectileBase projectileController;
    
    protected ProjectileBaseState() {  }
    
    protected ProjectileBaseState(ProjectileBase projectileController) => this.projectileController = projectileController;
    
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
        
        var startValue = projectileController.animator.GetFloat(blendVariableID);
        var lerpValue = 0f;

        while (lerpValue < 1)
        {
            projectileController.animator.SetFloat(blendVariableID, Mathf.Lerp(startValue, targetValue, lerpValue));
            lerpValue += Time.deltaTime / time;
            yield return null;
        }

        projectileController.animator.SetFloat(blendVariableID, Mathf.Lerp(startValue, targetValue, 1));
    }
}
