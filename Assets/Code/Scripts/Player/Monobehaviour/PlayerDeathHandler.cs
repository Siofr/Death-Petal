using UnityEngine;
using FMODUnity;

public class PlayerDeathHandler : MonoBehaviour
{
    public GameObject deathScreenPrefab;
    
    private EventBindings<PlayerDeathEvent> _playerDeathEventListener;

    [Header("Audio Paths")]
    public EventReference onPlayerDeathEventPath;

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
        RuntimeManager.PlayOneShot(onPlayerDeathEventPath);
        Instantiate(deathScreenPrefab, transform.position, transform.rotation);
    }
}
