using UnityEngine;

// Player Controller Events
public struct ActiveTargetEvent : IEvent
{
    public Transform activeTarget;

    public ActiveTargetEvent(Transform newTarget)
    {
        activeTarget = newTarget;
    }
}

public struct TransmitPlayerInfo : IEvent
{
    public Transform player;

    public TransmitPlayerInfo(Transform player)
    {
        this.player = player;
    }
}

public struct AimEvent : IEvent { }

// Reload Events
public struct AddBulletEvent : IEvent
{
    public BulletSO bulletType;

    public AddBulletEvent(BulletSO newBullet)
    {
        this.bulletType = newBullet;
    }
}
public struct RotateBarrelEvent : IEvent
{
    public int direction;

    public RotateBarrelEvent(int newDirection)
    {
        this.direction = newDirection;
    }
}

public struct RemoveBulletEvent : IEvent
{
    public int bulletIndex;
    public int direction;

    public RemoveBulletEvent(int newBulletIndex, int newDirection)
    {
        this.bulletIndex = newBulletIndex;
        this.direction = newDirection;
    }
}
public struct EndLongReload : IEvent { }
public struct StartLongReload : IEvent { }

public class PlayerEvents : MonoBehaviour
{
    
}
