using UnityEngine;

public class ProjectileExplodeState : ProjectileBaseState
{
    public ProjectileExplodeState(ProjectileBase projectileController) : base(projectileController) { }
    
    public override void OnEnter()
    {
        Debug.Log("Projectile: Entering Idle State");
    }
    
    public override void OnExit()
    {
        
    }
    
    
}
