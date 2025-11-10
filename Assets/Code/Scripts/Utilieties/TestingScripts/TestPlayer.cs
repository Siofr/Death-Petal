using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour, IEntity
{
    private List<Weakness> _weaknesses = new List<Weakness>();

    public List<Weakness> Weaknesses => _weaknesses;
    
    public void OnShot(Weakness weakness)
    {
        if (!Weaknesses.Contains(weakness)) return;
        
        Weaknesses.Remove(weakness);
        Destroy(weakness.gameObject);
        
        if (Weaknesses.Count < 1)
        {
            Destroy(gameObject);
        }
    }
}