using System.Collections;
using UnityEngine;

public class PuzzleAOEInput : PuzzleInputBase
{
    [Header("AOE Effect Input Setup")] 
    [SerializeField] private float _effectTime;
    
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        base.OnShot(weakness, damageType);

        foreach (var output in PuzzleOutputs)
        {
            CompletionCondition(true, output);
            
            StartEffectTimer(_effectTime);
        }
        
        ToggleAllWeaknesses(false);
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
