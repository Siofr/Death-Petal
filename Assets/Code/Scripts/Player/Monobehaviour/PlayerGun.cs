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

    private void OnDisable()
    {
        EventBus<ShootEvent>.Unregister(_shootEventListener);
        EventBus<AddBulletEvent>.Unregister(_addBulletEventListener);
        EventBus<RemoveBulletEvent>.Unregister(_removeEventListener);
        EventBus<StartLongReload>.Unregister(_startLongReloadListener);
        EventBus<EndLongReload>.Unregister(_endLongReloadListener);
        EventBus<QuickReload>.Unregister(_quickReloadListener);
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

    private void QuickReload()
    {
        bulletIndex = lastBulletIndex;
        bulletArray = CopyArray(lastBulletArray);
        Debug.Log("Bullets Loaded" + bulletArray);
    }

    public void ShootBullet(ShootEvent ctx)
    {
        // I want to shoot element at 0 always
        _shootEvent.setParameterByID(_bulletsLeft, bulletIndex);
        EventBus<SFXEventTrigger>.Raise(new SFXEventTrigger(_shootEvent, this.gameObject));

        if (bulletArray[0] == null)
        {
            EventBus<HapticFeedbackEvent>.Raise(new HapticFeedbackEvent(0.0f, 0.5f, 0.15f));
            return;
        }

        EventBus<SpawnTrail>.Raise(new SpawnTrail(bulletArray[0].bulletColor));
        EventBus<HapticFeedbackEvent>.Raise(new HapticFeedbackEvent(0.5f, 0.0f, 0.25f));

        // Now remove it
        if (ctx.weakness)
        {
            ctx.weakness.ParentEntity.OnShot(ctx.weakness, bulletArray[0].weakness);
        }

        bulletArray[0] = null;

        bulletIndex--;
        // Now Reorder it
        bulletArray = ReorderArray(bulletArray);
        GetNextBullet();
    }

    public void AddBullet(AddBulletEvent ctx)
    {
        if (bulletIndex >= bulletArray.Length) return;

        _addRemoveEvent.setParameterByID(_addRemove, 1);
        EventBus<HapticFeedbackEvent>.Raise(new HapticFeedbackEvent(0.0f, 0.05f, 0.15f));
        EventBus<SFXEventTrigger>.Raise(new SFXEventTrigger(_addRemoveEvent, this.gameObject));

        bulletArray[bulletIndex] = ctx.bulletType;
        bulletIndex++;
        lastBulletIndex = bulletIndex;
    }

    public void RemoveBullet()
    {
        if (bulletIndex - 1 < 0) return;

        _addRemoveEvent.setParameterByID(_addRemove, 0);
        EventBus<HapticFeedbackEvent>.Raise(new HapticFeedbackEvent(0.05f, 0.0f, 0.15f));
        EventBus<SFXEventTrigger>.Raise(new SFXEventTrigger(_addRemoveEvent, this.gameObject));

        bulletArray[bulletIndex - 1] = null;
        bulletIndex--;
        lastBulletIndex = bulletIndex;
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
        BulletSO nextBullet = bulletArray[0];
        EventBus<NextBulletEvent>.Raise(new NextBulletEvent(nextBullet));
    }
}
