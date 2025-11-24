using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour, IEntity
{
    //Non-Serializable Fields
    private List<Weakness> _weaknesses = new List<Weakness>();
    
    //Properties
    public List<Weakness> Weaknesses => _weaknesses;

    protected virtual void Awake()
    {
        InitialiseWeaknesses();
    }
    
    public void InitialiseWeaknesses()
    {
        if (_weaknesses == null) return;
        
        var weaknesses = GetComponentsInChildren<Weakness>();

        if (weaknesses == null) return;
        
        foreach (var weakness in weaknesses)
        {
            _weaknesses.Add(weakness);
        }
    }

    public void IntitialiseWeaknesses(List<Weakness> weaknesses)
    {
        _weaknesses = weaknesses;
    }
    
    public abstract void OnShot(Weakness weakness, WeakTypes damageType);
}