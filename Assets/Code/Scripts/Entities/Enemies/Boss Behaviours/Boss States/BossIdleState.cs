using UnityEngine;

public class BossIdleState : BossBaseState
{
    public BossIdleState(BossBase bossController) : base(bossController) { }
    
    public override void OnEnter()
    {
        Debug.Log("Boss: Entering Idle State");
    }
    
    public override void OnExit()
    {
        
    }
    
    
}