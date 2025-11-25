using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Save Data")]
public class PlayerSaveData : SaveData
{
    public List<WeakTypes> health;

    public void Save(Vector3 pos, List<Weakness> weaknesses)
    {
        Save(pos);

        health.Clear();
        
        foreach (var weakness in weaknesses)
        {
            health.Add(weakness.WeakType);
        }
    }

    public void Load(Transform refTransform, List<Weakness> weaknesses)
    {
        Load(refTransform);

        for (int i = weaknesses.Count - 1; i >= 0; i--)
        {
            if (i > health.Count - 1) continue;

            weaknesses[i].RemoveWeakType(health[i]);
        }
    }
}