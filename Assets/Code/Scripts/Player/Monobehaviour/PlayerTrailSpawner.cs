using UnityEngine;

public class PlayerTrailSpawner : MonoBehaviour
{
    private EventBindings<SpawnTrail> _onShootEventListener;
    private GameObject _trailObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _onShootEventListener = new EventBindings<SpawnTrail>(SpawnTrail);
    }

    private void Start()
    {
        _trailObject = transform.GetChild(0).gameObject;
    }

    private void OnEnable()
    {
        EventBus<SpawnTrail>.Register(_onShootEventListener);
    }

    private void SpawnTrail(SpawnTrail ctx)
    {
        GameObject newSpawn = Instantiate(_trailObject, transform.position, transform.rotation);
        TrailRenderer newTrail = newSpawn.GetComponent<TrailRenderer>();
        newTrail.startColor = ctx.bulletColor;
        newSpawn.transform.parent = null;
        newSpawn.SetActive(true);
    }
}
