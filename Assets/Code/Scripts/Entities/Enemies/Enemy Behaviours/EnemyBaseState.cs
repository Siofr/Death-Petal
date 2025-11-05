using State_Machine;
using UnityEngine.PlayerLoop;

public class EnemyBaseState : IState
{
    public EnemyBase enemyController;
    
    protected EnemyBaseState() {  }
    
    public EnemyBaseState(EnemyBase enemyController) => this.enemyController = enemyController;

    public virtual void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Update()
    {
        throw new System.NotImplementedException();
    }

    public virtual void FixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnExit()
    {
        throw new System.NotImplementedException();
    }
}

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(EnemyBase enemyController) : base(enemyController) { } 

    public override void Update()
    {
    }
}

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState(EnemyBase enemyController) : base(enemyController) { }

    public override void Update()
    {
    }
}