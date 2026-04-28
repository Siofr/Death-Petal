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
    
    [Range(0, 90)] public float alertRotationAngle;

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
        base.Awake();
        _initialRotation = transform.rotation.eulerAngles.y;
        _currentRotation = transform.rotation.eulerAngles.y;
        _targetLineRender.enabled = false;
        _targetLineRender.SetPosition(0, _LOSRef.position);
    }

    // protected override void Start()
    // {
    //     InitialiseWeaknesses();
    //     Initialise();
    // }
    
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        base.OnShot(weakness, damageType);
        
        // //TO REMOVE JUST FOR TESTING
        // if(Weaknesses.Count < 1) Destroy(gameObject);
    }

    protected override void InitialiseStateMachine()
    {
        var idleState = new EnemyIdleState<EnemyArcher>(this);
        var deathState = new EnemyDeathState<EnemyArcher>(this);
        var alertState = new EnemyArcherAlertState(this);
        var targetState = new EnemyArcherTargetState(this);
        var shootState = new EnemyArcherShootState(this);
        var spawnState = new EnemySpawnState<EnemyArcher>(this);
        
        __enemyStateMachine.AddAnyTransition(deathState, new FuncPredicate( () => IsDead));
        
        __enemyStateMachine.AddTransition(alertState, idleState, new FuncPredicate( ()=> target == null));
        __enemyStateMachine.AddTransition(idleState, alertState, new FuncPredicate( ()=> target != null));
        
        __enemyStateMachine.AddTransition(alertState, targetState, new FuncPredicate( ()=> _inLos));
        __enemyStateMachine.AddTransition(targetState, alertState, new FuncPredicate( ()=> !_inLos ));
        
        __enemyStateMachine.AddTransition(targetState, shootState, new FuncPredicate( () => _targetRoutine == null));
        __enemyStateMachine.AddTransition(shootState, alertState, new FuncPredicate( ()=> _shotRoutine == null));
        
        if (animator != null)
        {
            __enemyStateMachine.AddAnyTransition(spawnState, new FuncPredicate(()=> animator.GetBool("Spawning")));
            __enemyStateMachine.AddTransition(spawnState, idleState, new FuncPredicate(()=> !animator.GetBool("Spawning")));
        }
        
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
        
        var signAngle = Vector3.SignedAngle(transform.forward, target.position-transform.position, Vector3.up);
        var tempSpeed = signAngle < 0 ? -speed : speed;
        var tempAngle = signAngle < 0 ? -absAngle : absAngle;

        var tempRot = 0f;
        
        //animator.SetFloat(Animator.StringToHash("Angle"), 0);
        
        while (!_inLos)
        {
            if (angle == 0) yield return null;

            while (tempRot != tempAngle)
            {
                tempRot += Time.deltaTime * tempSpeed;
                _currentRotation += Time.deltaTime * tempSpeed;
                
                transform.eulerAngles = new Vector3(0, _currentRotation, 0);
                
                if (tempAngle < 0)
                {
                    if (tempRot < tempAngle)
                    {
                        tempRot = tempAngle;
                        transform.eulerAngles = new Vector3(0, _initialRotation + tempAngle, 0);
                        _currentRotation = transform.eulerAngles.y;
                        //animator.SetFloat(Animator.StringToHash("Angle"), MapRotToAnim(_currentRotation));

                        break;
                    }
                }
                else
                {
                    if (tempRot > tempAngle)
                    {
                        tempRot = tempAngle;
                        transform.eulerAngles = new Vector3(0, _initialRotation + tempAngle, 0);
                        _currentRotation = transform.eulerAngles.y;
                        //animator.SetFloat(Animator.StringToHash("Angle"), MapRotToAnim(_currentRotation));
                        
                        break;
                    }
                }
                
                yield return null;
            }
            
            yield return new WaitForSeconds(pauseTime);
            
            tempAngle = -tempAngle;
            tempSpeed = -tempSpeed;
        }
        
        print("Found Target");
    }

    private float _initialRotation;
    private float _currentRotation;

    private float MapAnimToRot(float value)
    {
        var minRot = _initialRotation - 90;
        var maxRot = _initialRotation + 90;

        var minAnim = -90f;
        var maxAnim = 90f;
        
        return minRot + (maxRot - minRot) * ((value - minAnim) / (maxAnim - minAnim));
    }

    private float MapRotToAnim(float value)
    {
        var minRot = _initialRotation - alertRotationAngle;
        var maxRot = _initialRotation + alertRotationAngle;

        var minAnim = -90f;
        var maxAnim = 90f;
        
        return minAnim + (maxAnim - minAnim) * ((value - minRot) / (maxRot - minRot));
    }
    
    private IEnumerator TargetingRoutine(float time)
    {
        _timerRoutine = StartCoroutine(TimerRoutine(time));
        
        while (_timerRoutine != null)
        {
            LookAt();

            if (!IsInAlertRange())
            {
                _targetRoutine = null;

                yield break;
            }
            
            yield return null;
        }

        _targetRoutine = null;
    }

    private bool IsInAlertRange()
    {
        var constraints = new Vector2(_initialRotation-alertRotationAngle, _initialRotation+alertRotationAngle);
        
        return _currentRotation >=  constraints.x && _currentRotation <= constraints.y;
    }
    
    private void LookAt()
    {
        if (target == null) return;

        transform.LookAt(target);
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
        
        CheckLOS(maxLOSRadius, enemyData.attackRange);
        animator.SetTrigger("Shoot");
        
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