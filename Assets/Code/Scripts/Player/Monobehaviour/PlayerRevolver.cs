using UnityEngine;

public class PlayerRevolver : PlayerWeapon
{
    public override BulletSO[] currentBarrel { get; set; }
    public override BulletSO[] lastBarrel { get; set; }
    public override int currentIndex { get; set; }

    public override void Shoot(ShootEvent ctx)
    {
        if (currentIndex < 0 || !currentBarrel[currentIndex]) { return; }
        Debug.Log("Color" + currentBarrel[currentIndex].bulletTypeName);
        BulletSO currentBullet = currentBarrel[currentIndex];
        EventBus<RemoveBulletEvent>.Raise(new RemoveBulletEvent(0, 1));

        if (ctx.weakness == null) { return; }

        ctx.weakness.ParentEntity.OnShot(ctx.weakness, currentBullet.weakness);
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

        for (int i = currentIndex; i < currentBarrel.Length; i++)
        {
            EventBus<RotateBarrelEvent>.Raise(new RotateBarrelEvent(1));
        }
    }

    public override void RemoveBullet(RemoveBulletEvent ctx)
    {
        if (currentIndex + ctx.bulletIndex < 0) return;

        currentBarrel[currentIndex + ctx.bulletIndex] = null;
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
            EventBus<RemoveBulletEvent>.Raise(new RemoveBulletEvent(0, -1));
        }
    }

    private void MoveArrayUp(BulletSO[] arr)
    {

    }
}
