using System.Collections.Generic;
using UnityEngine;

public class DeathEffects : MonoBehaviour
{
    private EventBindings<EnemyDeathEvent> _enemyDeathEventListener;
    private EventBindings<WrongShotEvent> _wrongShotEventListener;
    
    public List<GameObject> deathEffectObjects;
    public List<GameObject> wrongShotEffectObjects;

    private void OnEnable()
    {
        _enemyDeathEventListener = new EventBindings<EnemyDeathEvent>(OnDeath);
        _wrongShotEventListener = new EventBindings<WrongShotEvent>(OnWrongShot);
        
        EventBus<EnemyDeathEvent>.Register(_enemyDeathEventListener);
        EventBus<WrongShotEvent>.Register(_wrongShotEventListener);
    }

    private void OnDisable()
    {
        EventBus<EnemyDeathEvent>.Unregister(_enemyDeathEventListener);
        EventBus<WrongShotEvent>.Unregister(_wrongShotEventListener);
    }

    private void OnDeath(EnemyDeathEvent ctx)
    {
        if (ctx.enemy != GetComponentInParent<EnemyBase>())
            return;
        
        foreach (var effect in deathEffectObjects)
        {
            var particleSystems = effect.GetComponentsInChildren<ParticleSystem>();

            foreach (var particle in particleSystems)
            {
                particle.Stop();
                particle.Play();
            }
        }
    }

    private void OnWrongShot(WrongShotEvent ctx)
    {
        if (ctx.enemy != GetComponentInParent<EnemyBase>())
            return;

        foreach (var effect in wrongShotEffectObjects)
        {
            effect.SetActive(true);
        }
    }
}
