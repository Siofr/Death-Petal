using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestPlayer : EntityBase, IEntity, ISaveable<PlayerSaveData>
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
            Weaknesses.Remove(weakness);
            Destroy(weakness.gameObject);   
        }
        
        if (Weaknesses.Count < 1)
        {
            Destroy(gameObject);
        }
    }

    public SaveData GetSaveData(LevelData levelData)
    {
        if (_saveData == null)
        {
            var dataInstance = ScriptableObject.CreateInstance<PlayerSaveData>();
            AssetDatabase.CreateAsset(dataInstance, levelData.AssetSavePath + $"/{gameObject.name}SaveData.asset");
            
            _saveData = dataInstance;
            _saveData.Save(transform.position, base.Weaknesses);
        }
        
        return _saveData;
    }

    public void SaveData()
    {
        Debug.Log("Saving");
        _saveData.Save(transform.position, new List<Weakness>());
    }

    public void LoadSaveData(SaveData levelData)
    {
        _saveData = (PlayerSaveData)levelData;

        _saveData.Load(transform, base.Weaknesses);
    }
}