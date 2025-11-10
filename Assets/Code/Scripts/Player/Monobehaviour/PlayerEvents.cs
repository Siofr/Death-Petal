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

public struct RemoveBulletEvent : IEvent { }
public struct EndLongReload : IEvent { }
public struct StartLongReload : IEvent { }

public class PlayerEvents : MonoBehaviour
{
    
}
