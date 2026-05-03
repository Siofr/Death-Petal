using System.Collections;
using UnityEngine;

public class PuzzleAOEInput : PuzzleInputBase
{
    [Header("AOE Effect Input Setup")] 
    [SerializeField] private float _effectTime;
    [SerializeField] private ParticleSystem[] _onShotParticles;
    
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        base.OnShot(weakness, damageType);

        foreach (var output in PuzzleOutputs)
        {
            CompletionCondition(true, output);
            
            StartEffectTimer(_effectTime);
        }
        
        foreach (ParticleSystem onShotParticle in _onShotParticles)
        {
            onShotParticle.Play();
        }
        
        ToggleAllWeaknesses(false);
    }

    protected override void OnCameraChange(CameraChangeEvent ctx)
    {
        if (PuzzleOutputs[0].IsSolved)
        {
            ToggleAllWeaknesses(false);
            return;
        }
        
        base.OnCameraChange(ctx);
    }

    public void StartEffectTimer(float time)
    {
        StopAllCoroutines();
        StartCoroutine(EffectTimer(time));
    }
    
    private IEnumerator EffectTimer(float time)
    {
        yield return new WaitForSeconds(time);

        foreach (var output in PuzzleOutputs)
        {
            CompletionCondition(false, output);
        }
        
        ToggleAllWeaknesses(true);
    }
}
