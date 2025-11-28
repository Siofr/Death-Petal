using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour, IEntity, ISaveable<EntitySaveData>
{
    [Header("BaseEntityFields")]
    [SerializeField] private EntitySaveData _saveData;
    public List<Weakness> weaknesses = new List<Weakness>();
    
    
    //Non-Serialized Fields
    private bool _isInitialized;
    private int _saveID;
    
    //Properties
    public List<Weakness> Weaknesses => weaknesses;
    public EntitySaveData SaveInfo =>  _saveData;
    public int SaveID => _saveID;

    protected virtual void Awake()
    {
        _saveID = _saveData.id;
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
    
    
    public void CreateSaveInstance()
    {
        _saveID = ISaveableHelper.GenerateISaveableID();
        ReInitializeWeaknesses();
        
        var health = new List<int>();
        Weaknesses.ForEach(x=>health.Add((int)x.WeakType));
        
        _saveData = new EntitySaveData(_saveID, transform.position, health);
    }

    public void DeleteSaveInstance()
    {
        if (SaveID == 0) return;
        ISaveableHelper.RemoveExistingID(ref _saveID);
        
        _saveData = new EntitySaveData();
    }
    
    public void HandleLoadData(ref LevelSaveData refData)
    {
        if (!refData.saveableID.Contains(SaveID)) return;

        foreach (var data in refData.entitySaveData)
        {
            if (data.id != SaveID) continue;
            
            ReInitializeWeaknesses();
            
            _saveData = data;
            _saveData.Load(transform, ref weaknesses);
            
            if(data.health.Count > 0) ReInitializeWeaknesses();
            return;
        }
    }

    public void HandleSaveData(ref LevelSaveData refData)
    {
        if (!refData.saveableID.Contains(SaveID)) return;
        
        _saveData.Save(transform.position, Weaknesses);

        for (var i = 0; i < refData.entitySaveData.Count; i++)
        {
            if (refData.entitySaveData[i].id != SaveID) continue;

            refData.entitySaveData[i] = _saveData;
            return;
        }
        
        refData.entitySaveData.Add(_saveData);
    }
}