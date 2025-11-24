using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class UIRevolverIndicator : MonoBehaviour
{
    public Image[] bulletSprites = new Image[6];
    private Color[] lastBulletColors = new Color[6];
    private int currentBullet = 0;
    private int shootIndex;
    private int bulletsLoaded;

    private EventBindings<ShootEvent> _shootEventListener;
    private EventBindings<RemoveBulletEvent> _removeBulletEventListener;
    private EventBindings<AddBulletEvent> _addBulletEventListener;
    private EventBindings<EndLongReload> _endLongReloadEventListener;
    private EventBindings<StartLongReload> _startLongReloadListener;
    private EventBindings<QuickReload> _quickReloadEventListener;

    private void Awake()
    {
        _shootEventListener = new EventBindings<ShootEvent>(ShootBullet);
        _removeBulletEventListener = new EventBindings<RemoveBulletEvent>(RemoveBullet);
        _addBulletEventListener = new EventBindings<AddBulletEvent>(AddBullet);
        _endLongReloadEventListener = new EventBindings<EndLongReload>(EndReload);
        _startLongReloadListener = new EventBindings<StartLongReload>(Initialize);
        _quickReloadEventListener = new EventBindings<QuickReload>(QuickReload);
    }

    private void OnEnable()
    {
        EventBus<ShootEvent>.Register(_shootEventListener);
        EventBus<RemoveBulletEvent>.Register(_removeBulletEventListener);
        EventBus<AddBulletEvent>.Register(_addBulletEventListener);
        EventBus<EndLongReload>.Register(_endLongReloadEventListener);
        EventBus<StartLongReload>.Register(_startLongReloadListener);
        EventBus<QuickReload>.Register(_quickReloadEventListener);
    }

    private void Start()
    {
        for (int i = 0; i < transform.childCount;  i++)
        {
            bulletSprites[i] = transform.GetChild(i).GetComponent<Image>();
        }

        DOTween.Init();
    }

    public void Initialize()
    {
        StartReload();
        shootIndex = 0;
    }

    public void ShootBullet()
    {
        // 0. Sanity Check 1. Remove current bullet 2. Rotate barrel counter clockwise
        if (shootIndex >= bulletSprites.Length || shootIndex >= bulletsLoaded) return;

        bulletSprites[shootIndex].enabled = false;
        shootIndex += 1;
        currentBullet -= 1;
        Rotate(1, 30, 0.05f);
    }

    public void RemoveBullet()
    {
        // 0. Sanity Check 1. Remove last bullet 2. Rotate barrel clockwise
        if (currentBullet - 1 < 0) return;

        bulletSprites[currentBullet - 1].enabled = false;
        currentBullet -= 1;
        Rotate(-1, 30, 0.05f);
    }

    public void AddBullet(AddBulletEvent ctx)
    {
        // 0. Sanity check 1. Add Bullet 2. Rotate Barrel counter clockwise
        if (currentBullet >= bulletSprites.Length) return;

        bulletSprites[currentBullet].enabled = true;
        bulletSprites[currentBullet].sprite = ctx.bulletType.bulletSprite;
        currentBullet += 1;
        Rotate(1, 30, 0.05f);
    }

    public void Rotate(int direction, int angle, float speed)
    {
        // Rotate barrel in specified direction
        int zRot = Mathf.RoundToInt(transform.eulerAngles.z);

        if ((zRot + 30) / 30 % 2 == 0) {  zRot = zRot + angle * direction; }
        else { zRot = zRot + (angle * 2) * direction; }

        Vector3 rot = new Vector3(0, 0, zRot);
        transform.DORotate(rot, speed, RotateMode.FastBeyond360);
    }

    public void QuickReload()
    {
        Initialize();

        for (int i = 0; i < lastBulletColors.Length; i++)
        {
            bulletSprites[i].color = lastBulletColors[i];
            bulletSprites[i].enabled = true;
            currentBullet += 1;
        }

        EndReload();
    }

    public void EndReload()
    {
        int diff = bulletSprites.Length - currentBullet;
        bulletsLoaded = currentBullet;
        lastBulletColors = new Color[bulletsLoaded];

        for (int i = 0; i < bulletsLoaded; i++)
        {
            lastBulletColors[i] = bulletSprites[i].color;
        }

        if (diff != 0) Rotate(1, diff * 30, 0.05f);
    }

    public void StartReload()
    {
        currentBullet = 0;

        int diff = bulletSprites.Length - shootIndex;

        for (int i = 0; i < bulletSprites.Length; i++)
        {
            bulletSprites[i].enabled = false;
        }

        if (diff != 0) Rotate(1, diff * 30, 0.00f);
    }
}
