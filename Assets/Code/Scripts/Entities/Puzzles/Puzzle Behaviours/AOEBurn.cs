using System.Collections;
using UnityEngine;

public class AOEBurn : AOEEffect
{
    [Header("Burn Effect Setup")] 
    [SerializeField] private float _damageTime;

    private Coroutine _damageTimer;
    
    public override void StartEffect()
    {
        __isActive = true;
        StartDamageTimer();
        
        StartPlaceHolderVFX(true);
    }

    public override void EndEffect()
    {
        print("ending effect");
        
        __isActive = false;
        StopAllCoroutines();

        _damageTimer = null;
        
        StartPlaceHolderVFX(false);
        
        __targets.Clear();
    }
    
    private void StartDamageTimer()
    {
        if (_damageTimer != null) return;

        _damageTimer = StartCoroutine(DamageTimer(_damageTime));
    }
    
    private IEnumerator DamageTimer(float time)
    {
        yield return DamageRoutine(time);

        if (__isActive) yield return DamageRoutine(time);

        _damageTimer = null;
    }

    private IEnumerator DamageRoutine(float time)
    {
        print("Starting damage timer");
        print(time);
        
        yield return new WaitForSeconds(time);
            
        print("Starting to Check Targets");
        
        for (int i = __targets.Count - 1; i >= 0; i--)
        {
            print("Check Targets");
            
            if ((int)__aoeTargetType == -1 || __targets[i].Weaknesses[0].WeakType == __aoeTargetType)
            {
                __targets[i].OnShot(__targets[i].Weaknesses[0], __targets[i].Weaknesses[0].WeakType);

                if (__targets[i] == null || __targets[i].Weaknesses.Count < 1)
                {
                    __targets.RemoveAt(i);
                }
            }
        }

        print("Checked Targets");
    }
}