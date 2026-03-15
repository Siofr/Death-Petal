using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class EntityBase : MonoBehaviour, IEntity, ISaveable<EntitySaveData>
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
    
    public void ToggleAllWeaknessIcons(bool toggle)
    {
        foreach(var weakness in Weaknesses) weakness.ToggleIcon(toggle);
    }
    
    public void IntitialiseWeaknesses(List<Weakness> weaknesses)
    {
        _weaknesses = weaknesses;
    }
    
    public abstract void OnShot(Weakness weakness, WeakTypes damageType);
    
    //Saving
    private EntitySaveData _saveData;
    private int _saveID;
    
    public EntitySaveData SaveInfo =>  _saveData;
    public int SaveID => _saveID;

    public void CreateSaveInstance()
    {
        _saveID = ISaveableHelper.GenerateISaveableID();
        InitialiseWeaknesses();
        
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
            
            InitialiseWeaknesses();
            
            _saveData = data;
            _saveData.Load(transform, ref _weaknesses);
            
            if(data.health.Count > 0) InitialiseWeaknesses();
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