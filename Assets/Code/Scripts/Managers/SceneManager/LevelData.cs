using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/Level Data")]
public class LevelData : ScriptableObject
{
    private ISaveable[] _saveables;
    
    public void SaveLevelData()
    {
        _saveables = FindSaveables();

        if (_saveables.Length < 1) return;
        
        foreach (var saveable in _saveables)
        {
            saveable.SaveData();
        }
        
        Debug.Log("Saved LevelData");
    }

    public void LoadLevelData()
    {
        if (_saveables == null || _saveables.Length < 1 || _saveables != FindSaveables()) return;
        
        foreach (var saveable in _saveables)
        {
            saveable.LoadSaveData();
        }
    }

    public void ClearLevelData()
    {
        _saveables = null;
    }
    
    private static ISaveable[] FindSaveables()
    {
        ISaveable[] results = { };

        return results;
    }
}