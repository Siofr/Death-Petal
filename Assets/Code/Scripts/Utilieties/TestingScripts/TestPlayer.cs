using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour, IEntity, ISaveable<PlayerSaveData>
{
    public PlayerSaveData SaveInfo { get; }
    
    private List<Weakness> _weaknesses = new List<Weakness>();

    public List<Weakness> Weaknesses => _weaknesses;
    
    public void OnShot(Weakness weakness, WeakTypes damageType)
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

    public void LoadSaveData()
    {
    }

    public void SaveData()
    {
    }
}