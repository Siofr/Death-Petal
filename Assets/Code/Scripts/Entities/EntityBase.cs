using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class EntityBase : MonoBehaviour, IEntity
{
    //Non-Serializable Fields
    private List<Weakness> _weaknesses = new List<Weakness>();
    
    //Properties
    public List<Weakness> Weaknesses => _weaknesses;
    
    [FormerlySerializedAs("_sequentialWeaknesses")]
    [Header("Entity Sequential Fields")]
    [SerializeField] protected bool __sequentialWeaknesses;
    public List<WeakTypes> defaultWeaknessTypes;
    
    protected virtual void Awake()
    {
        InitialiseWeaknesses();
    }
    
    public virtual void InitialiseWeaknesses()
    {
        if (_weaknesses == null) return;
        
        var weaknesses = GetComponentsInChildren<Weakness>();

        if (weaknesses == null) return;
        
        _weaknesses.Clear();
        
        foreach (var weakness in weaknesses)
        {
            _weaknesses.Add(weakness);
        }
        
        if (!__sequentialWeaknesses) return;
        
        defaultWeaknessTypes.Clear();
        
        if (Weaknesses.Count > 0)
        {
            for (int i = 0; i < Weaknesses.Count; i++)
            {
                defaultWeaknessTypes.Add(Weaknesses[i].WeakType);

                if (i == 0) continue;
                
                Weaknesses[i].SetWeakType(WeakTypes.PLAYER);
                Weaknesses[i].ToggleHitbox(false);
            }
        }
    }

    public void IntitialiseWeaknesses(List<Weakness> weaknesses)
    {
        _weaknesses = weaknesses;
    }
    
    public abstract void OnShot(Weakness weakness, WeakTypes damageType);
}