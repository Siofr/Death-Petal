using System.Collections.Generic;
using UnityEngine;


public class ShootEffects : MonoBehaviour
{
    private EventBindings<SpawnTrail> _spawnTrailListener;
    
    public List<ParticleSystem> particleSystems;
    
    private void OnEnable()
    {
        _spawnTrailListener = new EventBindings<SpawnTrail>(OnShoot);
        EventBus<SpawnTrail>.Register(_spawnTrailListener);
    }

    private void OnDisable()
    {
        EventBus<SpawnTrail>.Unregister(_spawnTrailListener);
    }

    private void OnShoot(SpawnTrail context)
    {
        //if (context.weakness == null) return; 
        
        foreach (var effect in particleSystems)
        {
            effect.Stop();
            effect.Play();
        }
    }
}