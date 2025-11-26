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

    private int _saveID = ISaveableHelper.GenerateISaveableID();
    
    public EntitySaveData SaveInfo => _saveData;
    public int SaveID
    {
        get => _saveID;
    }
    
    public void Awake()
    {
        base.Awake();
    }

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

    public void HandleLoadData()
    {
        _saveData.Load(transform, ref weaknesses);
    }

    public void HandleSaveData()
    {
        _saveData.Save(transform.position, Weaknesses);
    }
}