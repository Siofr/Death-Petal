public class EntityHelper
{
    static public void LockAllInputs()
    {
        EventBus<LockInput>.Raise(new LockInput("Move"));
        EventBus<LockInput>.Raise(new LockInput("Attack"));
        EventBus<LockInput>.Raise(new LockInput("Look"));
        EventBus<LockInput>.Raise(new LockInput("Aim"));
        EventBus<LockInput>.Raise(new LockInput("North"));
        EventBus<LockInput>.Raise(new LockInput("South"));
        EventBus<LockInput>.Raise(new LockInput("West"));
        EventBus<LockInput>.Raise(new LockInput("BarrelRight"));
        EventBus<LockInput>.Raise(new LockInput("BarrelLeft"));
    }

    static public void UnlockAllInputs()
    {
        EventBus<UnlockInput>.Raise(new UnlockInput("Move"));
        EventBus<UnlockInput>.Raise(new UnlockInput("Attack"));
        EventBus<UnlockInput>.Raise(new UnlockInput("Look"));
        EventBus<UnlockInput>.Raise(new UnlockInput("Aim"));
        EventBus<UnlockInput>.Raise(new UnlockInput("North"));
        EventBus<UnlockInput>.Raise(new UnlockInput("South"));
        EventBus<UnlockInput>.Raise(new UnlockInput("West"));
        EventBus<UnlockInput>.Raise(new UnlockInput("BarrelRight"));
        EventBus<UnlockInput>.Raise(new UnlockInput("BarrelLeft"));
    }
}