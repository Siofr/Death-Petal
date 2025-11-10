using UnityEngine;

public struct ShootEvent : IEvent
{
    public Weakness weakness;

    public ShootEvent(Weakness newWeakness)
    {
        this.weakness = newWeakness;
    }
}

public class PlayerWeaponEvents : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
