using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    private BulletSO[] bulletArray = new BulletSO[6];
    private BulletSO[] lastBulletArray = new BulletSO[6];
    private int bulletIndex = 0;

    private void PrintContents(BulletSO[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            Debug.Log(arr[i]);
        }
    }

    private EventBindings<ShootEvent> _shootEventListener;
    private EventBindings<AddBulletEvent> _addBulletEventListener;
    private EventBindings<RemoveBulletEvent> _removeEventListener;
    private EventBindings<StartLongReload> _startLongReloadListener;
    private EventBindings<EndLongReload> _endLongReloadListener;
    private EventBindings<QuickReload> _quickReloadListener;

    public void Awake()
    {
        _shootEventListener = new EventBindings<ShootEvent>(ShootBullet);
        _addBulletEventListener = new EventBindings<AddBulletEvent>(AddBullet);
        _removeEventListener = new EventBindings<RemoveBulletEvent>(RemoveBullet);
        _startLongReloadListener = new EventBindings<StartLongReload>(Initialize);
        _endLongReloadListener = new EventBindings<EndLongReload>(SaveArray);
        _quickReloadListener = new EventBindings<QuickReload>(QuickReload);
    }

    private void OnEnable()
    {
        EventBus<ShootEvent>.Register(_shootEventListener);
        EventBus<AddBulletEvent>.Register(_addBulletEventListener);
        EventBus<RemoveBulletEvent>.Register(_removeEventListener);
        EventBus<StartLongReload>.Register(_startLongReloadListener);
        EventBus<EndLongReload>.Register(_endLongReloadListener);
        EventBus<QuickReload>.Register(_quickReloadListener);
    }

    public void Initialize(StartLongReload ctx)
    {
        bulletIndex = 0;
    }

    private void QuickReload()
    {
        bulletIndex = 0;
        bulletArray = CopyArray(lastBulletArray);
        Debug.Log("Bullets Loaded" + bulletArray);
    }

    public void ShootBullet(ShootEvent ctx)
    {
        // I want to shoot element at 0 always

        if (bulletArray[0] == null) return;

        EventBus<SpawnTrail>.Raise(new SpawnTrail(bulletArray[0].bulletColor));

        // Now remove it
        ctx.weakness?.ParentEntity.OnShot(ctx.weakness, bulletArray[0].weakness);
        bulletArray[0] = null;

        // Now Reorder it
        bulletArray = ReorderArray(bulletArray);
    }

    public void AddBullet(AddBulletEvent ctx)
    {
        if (bulletIndex >= bulletArray.Length) return;

        bulletArray[bulletIndex] = ctx.bulletType;
        bulletIndex++;
    }

    public void RemoveBullet()
    {
        if (bulletIndex - 1 < 0) return;

        bulletArray[bulletIndex - 1] = null;
        bulletIndex--;
    }

    public BulletSO[] ReorderArray(BulletSO[] arr)
    {
        // Reorder the array
        BulletSO[] newArr = new BulletSO[arr.Length];

        for (int i = 0; i < arr.Length - 1; i++)
        {
            newArr[i] = arr[i + 1];
        }

        newArr[arr.Length - 1] = null;
        return newArr;
    }

    private void SaveArray()
    {
        lastBulletArray = CopyArray(bulletArray);

        Debug.Log("Bullets loaded" + lastBulletArray);
    }

    private BulletSO[] CopyArray(BulletSO[] arr)
    {
        BulletSO[] newArr = new BulletSO[6];

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == null) break;
            newArr[i] = arr[i];
        }

        return newArr;
    }
}
