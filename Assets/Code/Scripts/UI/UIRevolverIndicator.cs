using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class UIRevolverIndicator : MonoBehaviour
{
    public Image[] bulletSprites = new Image[6];
    private Color[] lastBulletColors = new Color[6];
    private int currentBullet = 0;
    private int lastBulletCount;
    private int shootIndex;
    private int currentBulletsLoaded;
    private int lastBulletsLoaded;

    private EventBindings<ShootEvent> _shootEventListener;
    private EventBindings<RemoveBulletEvent> _removeBulletEventListener;
    private EventBindings<AddBulletEvent> _addBulletEventListener;
    private EventBindings<EndLongReload> _endLongReloadEventListener;
    private EventBindings<StartLongReload> _startLongReloadListener;
    private EventBindings<QuickReload> _quickReloadEventListener;

    private void Awake()
    {
        _shootEventListener = new EventBindings<ShootEvent>(NewShootBullet);
        _removeBulletEventListener = new EventBindings<RemoveBulletEvent>(NewRemoveBullet);
        _addBulletEventListener = new EventBindings<AddBulletEvent>(NewAddBullet);
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
    }

    public void ShootBullet()
    {
        // 0. Sanity Check 1. Remove current bullet 2. Rotate barrel counter clockwise
        if (shootIndex >= bulletSprites.Length || shootIndex >= currentBullet) return;

        bulletSprites[shootIndex].enabled = false;
        currentBulletsLoaded -= 1;
        shootIndex += 1;
        Rotate(1, 30, 0.05f);
    }

    public void RemoveBullet()
    {
        // 0. Sanity Check 1. Remove last bullet 2. Rotate barrel clockwise
        if (currentBullet - 1 < 0) currentBullet = 1;

        bulletSprites[currentBullet - 1].enabled = false;
        currentBulletsLoaded -= 1;
        currentBullet -= 1;
        Rotate(-1, 30, 0.05f);
    }

    public void AddBullet(AddBulletEvent ctx)
    {
        // 0. Sanity check 1. Add Bullet 2. Rotate Barrel counter clockwise
        if (currentBullet >= bulletSprites.Length) currentBullet = 0;

        bulletSprites[currentBullet + shootIndex].enabled = true;
        bulletSprites[currentBullet + shootIndex].sprite = ctx.bulletType.bulletSprite;

        currentBulletsLoaded += 1;
        currentBullet += 1;
        Rotate(1, 30, 0.05f);
    }

    public void Rotate(int direction, int angle, float speed)
    {
        Debug.Log("Rotate");
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
        shootIndex = 0;
        int diff = bulletSprites.Length - currentBullet;

        // lastBulletColors = new Color[bulletsLoaded];

        // for (int i = 0; i < bulletsLoaded; i++)
        // {
        //    lastBulletColors[i] = bulletSprites[i].color;
        // }

        if (diff != 0) Rotate(1, diff * 30, 0.05f);
    }

    public void StartReload()
    {
        int diff = 0;
        diff = currentBullet;
        // currentBullet -= shootIndex;

        //for (int i = 0; i < bulletSprites.Length; i++)
        //{
        //    bulletSprites[i].enabled = false;
        //}

        if (diff != 0) Rotate(1, diff * 30, 0.00f);
    }

    public void NewShootBullet()
    {
        if (bulletSprites[0].enabled == false) return;

        bulletSprites[0].enabled = false;

        shootIndex++;
        currentBullet--;
        // Now Reorder it
        bulletSprites = ReorderArray(bulletSprites);

        Rotate(1, 30, 0.05f);
    }

    public void NewAddBullet(AddBulletEvent ctx)
    {
        Debug.Log("Bullet why current " + currentBullet);
        Debug.Log("Bullet why Shoot Index " + shootIndex);

        if (shootIndex != 0 && currentBullet + shootIndex <= bulletSprites.Length)
        {
            bulletSprites[currentBullet].sprite = ctx.bulletType.bulletSprite;
            bulletSprites[currentBullet].enabled = true;
            shootIndex--;
            currentBullet++;
            Rotate(1, 30, 0.05f);
            return;
        }

        if (currentBullet + shootIndex >= bulletSprites.Length) return;

        bulletSprites[currentBullet + shootIndex].sprite = ctx.bulletType.bulletSprite;
        bulletSprites[currentBullet + shootIndex].enabled = true;
        currentBullet++;

        Rotate(1, 30, 0.05f);
    }

    public void NewRemoveBullet()
    {

        if (shootIndex != 0 && currentBullet + shootIndex - 1 !> bulletSprites.Length)
        {
            bulletSprites[currentBullet + shootIndex - 1].enabled = false;
            currentBullet--;
            Rotate(-1, 30, 0.05f);
            return;
        }

        if (currentBullet - 1 < 0) return;

        bulletSprites[currentBullet - 1].enabled = false;
        currentBullet--;

        Rotate(-1, 30, 0.05f);
    }

    private Image[] CopyArray(Image[] arr)
    {
        Image[] newArr = new Image[6];

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == null) break;
            newArr[i] = arr[i];
        }

        return newArr;
    }

    public Image[] ReorderArray(Image[] arr)
    {
        // Reorder the array
        Image[] newArr = new Image[arr.Length];

        for (int i = 0; i < arr.Length - 1; i++)
        {
            newArr[i] = arr[i + 1];
        }

        newArr[arr.Length - 1] = arr[0];
        return newArr;
    }
}
