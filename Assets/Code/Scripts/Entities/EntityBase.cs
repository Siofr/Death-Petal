using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour, IEntity
{
    [Header("BaseEntityFields")]
    public List<Weakness> weaknesses = new List<Weakness>();
    
    //Non-Serialized Fields
    private bool _isInitialized;
    
    //Properties
    public List<Weakness> Weaknesses => weaknesses;
    
    protected virtual void Awake()
    {
        InitialiseWeaknesses();
    }
    
    public void InitialiseWeaknesses()
    {
        if (weaknesses == null || _isInitialized) return;
        
        var _weaknesses = GetComponentsInChildren<Weakness>();

        if (_weaknesses == null) return;
        
        foreach (var weakness in _weaknesses)
        {
            weaknesses.Add(weakness);
            weakness.Initialize(weakness.WeakType);
        }
        
        _isInitialized = true;
    }

    public void ReInitializeWeaknesses()
    {
        weaknesses.Clear();
        _isInitialized = false;
        
        InitialiseWeaknesses();
    }
    
    
    public abstract void OnShot(Weakness weakness, WeakTypes damageType);
}