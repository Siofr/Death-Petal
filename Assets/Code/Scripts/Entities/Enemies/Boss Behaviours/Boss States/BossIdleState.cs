using UnityEngine;

public class BossIdleState<T> : BossBaseState<T> where T : BossBase
{
    public BossIdleState(T bossController) : base(bossController) { }
    
    public override void OnEnter()
    {
        Debug.Log("Boss: Entering Idle State");
    }
    
    public override void OnExit()
    {
        
    }
    
    
}