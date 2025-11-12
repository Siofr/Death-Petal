using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIRevolverIndicator : MonoBehaviour
{
    public Image[] bulletSprites = new Image[6];
    private int currentBullet = 0;
    private int shootIndex;
    private Transform _cylinder;

    private EventBindings<ShootEvent> _shootEventListener;
    private EventBindings<RemoveBulletEvent> _removeBulletEventListener;
    private EventBindings<AddBulletEvent> _addBulletEventListener;
    private EventBindings<EndLongReload> _endLongReloadEventListener;
    private EventBindings<StartLongReload> _startLongReloadListener;

    private void Awake()
    {
        _shootEventListener = new EventBindings<ShootEvent>(ShootBullet);
        _removeBulletEventListener = new EventBindings<RemoveBulletEvent>(RemoveBullet);
        _addBulletEventListener = new EventBindings<AddBulletEvent>(AddBullet);
        _endLongReloadEventListener = new EventBindings<EndLongReload>(EndReload);
        _startLongReloadListener = new EventBindings<StartLongReload>(Initialize);
    }

    private void OnEnable()
    {
        EventBus<ShootEvent>.Register(_shootEventListener);
        EventBus<RemoveBulletEvent>.Register(_removeBulletEventListener);
        EventBus<AddBulletEvent>.Register(_addBulletEventListener);
        EventBus<EndLongReload>.Register(_endLongReloadEventListener);
        EventBus<StartLongReload>.Register(_startLongReloadListener);
    }

    private void Start()
    {
        _cylinder = transform.GetChild(0);
        for (int i = 0; i < _cylinder.childCount;  i++)
        {
            bulletSprites[i] = _cylinder.GetChild(i).GetComponent<Image>();
        }

        DOTween.Init();
    }

    public void Initialize()
    {
        shootIndex = 0;
        int j = currentBullet;
        for (int i = 0; i < j; i++)
        {
            RemoveBullet();
        }
    }

    public void ShootBullet()
    {
        // 0. Sanity Check 1. Remove current bullet 2. Rotate barrel counter clockwise
        if (!bulletSprites[shootIndex].enabled) return;

        bulletSprites[shootIndex].enabled = false;
        shootIndex += 1;
        currentBullet -= 1;
        Rotate(1, 30);
    }

    public void RemoveBullet()
    {
        // 0. Sanity Check 1. Remove last bullet 2. Rotate barrel clockwise
        if (currentBullet - 1 < 0) return;

        bulletSprites[currentBullet - 1].enabled = false;
        currentBullet -= 1;
        Rotate(-1, 30);
    }

    public void AddBullet(AddBulletEvent ctx)
    {
        // 0. Sanity check 1. Add Bullet 2. Rotate Barrel counter clockwise
        if (currentBullet >= bulletSprites.Length) return;

        bulletSprites[currentBullet].enabled = true;
        bulletSprites[currentBullet].color = ctx.bulletType.bulletColor;
        currentBullet += 1;
        Rotate(1, 30);
    }

    public void Rotate(int direction, int angle)
    {
        // Rotate barrel in specified direction
        int zRot = Mathf.RoundToInt(_cylinder.eulerAngles.z);

        if ((zRot + 30) / 30 % 2 == 0) { zRot = zRot + (angle * 2) * direction; }
        else { zRot = zRot + angle * direction; }

        Vector3 rot = new Vector3(0, 0, zRot);
        _cylinder.transform.DORotate(rot, 0.12f, RotateMode.FastBeyond360);
    }

    public void EndReload()
    {
        int diff = bulletSprites.Length - currentBullet;

        if (diff != 0) Rotate(1, diff * 30);
    }
}
