using UnityEngine;

public abstract class PlayerWeapon : MonoBehaviour
{
    public abstract BulletSO[] currentBarrel { get; set; }
    public abstract BulletSO[] lastBarrel { get; set; }
    public abstract int currentIndex { get; set; }
    public PlayerWeaponSO weaponData;

    public EventBindings<AddBulletEvent> addBulletListener;
    public EventBindings<RemoveBulletEvent> removeBulletListener;
    public EventBindings<ShootEvent> shootListener;


    protected void Start()
    {
        currentBarrel = new BulletSO[weaponData.maxBullets];
        lastBarrel = new BulletSO[weaponData.maxBullets];
    }

    protected void Awake()
    {
        addBulletListener = new EventBindings<AddBulletEvent>(AddBullet);
        removeBulletListener = new EventBindings<RemoveBulletEvent>(RemoveBullet);
        shootListener = new EventBindings<ShootEvent>(Shoot);
    }

    protected void OnEnable()
    {
        EventBus<AddBulletEvent>.Register(addBulletListener);
    }

    public abstract void Shoot(ShootEvent ctx);

    public abstract void QuickReload();

    public abstract void EndReload();

    public abstract void RemoveBullet();

    public abstract void AddBullet(AddBulletEvent ctx);

    public abstract void ClearBullets();
}
