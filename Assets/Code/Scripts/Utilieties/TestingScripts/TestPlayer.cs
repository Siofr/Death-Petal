using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct PlayerDamageEvent: IEvent{ }

public struct PlayerDamagedEvent : IEvent
{
    
}

public class TestPlayer : EntityBase, ISaveable<PlayerSaveData>
{
    [SerializeField]
    private PlayerSaveData _saveData;
    public PlayerSaveData SaveInfo => _saveData;
    
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

    public SaveData GetSaveInfo()
    {
        if (_saveData == null) _saveData = ScriptableObject.CreateInstance<PlayerSaveData>();
        
        return _saveData;
    }

    public void LoadData(SaveData saveData)
    {
        _saveData = (PlayerSaveData)saveData;
        
        _saveData.Load(transform, weaknesses);
    }

    public void SaveData()
    {
        _saveData.Save(transform.position, weaknesses);
    }
}