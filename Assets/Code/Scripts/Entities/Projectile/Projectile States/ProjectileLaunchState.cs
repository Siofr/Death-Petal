using UnityEngine;

public class ProjectileLaunchState : ProjectileBaseState
{
    public ProjectileLaunchState(ProjectileBase projectileController) : base(projectileController) { }
    
    public override void OnEnter()
    {
        Debug.Log("Projectile: Entering Idle State");
    }
    
    public override void OnExit()
    {
        
    }
    
    
}

