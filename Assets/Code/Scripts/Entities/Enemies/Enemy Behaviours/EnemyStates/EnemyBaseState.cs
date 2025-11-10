using State_Machine;
using UnityEngine.PlayerLoop;

public class EnemyBaseState : IState
{
    protected readonly EnemyBase enemyController;
    
    protected EnemyBaseState() {  }
    
    protected EnemyBaseState(EnemyBase enemyController) => this.enemyController = enemyController;
    
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
}