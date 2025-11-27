using UnityEngine;

public struct NextBulletEvent : IEvent
{
    public BulletSO bulletType;

    public NextBulletEvent(BulletSO bulletType)
    {
        this.bulletType = bulletType;
    }
}

public struct ShootEvent : IEvent
{
    public Weakness weakness;

    public ShootEvent(Weakness newWeakness)
    {
        this.weakness = newWeakness;
    }
}

public struct SpawnTrail : IEvent
{
    public Color bulletColor;

    public SpawnTrail(Color newColor)
    {
        this.bulletColor = newColor;
    }
}

public struct QuickReload : IEvent
{

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
