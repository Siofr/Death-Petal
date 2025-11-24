using UnityEngine;

public class PlayerTrailSpawner : MonoBehaviour
{
    private EventBindings<SpawnTrail> _onShootEventListener;
    private EventBindings<ActiveTargetEvent> _onActiveTargetEventListener;
    private GameObject _trailObject;
    private Transform _activeTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _onShootEventListener = new EventBindings<SpawnTrail>(SpawnTrail);
        _onActiveTargetEventListener = new EventBindings<ActiveTargetEvent>(ActivateTarget);
    }

    private void Start()
    {
        _trailObject = transform.GetChild(0).gameObject;
    }

    private void Update()
    {

    }

    private void OnEnable()
    {
        EventBus<SpawnTrail>.Register(_onShootEventListener);
        EventBus<ActiveTargetEvent>.Register(_onActiveTargetEventListener);
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
