using UnityEngine;

public struct PetalSpawnEvent: IEvent
{
    public Vector3 position;
    
    public PetalSpawnEvent(Vector3 position)
    {
        this.position = position;
    }
}

public class PetalFactory : Singleton<PetalFactory>
{
    [SerializeField] private GameObject _petalPrefab;
    [SerializeField] private Transform _petalSpawnContainer;
    
    //Events
    private EventBindings<PetalSpawnEvent> _petalSpawnEventListener;

    private void CreatePetal(Vector3 position)
    {
        if (_petalSpawnContainer == null)
        {
            Instantiate(_petalPrefab, position, _petalPrefab.transform.rotation);
            return;
        } 
        
        Instantiate(_petalPrefab, position, _petalPrefab.transform.rotation, _petalSpawnContainer);
    }

    private void CreatePetal(PetalSpawnEvent context)
    {
        CreatePetal(context.position);
    }

    public void OnEnable()
    {
        _petalSpawnEventListener = new EventBindings<PetalSpawnEvent>(CreatePetal);
        EventBus<PetalSpawnEvent>.Register(_petalSpawnEventListener);
    }

    public void OnDisable()
    {
        EventBus<PetalSpawnEvent>.Unregister(_petalSpawnEventListener);
    }
}