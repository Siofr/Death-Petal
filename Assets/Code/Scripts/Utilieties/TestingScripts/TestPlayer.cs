using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Destroy(weakness.transform.parent.gameObject);   
        }
        
        if (Weaknesses.Count < 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    public SaveData GetSaveData(LevelData levelData)
    {
        if (_saveData == null)
        {
            var dataInstance = ScriptableObject.CreateInstance<PlayerSaveData>();
            AssetDatabase.CreateAsset(dataInstance, levelData.AssetSavePath + $"/{gameObject.name}SaveData.asset");
            
            _saveData = dataInstance;
            _saveData.Save(transform.position, Weaknesses);
        }
        
        return _saveData;
    }

    public void SaveData()
    {
        _saveData.Save(transform.position, new List<Weakness>());
    }

    public void LoadSaveData(SaveData levelData)
    {
        _saveData = (PlayerSaveData)levelData;

        _saveData.Load(transform, base.Weaknesses);
    }
}