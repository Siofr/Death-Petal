using UnityEngine;


public class ShootEffects : MonoBehaviour
{
    private EventBindings<ShootEvent> _shootEventListener;
    
    private void OnEnable()
    {
        _shootEventListener = new EventBindings<ShootEvent>(OnShoot);
        EventBus<ShootEvent>.Register(_shootEventListener);
    }

    private void OnDisable()
    {
        EventBus<ShootEvent>.Unregister(_shootEventListener);
    }

    private void OnShoot(ShootEvent context)
    {
        
    }
}