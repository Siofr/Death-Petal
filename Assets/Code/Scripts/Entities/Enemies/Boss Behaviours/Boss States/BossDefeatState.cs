using UnityEngine;

public class BossDefeatState : BossBaseState
{
    public BossDefeatState(BossBase bossController) : base(bossController) { }
	
    public override void OnEnter()
    {
        bossController.StopAllCoroutines();
        bossController.animator.SetFloat(Animator.StringToHash("Blend"),0f);
        bossController.ClearPath();
    }
}