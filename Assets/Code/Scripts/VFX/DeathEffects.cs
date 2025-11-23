using System.Collections.Generic;
using UnityEngine;

public class DeathEffects : MonoBehaviour
{
    private EventBindings<EnemyDeathEvent> _enemyDeathEventListener;
    
    public List<GameObject> effectObjects;

    private void OnEnable()
    {
        _enemyDeathEventListener = new EventBindings<EnemyDeathEvent>(OnDeath);
        EventBus<EnemyDeathEvent>.Register(_enemyDeathEventListener);
    }

    private void OnDisable()
    {
        EventBus<EnemyDeathEvent>.Unregister(_enemyDeathEventListener);
    }

    private void OnDeath(EnemyDeathEvent ctx)
    {
        if (ctx.enemy != GetComponent<EnemyBase>())
            return;
        
        foreach (var effect in effectObjects)
        {
            var particleSystems = effect.GetComponentsInChildren<ParticleSystem>();

            foreach (var particle in particleSystems)
            {
                particle.Stop();
                particle.Play();
            }
        }
    }
}
