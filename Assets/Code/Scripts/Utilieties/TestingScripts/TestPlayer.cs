using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct PlayerDamageEvent: IEvent{ }

public struct PlayerDamagedEvent : IEvent
{
    
}

public class TestPlayer : EntityBase, ISaveable<EntitySaveData>
{
    [SerializeField] private EntitySaveData _saveData;
    
    private int _saveID;
    public EntitySaveData SaveInfo =>  _saveData;
    public int SaveID => _saveID;
    
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        if (!Weaknesses.Contains(weakness)) return;

        if (damageType == WeakTypes.PLAYER)
        {
            EventBus<PlayerDamageEvent>.Raise(new PlayerDamageEvent());
            Weaknesses.Remove(weakness);

            Destroy(weakness.transform.parent.gameObject);   
            EventBus<PlayerDamagedEvent>.Raise(new PlayerDamagedEvent());

        }
        
        if (Weaknesses.Count < 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    public void CreateSaveInstance()
    {
        _saveID = ISaveableHelper.GenerateISaveableID();

        var health = new List<int>();
        Weaknesses.ForEach(x=>health.Add((int)x.WeakType));
        
        _saveData = new EntitySaveData(_saveID, transform.position, health);
    }

    public void DeleteSaveInstance()
    {
        if (SaveID == 0) return;
        ISaveableHelper.RemoveExistingID(_saveID);
        
        _saveID = 0;
        _saveData = new EntitySaveData();
    }
    
    public void HandleLoadData(ref LevelSaveData refData)
    {
        if (!refData.saveableID.Contains(SaveID)) return;

        foreach (var data in refData.entitySaveData)
        {
            if (data.id != SaveID) continue;

            _saveData = data;
            _saveData.Load(transform, ref weaknesses);
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