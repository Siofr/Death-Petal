using System;
using System.Collections;
using System.Timers;
using State_Machine;
using UnityEngine;
using UnityEngine.Serialization;
using FMODUnity;

public class EnemyArcher: EnemyBase
{
    [FormerlySerializedAs("targetLineRender")]
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

    [Header("Unit Specific Audio Paths")]
    public EventReference onArrowKnock;
    public EventReference onArrowRelease;

    protected override void Awake()
    {
        _targetLineRender.enabled = false;
        _targetLineRender.SetPosition(0, _LOSRef.position);
    }

    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        base.OnShot(weakness, damageType);
        
        //TO REMOVE JUST FOR TESTING
        if(Weaknesses.Count < 1) Destroy(gameObject);
    }

    protected override void InitialiseStateMachine()
    {
        var idleState = new EnemyIdleState<EnemyArcher>(this);
        var deathState = new EnemyDeathState<EnemyArcher>(this);
        var alertState = new EnemyArcherAlertState(this);
        var targetState = new EnemyArcherTargetState(this);
        var shootState = new EnemyArcherShootState(this);
        
        __enemyStateMachine.AddAnyTransition(deathState, new FuncPredicate( () => IsDead));
        
        __enemyStateMachine.AddTransition(alertState, idleState, new FuncPredicate( ()=> target == null));
        __enemyStateMachine.AddTransition(idleState, alertState, new FuncPredicate( ()=> target != null));
        
        __enemyStateMachine.AddTransition(alertState, targetState, new FuncPredicate( ()=> _inLos));
        __enemyStateMachine.AddTransition(targetState, alertState, new FuncPredicate( ()=> !_inLos ));
        
        __enemyStateMachine.AddTransition(targetState, shootState, new FuncPredicate( () => _targetRoutine == null));
        __enemyStateMachine.AddTransition(shootState, alertState, new FuncPredicate( ()=> _shotRoutine == null));
        
        __enemyStateMachine.SetState(idleState);
    }

    public void CheckLOS(float losRadius, float losDist)
    {
        if (!Physics.SphereCast(_LOSRef.position, losRadius, _LOSRef.forward, out RaycastHit hit, losDist, 1 << 6))
        {
            _inLos = false;
            return;
        }
        
        //print("inLos");

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
        
        Gizmos.DrawLine(_LOSRef.position, _LOSRef.forward*enemyData.attackRange);
    }

    public void ToggleLineRenderer(bool enable)
    {
        _targetLineRender.enabled = enable;
    }

    public void ToggleLineRendererColor(Color color)
    {
        color.a = 1f;
        
        _targetLineRender.startColor = color;
        _targetLineRender.endColor = color;
    }

    public void UpdateLineRenderer()
    {
        _targetLineRender.SetPosition(1, target.position);
    }
    
    public void StartAlertRoutine(float pauseTime, float angle, float speed)
    {
        if (_alertRoutine != null) return;
        
        _alertRoutine = StartCoroutine(AlertRoutine(pauseTime, angle, speed));
    }
    
    private IEnumerator AlertRoutine(float pauseTime, float angle, float speed)
    {
        if (target == null) yield break;
        
        var absAngle = Mathf.Abs(angle);
        var absSpeed = Mathf.Abs(speed);
        
        var signAngle = Vector3.SignedAngle(transform.forward, target.position-transform.position, Vector3.up);
        speed = signAngle < 0 ? -speed : speed;
        angle = signAngle < 0 ? absAngle : absAngle;

        var firstActivation = true;
        var isFirst = false;
        var tempRot = 0f;
        
        animator.SetFloat("Angle", LookAtAngle());
        
        while (target != null && !_inLos)
        {

            if (firstActivation)
            {
                animator.SetTrigger("Spawn");
                firstActivation = false;
            }

            if (absAngle >= 360)
            {
                //transform.Rotate(transform.up, speed*Time.deltaTime);
                
                yield return null;
            }

            if (angle == 0) yield return null;

            if (!isFirst)
            {
                
                
                if (tempRot + absSpeed * Time.deltaTime < absAngle)
                {
                    tempRot += absSpeed * Time.deltaTime;
                    //transform.Rotate(transform.up, speed * Time.deltaTime);
                }
                else
                {
                    isFirst = true;
                    tempRot = 0f;
                    speed = -speed;
                    
                    yield return new WaitForSeconds(pauseTime);
                }
            }
            else
            {
                
                if (tempRot + absSpeed * Time.deltaTime < absAngle * 2)
                {
                    tempRot += absSpeed * Time.deltaTime;
                    //transform.Rotate(transform.up, speed * Time.deltaTime);
                }
                else
                {
                    tempRot = 0f;
                    speed = -speed;

                    yield return new WaitForSeconds(pauseTime);
                }
            }

            yield return null;
            //TODO ALERT STATE ROTATION
        }

        _alertRoutine = null;
        
        Debug.Log("Player Spotted");
    }

    private IEnumerator TargetingRoutine(float time)
    {
        _timerRoutine = StartCoroutine(TimerRoutine(time));
        
        while (_timerRoutine != null)
        {
            animator.SetFloat("Angle", LookAtAngle());
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
        animator.SetTrigger("Shoot");
        
        yield return TimerRoutine(time);
        
        CheckLOS(maxLOSRadius, enemyData.attackRange);
        
        if(_inLos) playerController.OnShot(playerController.Weaknesses[0], WeakTypes.PLAYER);

        _inLos = false;
        
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

    public override void StopAllStateRoutines()
    {
        base.StopAllStateRoutines();
        
        _alertRoutine = null;
        _targetRoutine = null;
        _shotRoutine = null;
        _timerRoutine = null;
    }
}