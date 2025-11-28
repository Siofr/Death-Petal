using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    public GameObject deathScreenPrefab;
    
    private EventBindings<PlayerDeathEvent> _playerDeathEventListener;

    private void OnEnable()
    {
        _playerDeathEventListener = new EventBindings<PlayerDeathEvent>(OnDeath);
        EventBus<PlayerDeathEvent>.Register(_playerDeathEventListener);
    }

    private void OnDisable()
    {
        EventBus<PlayerDeathEvent>.Unregister(_playerDeathEventListener);
    }
    
    private void OnDeath()
    {
        Instantiate(deathScreenPrefab, transform.position, transform.rotation);
    }
}
