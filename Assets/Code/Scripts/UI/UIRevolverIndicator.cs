using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class UIRevolverIndicator : MonoBehaviour
{
    public Image[] bulletSprites = new Image[6];
    private int currentBullet = 0;
    private int shootIndex;

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

    public void EndReload()
    {
        shootIndex = 0;
        int diff = bulletSprites.Length - currentBullet;

        if (diff != 0) Rotate(1, diff * 30, 0.05f);
    }

    public void StartReload()
    {
        int diff = 0;
        diff = currentBullet;

        if (diff != 0) Rotate(1, diff * 30, 0.00f);
    }

    public void ShootBullet()
    {
        if (bulletSprites[0].enabled == false) return;

        bulletSprites[0].enabled = false;

        shootIndex++;
        currentBullet--;
        // Now Reorder it
        bulletSprites = ReorderArray(bulletSprites);

        Rotate(1, 30, 0.05f);
    }

    public void AddBullet(AddBulletEvent ctx)
    {
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

    public void RemoveBullet()
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
