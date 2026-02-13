using System.Collections;
using State_Machine;
using UnityEngine;

public class EnemyArcher: EnemyBase
{
    [Header("Archer Fields")]
    [SerializeField] private LineRenderer _targetLineRender;
    [SerializeField] private Transform _lineRendererStartRef;
    [SerializeField] private Transform _LOSRef;
    
    private Coroutine _targetTimer;
    private Coroutine _shotTimer;
    private bool _inLOS;

    protected override void Awake()
    {
        _targetLineRender.enabled = false;
        _targetLineRender.SetPosition(0, _lineRendererStartRef.position);
    }
    
    protected override void InitialiseStateMachine()
    {
        var idleState = new EnemyIdleState(this);
        var deathState = new EnemyDeathState(this);
        var alertState = new EnemyArcherAlertState(this);
        var targetState = new EnemyArcherTargetState(this);
        var shootState = new EnemyArcherShootState(this);
        
        __enemyStateMachine.AddAnyTransition(deathState, new FuncPredicate( () => IsDead));
        __enemyStateMachine.AddAnyTransition(idleState, new FuncPredicate( ()=> target == null));
        __enemyStateMachine.AddAnyTransition(alertState, new FuncPredicate( ()=> target != null));
        
        __enemyStateMachine.AddTransition(alertState, targetState, new FuncPredicate( ()=> _inLOS));
        
        __enemyStateMachine.AddTransition(targetState, shootState, new FuncPredicate( () => _targetTimer == null));
        __enemyStateMachine.AddTransition(shootState, targetState, new FuncPredicate( ()=> _shotTimer == null));
    }

    public bool CheckLOS(float losRadius, float maxLOSDist)
    {
        if (Physics.SphereCast(_LOSRef.position, losRadius, _LOSRef.forward, out RaycastHit hit, maxLOSDist))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }
    
    public void LookAtTarget()
    {
        if (target == null) return;
        if (!_inLOS) return;
        
        var lookRot = Quaternion.LookRotation(target.position - transform.position);
        var eulerLookRot = lookRot.eulerAngles;
        
        transform.eulerAngles = new Vector3(transform.eulerAngles.z, eulerLookRot.y, transform.eulerAngles.z);
        
        _targetLineRender.enabled = true;
        _targetLineRender.SetPosition(1, target.position);
    }
    
    public void StartTargetTimer(float time)
    {
        if (_targetTimer != null) return;

        StartCoroutine(TimerRoutine(time));
    }

    public void StartShotTime(float time)
    {
        if (_targetTimer != null) return;
        
        StartCoroutine(TimerRoutine(time));
    }

    private IEnumerator TimerRoutine(float time)
    {
        yield return new WaitForSeconds(time);
    }
}