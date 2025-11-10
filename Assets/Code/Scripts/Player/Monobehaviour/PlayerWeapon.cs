using UnityEngine;

public struct EndLongReload : IEvent { }
public struct StartLongReload : IEvent { }
public struct AddBulletEvent : IEvent 
{
    public BulletSO bulletType;

    public AddBulletEvent(BulletSO newBullet)
    {
        this.bulletType = newBullet;
    }
}
public struct RemoveBullet : IEvent { }

public abstract class PlayerWeapon : MonoBehaviour
{
    public abstract BulletSO[] currentBarrel { get; set; }
    public abstract BulletSO[] lastBarrel { get; set; }
    public abstract int currentIndex { get; set; }
    public abstract int maxBullets { get; set; }

    public EventBindings<AddBulletEvent> addBulletListener;
    public EventBindings<RemoveBullet> removeBulletListener;


    protected void Start()
    {
        currentBarrel = new BulletSO[maxBullets];
        lastBarrel = new BulletSO[maxBullets];
    }

    protected void Awake()
    {
        addBulletListener = new EventBindings<AddBulletEvent>(AddBullet);
        removeBulletListener = new EventBindings<RemoveBullet>(RemoveBullet);
    }

    protected void OnEnable()
    {
        
    }

    public abstract void Shoot(Transform activeTarget);

    public abstract void QuickReload();

    public abstract void EndReload();

    public abstract void RemoveBullet();

    public abstract void AddBullet(AddBulletEvent ctx);

    public abstract void ClearBullets();
}
