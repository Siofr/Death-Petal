using UnityEngine;

public class ProjectileIdleState : ProjectileBaseState
{
    public ProjectileIdleState(ProjectileBase projectileController) : base(projectileController) { }
    
    public override void OnEnter()
    {
        Debug.Log("Projectile: Entering Idle State");
    }
    
    public override void OnExit()
    {
        
    }
    
    
}