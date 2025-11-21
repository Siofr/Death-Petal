using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Save Data")]
public class PlayerSaveData : SaveData
{
    public List<Weakness> health;

    public void Save(Vector3 @position, List<Weakness> @health)
    {
        Save(@position);
        this.health = health;
    }

    public void Load(Transform refTransform, List<Weakness> refHealth)
    {
        Load(refTransform);
        refHealth = this.health;
    }
}