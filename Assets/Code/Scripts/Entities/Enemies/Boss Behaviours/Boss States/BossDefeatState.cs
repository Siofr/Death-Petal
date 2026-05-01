using UnityEngine;

public class BossDefeatState<T> : BossBaseState<T> where T : BossBase
{
    public BossDefeatState(T bossController) : base(bossController) { }
	
    public override void OnEnter()
    {
        Debug.Log("BOSS DIE");
        bossController.StopAllCoroutines();
        bossController.animator.SetFloat(Animator.StringToHash("Blend"),0f);
        EventBus<OnBossKilledEvent>.Raise(new OnBossKilledEvent());
        bossController.ClearPath();
    }
}