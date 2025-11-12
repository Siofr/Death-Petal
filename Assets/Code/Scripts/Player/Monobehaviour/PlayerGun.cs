using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    private BulletSO[] bulletArray = new BulletSO[6];
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

    public void Awake()
    {
        _shootEventListener = new EventBindings<ShootEvent>(ShootBullet);
        _addBulletEventListener = new EventBindings<AddBulletEvent>(AddBullet);
        _removeEventListener = new EventBindings<RemoveBulletEvent>(RemoveBullet);
    }

    private void OnEnable()
    {
        EventBus<ShootEvent>.Register(_shootEventListener);
        EventBus<AddBulletEvent>.Register(_addBulletEventListener);
        EventBus<RemoveBulletEvent>.Register(_removeEventListener);
    }

    public void ShootBullet(ShootEvent ctx)
    {
        // I want to shoot element at 0 always

        if (bulletArray[0] == null) return;

        // Now remove it
        Debug.Log("GUN Shooting" + bulletArray[0].bulletTypeName);
        ctx.weakness?.ParentEntity.OnShot(ctx.weakness, bulletArray[0].weakness);
        bulletArray[0] = null;

        // Now Reorder it
        bulletArray = ReorderArray(bulletArray);
    }

    public void AddBullet(AddBulletEvent ctx)
    {
        if (bulletIndex >= bulletArray.Length) return;

        Debug.Log("GUN Adding" + ctx.bulletType.bulletTypeName);

        bulletArray[bulletIndex] = ctx.bulletType;
        bulletIndex++;
    }

    public void RemoveBullet()
    {
        if (bulletIndex - 1 < 0) return;

        // I want to remove the latest element

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
}
