using UnityEngine;

public class PlayerRevolver : PlayerWeapon
{
    public override BulletSO[] currentBarrel { get; set; }
    public override BulletSO[] lastBarrel { get; set; }
    public override int currentIndex { get; set; }

    public override void Shoot(ShootEvent ctx)
    {
        if (currentIndex < 0) return;

        // ctx.weakness.ParentEntity.OnShot(currentBarrel[currentIndex].weakness);
        currentIndex -= 1;
    }

    public override void QuickReload()
    {
        if (lastBarrel.Length <= 0) return;

        for (int i = 0; i <= lastBarrel.Length; i++)
        {
            EventBus<AddBulletEvent>.Raise(new AddBulletEvent(lastBarrel[i]));
        }
    }

    public override void EndReload()
    {
        if (currentIndex >= currentBarrel.Length) return;

        for (int i = currentIndex; i <= currentBarrel.Length; i++)
        {
            EventBus<RotateBarrelEvent>.Raise(new RotateBarrelEvent(1));
        }
    }

    public override void RemoveBullet()
    {
        if (currentIndex - 1 < 0) return;

        currentBarrel[currentIndex - 1] = null;
        currentIndex -= 1;
    }

    public override void AddBullet(AddBulletEvent ctx)
    {
        if (currentIndex + 1 > weaponData.maxBullets - 1) return;

        currentBarrel[currentIndex + 1] = ctx.bulletType;
        currentIndex += 1;
    }

    public override void ClearBullets()
    {
        for (int i = currentIndex; i > 0; i--)
        {
            EventBus<RemoveBulletEvent>.Raise(new RemoveBulletEvent());
        }
    }
}
