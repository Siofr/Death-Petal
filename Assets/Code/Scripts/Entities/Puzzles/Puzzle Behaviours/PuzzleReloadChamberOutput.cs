using JetBrains.Annotations;
using UnityEngine;

public struct SetChamberEvent : IEvent
{
    public BulletSO[] bulletOrder;

    public SetChamberEvent(BulletSO[] bullets) =>  bulletOrder = bullets;
}

public class PuzzleReloadChamberOutput: PuzzleOutputBase
{
    [Header("Circle Reload - Bullet Prefabs")]
    [SerializeField] private BulletSO[] bulletPrefabs;
    
    [Header("Circle Reload - Bullets Set in Shooting Order\nOnly Elements 1 - 5 Valid")] 
    [SerializeField] private WeakTypes[] gunChamberFill;
    
    public override void OnPuzzleSolved(PuzzleSolvedEvent context)
    {
        EventBus<SetChamberEvent>.Raise(new SetChamberEvent(SetChamber()));
        base.OnPuzzleSolved(context);
    }

    private BulletSO[] SetChamber()
    {
        BulletSO[] result = new BulletSO[6];

        for (int i = 0; i < gunChamberFill.Length; i++)
        {
            if(i > 5) break;

            BulletSO value;
            
            switch (gunChamberFill[i])
            {
                case WeakTypes.RED:
                    value = bulletPrefabs[0];
                    break;
                case WeakTypes.GREEN:
                    value = bulletPrefabs[1];
                    break;
                case WeakTypes.BLUE:
                    value = bulletPrefabs[2];
                    break;
                default:
                    value = null;
                    break;
            }

            result[i] = value;
        }
        
        return result;
    }
}