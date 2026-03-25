using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SphereCollider))]
public abstract class AOEEffect: MonoBehaviour
{
    [FormerlySerializedAs("_isActive")]
    [Header("AOE Effect Fields")]
    [SerializeField] protected WeakTypes __aoeTargetType;

    [Header("PLACEHOLDER VFX")] [SerializeField]
    private GameObject _aoeSphereEffect;
    
    //Non-Serializable Fields
    protected List<EnemyBase> __targets = new List<EnemyBase>();
    protected SphereCollider __col;
    protected bool __isActive;
    
    public abstract void StartEffect();
    public abstract void EndEffect();
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent(out EnemyBase enemy))
        {
            if(!__targets.Contains(enemy))
            {
                __targets.Add(enemy);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.TryGetComponent(out EnemyBase enemy))
        {
            if (__targets.Contains(enemy))
            {
                __targets.Remove(enemy);
            }
        }
    }

    private void Awake()
    {
        __col = GetComponent<SphereCollider>();
    }
        
    //TO CHANGE
    
    protected void StartPlaceHolderVFX(bool toggle)
    {
        if (_aoeSphereEffect == null) return;
        
        StopAllCoroutines();
        StartCoroutine(PlaceHolderVFX(toggle));
    }

    private IEnumerator PlaceHolderVFX(bool toggle)
    {
        float targetScale = toggle ? __col.radius * 2 : 0f;
        float lerp = 0f;
        
        _aoeSphereEffect.SetActive(toggle);
        
        Vector3 targetValue = new Vector3(targetScale, targetScale, targetScale);
        Vector3 initScale = _aoeSphereEffect.transform.localScale;
        
        while (lerp < .5f)
        {
            lerp += Time.deltaTime * 2;

            _aoeSphereEffect.transform.localScale = Vector3.Lerp(initScale, targetValue, lerp);

            yield return null;
        }

        _aoeSphereEffect.transform.localScale = targetValue;
    }
}



