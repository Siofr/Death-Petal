using UnityEngine;

public class PlayerTrailSpawner : MonoBehaviour
{
    private EventBindings<SpawnTrail> _onShootEventListener;
    private EventBindings<ActiveTargetEvent> _onActiveTargetEventListener;
    private GameObject _trailObject;
    private Transform _activeTarget;
    public GameObject _particleSystem;
    public float _particleLifetime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _onShootEventListener = new EventBindings<SpawnTrail>(SpawnParticle);
        _onActiveTargetEventListener = new EventBindings<ActiveTargetEvent>(ActivateTarget);
    }

    private void Start()
    {
        _trailObject = transform.GetChild(0).gameObject;

        ParticleSystem _parentParticles = _particleSystem.GetComponent<ParticleSystem>();
        ParticleSystem _childParticles = _particleSystem.transform.GetChild(0).GetComponent<ParticleSystem>();
        _particleLifetime = _parentParticles.main.startLifetime.constant + _childParticles.main.startLifetime.constant;
    }

    private void OnEnable()
    {
        EventBus<SpawnTrail>.Register(_onShootEventListener);
        EventBus<ActiveTargetEvent>.Register(_onActiveTargetEventListener);
    }


    private void OnDisable()
    {
        EventBus<SpawnTrail>.Unregister(_onShootEventListener);
        EventBus<ActiveTargetEvent>.Unregister(_onActiveTargetEventListener);
    }

    private void SpawnParticle(SpawnTrail ctx)
    {
        Quaternion rotation = new Quaternion();

        if (_activeTarget)
        {
            Debug.Log("Active Target");
            var lookPos = _activeTarget.parent.position - transform.position;
            rotation = Quaternion.LookRotation(lookPos);
        }
        else
        {
            rotation = transform.rotation;
        }

        GameObject newParticle = Instantiate(_particleSystem, transform.position, rotation);
        Destroy(newParticle, _particleLifetime);
    }

    private void SpawnTrail(SpawnTrail ctx)
    {
        Quaternion rotation = new Quaternion();

        if (_activeTarget)
        {
            var lookPos = _activeTarget.parent.position - transform.position;
            rotation = Quaternion.LookRotation(lookPos);
        }
        else
        {
            rotation = transform.rotation;
        }

        GameObject newSpawn = Instantiate(_trailObject, transform.position, rotation);
        PlayerTrail trailScript = newSpawn.GetComponent<PlayerTrail>();
        if (_activeTarget) trailScript.target = _activeTarget.parent;
        TrailRenderer newTrail = newSpawn.GetComponent<TrailRenderer>();
        newTrail.startColor = ctx.bulletColor;
        newSpawn.transform.parent = null;
        newSpawn.SetActive(true);
    }

    private void ActivateTarget(ActiveTargetEvent ctx)
    {
        _activeTarget = ctx.activeTarget;
    }
}
