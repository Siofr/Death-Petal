using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Save Data")]
public class EnemySaveData : SaveData
{
    public List<Weakness> health;

    public void Save(Vector3 pos, List<Weakness> weaknesses)
    {
        Save(pos);
        health = weaknesses;
    }

    public void Load(Transform refTransform, List<Weakness> weaknesses)
    {
        if (health == null || health.Count < 1)
        {
            refTransform.gameObject.SetActive(false);
            return;
        }
        
        Load(refTransform);
        weaknesses = health;
    }
}