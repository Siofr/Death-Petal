using System;
using System.Collections;
using System.Timers;
using State_Machine;
using UnityEngine;

public class EnemyArcher: EnemyBase
{
    [Header("Archer Fields")]
    [SerializeField] private LineRenderer _targetLineRender;
    [SerializeField] private Transform _LOSRef;
    public float targetTime;
    public float maxLOSRadius;
    public float alertRotationAngle;

    private Coroutine _alertRoutine;
    private Coroutine _targetRoutine;
    private Coroutine _shotRoutine;
    private Coroutine _timerRoutine;
    private bool _inLos;
    
    protected override void Awake()
    {
        _targetLineRender.enabled = false;
        _targetLineRender.SetPosition(0, _LOSRef.position);
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
        
        __enemyStateMachine.AddTransition(alertState, targetState, new FuncPredicate( ()=> _alertRoutine == null));
        __enemyStateMachine.AddTransition(targetState, alertState, new FuncPredicate( ()=> !_inLos ));
        
        __enemyStateMachine.AddTransition(targetState, shootState, new FuncPredicate( () => _targetRoutine == null));
        __enemyStateMachine.AddTransition(shootState, targetState, new FuncPredicate( ()=> _shotRoutine == null));
        
        __enemyStateMachine.SetState(idleState);
    }

    public void CheckLOS(float losRadius, float losDist)
    {
        if (!Physics.SphereCast(_LOSRef.position, losRadius, _LOSRef.forward, out RaycastHit hit, losDist))
        {
            _inLos = false;
            return;
        }

        if (!hit.collider.CompareTag("Player"))
        {
            _inLos = false;
            return;
        }
        
        print("inLos");

        /*var hitDist = Vector3.Distance(hit.point, _LOSRef.position);
        var radiusRatio = hitDist / maxLOSDist;

        float hitXMin = hit.point.x + _LOSRef.position.x + losRadius * radiusRatio;
        float hitXMax = hit.point.x + _LOSRef.position.x + losRadius * (-radiusRatio);
        float hitYMin = hit.point.y + _LOSRef.position.y + losRadius * (-radiusRatio);
        float hitYMax = hit.point.y + _LOSRef.position.y + losRadius * (-radiusRatio);
            
        var conditionX = hitXMin <= hit.point.x && hit.point.x <= hitXMax;
        var conditionY = hitYMin <= hit.point.y && hit.point.y <= hitYMax;

        if (conditionY && conditionX) return;*/
        
        _inLos = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_LOSRef.position, _LOSRef.position + _LOSRef.forward*enemyData.attackRange);
    }

    private void LookAtTarget()
    {
        if (target == null) return;
        if (!_inLos) return;

        var targetPos = target.position;
        targetPos.y = transform.position.y;
        
        transform.LookAt(targetPos);
        
        _targetLineRender.SetPosition(1, target.position);
    }

    public void ToggleLineRenderer(bool enable)
    {
        _targetLineRender.enabled = enable;
    }

    public void StartAlertRoutine(float pauseTime, float angle, float speed)
    {
        if (_alertRoutine != null) return;
        
        _alertRoutine = StartCoroutine(AlertRoutine(pauseTime, angle, speed));
    }
    
    private IEnumerator AlertRoutine(float pauseTime, float angle, float speed)
    {
        float initAngle = transform.eulerAngles.y;

        int rotations = 0;
        
        while (target != null && !_inLos)
        {
            if (angle >= 360)
            {
                transform.Rotate(transform.up, speed * Time.deltaTime);
            }
            else if(angle == 0)
            {
                
            }
            else
            {
                if (rotations % 2 != 0)
                {
                    if (transform.eulerAngles.y < initAngle + angle)
                    {
                        transform.Rotate(transform.up, speed * Time.deltaTime);
                    }
                    else
                    {
                        rotations++;
                        yield return TimerRoutine(pauseTime);
                    }
                }
                else
                {
                    if (transform.eulerAngles.y > initAngle - angle)
                    {
                        transform.Rotate(transform.up, -speed * Time.deltaTime);
                    }
                    else
                    {
                        rotations++;
                        yield return TimerRoutine(pauseTime);
                    }
                }
            }

            yield return null;
        }

        _alertRoutine = null;
        
        Debug.Log("Player Spotted");
    }

    private IEnumerator TargetingRoutine(float time)
    {
        StopCoroutine(_timerRoutine);
        
        _timerRoutine = StartCoroutine(TimerRoutine(time));
        
        while (_timerRoutine != null)
        {
            LookAtTarget();
            yield return null;
        }

        _targetRoutine = null;
    }
    
    public void StartTargeting(float time)
    {
        if (_targetRoutine != null) return;

        _targetRoutine = StartCoroutine(TargetingRoutine(time));
    }

    private IEnumerator ShotRoutine(float time)
    {
        if (_timerRoutine != null || target == null) yield break;

        var playerController = target.GetComponent<TestPlayer>();
        
        yield return TimerRoutine(time);
        
        playerController.OnShot(playerController.Weaknesses[0], WeakTypes.PLAYER);

        _shotRoutine = null;
    }
    
    public void StartShot(float time)
    {
        if (_shotRoutine != null) return;
        
        _shotRoutine = StartCoroutine(ShotRoutine(time));
    }
    
    private IEnumerator TimerRoutine(float time)
    {
        yield return new WaitForSeconds(time);

        _timerRoutine = null;
    }

    public void StopAllArcherRoutines()
    {
        StopAllCoroutines();
        _alertRoutine = null;
        _timerRoutine = null;
        _shotRoutine = null;
        _targetRoutine = null;
    }
}