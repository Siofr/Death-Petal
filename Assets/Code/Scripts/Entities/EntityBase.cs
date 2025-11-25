using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour, IEntity
{
    [Header("BaseEntityFields")]
    public List<Weakness> weaknesses = new List<Weakness>();
    
    //Properties
    public List<Weakness> Weaknesses => weaknesses;

    protected virtual void Awake()
    {
        InitialiseWeaknesses();
    }
    
    public void InitialiseWeaknesses()
    {
        if (weaknesses == null) return;
        
        var _weaknesses = GetComponentsInChildren<Weakness>();

        if (_weaknesses == null) return;
        
        foreach (var weakness in _weaknesses)
        {
            weaknesses.Add(weakness);
        }
    }

    public void IntitialiseWeaknesses(List<Weakness> _weaknesses)
    {
        weaknesses = _weaknesses;
    }
    
    public abstract void OnShot(Weakness weakness, WeakTypes damageType);
}