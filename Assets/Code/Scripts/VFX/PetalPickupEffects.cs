using System.Collections.Generic;
using UnityEngine;

public class PetalPickupEffects : MonoBehaviour
{
    private EventBindings<PetalPickpEvent> _petalPickupListener;
    
    public List<ParticleSystem> particleSystems;
    
    private void OnEnable()
    {
        _petalPickupListener = new EventBindings<PetalPickpEvent>(PickedPetal);
        EventBus<PetalPickpEvent>.Register(_petalPickupListener);
    }

    private void OnDisable()
    {
        EventBus<PetalPickpEvent>.Unregister(_petalPickupListener);
    }

    private void PickedPetal()
    {
        //if (context.weakness == null) return; 
        
        foreach (var effect in particleSystems)
        {
            effect.Stop();
            effect.Play();
        }
    }
}
