using UnityEngine;
using FMOD.Studio;
using SFXUtil;
using FMODUnity;

public class PlayerGun : MonoBehaviour
{
    public EventReference shootSfxEventPath;
    private EventInstance _shootEvent;
    private PARAMETER_ID _bulletsLeft;

    public EventReference AddRemoveSFXEvent;
    private EventInstance _addRemoveEvent;
    private PARAMETER_ID _addRemove;

    private BulletSO[] bulletArray = new BulletSO[6];
    private BulletSO[] lastBulletArray = new BulletSO[6];
    private int bulletIndex = 0;
    private int lastBulletIndex;
    private int currentChamber = 0;

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
    private EventBindings<RotateBarrelEvent> _rotateBarrelEventListener;

    public void Awake()
    {
        _shootEventListener = new EventBindings<ShootEvent>(ShootBullet);
        _addBulletEventListener = new EventBindings<AddBulletEvent>(AddBullet);
        _removeEventListener = new EventBindings<RemoveBulletEvent>(RemoveBullet);
        _rotateBarrelEventListener = new EventBindings<RotateBarrelEvent>(OnRotateBarrel);
    }

    private void OnEnable()
    {
        EventBus<ShootEvent>.Register(_shootEventListener);
        EventBus<AddBulletEvent>.Register(_addBulletEventListener);
        EventBus<RemoveBulletEvent>.Register(_removeEventListener);
        EventBus<RotateBarrelEvent>.Register(_rotateBarrelEventListener);
    }

    private void OnDisable()
    {
        EventBus<ShootEvent>.Unregister(_shootEventListener);
        EventBus<AddBulletEvent>.Unregister(_addBulletEventListener);
        EventBus<RemoveBulletEvent>.Unregister(_removeEventListener);
        EventBus<RotateBarrelEvent>.Unregister(_rotateBarrelEventListener);
    }

    private void Start()
    {
        _bulletsLeft = SFXUtilities.AssignParamID("BulletLeft", shootSfxEventPath);
        _shootEvent = SFXUtilities.CreateEventInstance(shootSfxEventPath, this.gameObject);

        _addRemove = SFXUtilities.AssignParamID("AddRemoveBullet", AddRemoveSFXEvent);
        _addRemoveEvent = SFXUtilities.CreateEventInstance(AddRemoveSFXEvent, this.gameObject);
    }

    public void Initialize(StartLongReload ctx)
    {
        // bulletIndex = 0;
    }

    public void ShootBullet(ShootEvent ctx)
    {
        // I want to shoot element at 0 always
        _shootEvent.setParameterByID(_bulletsLeft, 5);

        if (bulletArray[currentChamber] == null)
        {
            _shootEvent.setParameterByID(_bulletsLeft, 0);
            EventBus<HapticFeedbackEvent>.Raise(new HapticFeedbackEvent(0.0f, 0.5f, 0.15f));
            EventBus<SFXEventTrigger>.Raise(new SFXEventTrigger(_shootEvent, this.gameObject));
            return;
        }

        EventBus<SFXEventTrigger>.Raise(new SFXEventTrigger(_shootEvent, this.gameObject));

        EventBus<SpawnTrail>.Raise(new SpawnTrail(bulletArray[currentChamber].bulletColor));
        EventBus<HapticFeedbackEvent>.Raise(new HapticFeedbackEvent(0.5f, 0.0f, 0.25f));

        // Now remove it
        if (ctx.weakness)
        {
            ctx.weakness.ParentEntity.OnShot(ctx.weakness, bulletArray[currentChamber].weakness);
        }

        bulletArray[currentChamber] = null;
        RotateBarrel(-1);
        GetNextBullet();
    }

    public void AddBullet(AddBulletEvent ctx)
    {
        if (bulletArray[currentChamber] != null) return;

        _addRemoveEvent.setParameterByID(_addRemove, 1);
        EventBus<HapticFeedbackEvent>.Raise(new HapticFeedbackEvent(0.0f, 0.05f, 0.15f));
        EventBus<SFXEventTrigger>.Raise(new SFXEventTrigger(_addRemoveEvent, this.gameObject));

        bulletArray[currentChamber] = ctx.bulletType;
        GetNextBullet();

        if (TEMP_ReloadTesting.Instance.manualRotate)
        {
            return;
        }

        RotateBarrel(1);
    }

    public void RemoveBullet()
    {
        // if (bulletIndex - 1 < 0) return;
        if (bulletArray[currentChamber] == null) return;

        _addRemoveEvent.setParameterByID(_addRemove, 0);
        EventBus<HapticFeedbackEvent>.Raise(new HapticFeedbackEvent(0.05f, 0.0f, 0.15f));
        EventBus<SFXEventTrigger>.Raise(new SFXEventTrigger(_addRemoveEvent, this.gameObject));

        bulletArray[currentChamber] = null;
        GetNextBullet();

        if (TEMP_ReloadTesting.Instance.manualRotate)
        {
            return;
        }

        RotateBarrel(-1);
    }

    public void OnRotateBarrel(RotateBarrelEvent ctx)
    {
        RotateBarrel(ctx.direction);
        GetNextBullet();

        if (ctx.direction < 0)
        {
            EventBus<HapticFeedbackEvent>.Raise(new HapticFeedbackEvent(0.05f, 0.0f, 0.1f));
            return;
        }

        EventBus<HapticFeedbackEvent>.Raise(new HapticFeedbackEvent(0.0f, 0.05f, 0.1f));
    }

    public void RotateBarrel(int direction)
    {
        currentChamber += direction;

        if (currentChamber > bulletArray.Length - 1) currentChamber = 0;
        else if (currentChamber < 0) currentChamber = bulletArray.Length - 1;
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
        GetNextBullet();

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

    private void GetNextBullet()
    {
        BulletSO nextBullet = bulletArray[currentChamber];
        EventBus<NextBulletEvent>.Raise(new NextBulletEvent(nextBullet));
    }
}
