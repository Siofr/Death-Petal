using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIRevolverIndicator : MonoBehaviour
{
    public SpriteRenderer[] bulletSprites = new SpriteRenderer[6];
    private int currentBullet = 0;
    private int shootIndex;

    private bool _isRotating;
    private Vector3 _startRot;
    private int _newZRot;
    [SerializeField] private Canvas parentCanvas;

    Sequence animationSequence;
    public Transform barrel;

    private EventBindings<ShootEvent> _shootEventListener;
    private EventBindings<RemoveBulletEvent> _removeBulletEventListener;
    private EventBindings<AddBulletEvent> _addBulletEventListener;
    private EventBindings<RotateBarrelEvent> _rotateBarrelListener;
    private EventBindings<SetChamberEvent> _setChamberEventListener;
    //private EventBindings<CameraChangeEvent> _cameraChangeEventListener;
    
    private void Awake()
    {
        //_cameraChangeEventListener = new EventBindings<CameraChangeEvent>(OnChangeCamera);
        _shootEventListener = new EventBindings<ShootEvent>(ShootBullet);
        _removeBulletEventListener = new EventBindings<RemoveBulletEvent>(RemoveBullet);
        _addBulletEventListener = new EventBindings<AddBulletEvent>(AddBullet);
        _rotateBarrelListener = new EventBindings<RotateBarrelEvent>(RotateBarrel);
        _setChamberEventListener = new EventBindings<SetChamberEvent>(OnSetChamber);

        _newZRot = Mathf.RoundToInt(transform.eulerAngles.z);
    }

    private void OnEnable()
    {
        //EventBus<CameraChangeEvent>.Register(_cameraChangeEventListener);
        EventBus<ShootEvent>.Register(_shootEventListener);
        EventBus<RemoveBulletEvent>.Register(_removeBulletEventListener);
        EventBus<AddBulletEvent>.Register(_addBulletEventListener);
        EventBus<RotateBarrelEvent>.Register(_rotateBarrelListener);
        EventBus<SetChamberEvent>.Register(_setChamberEventListener);
    }

    private void OnDisable()
    {
        //EventBus<CameraChangeEvent>.Unregister(_cameraChangeEventListener);
        EventBus<ShootEvent>.Unregister(_shootEventListener);
        EventBus<RemoveBulletEvent>.Unregister(_removeBulletEventListener);
        EventBus<AddBulletEvent>.Unregister(_addBulletEventListener);
        EventBus<RotateBarrelEvent>.Unregister(_rotateBarrelListener);
        EventBus<SetChamberEvent>.Unregister(_setChamberEventListener);
    }

    private void Start()
    {
        
        for (int i = 0; i < transform.childCount;  i++)
        {
            bulletSprites[i] = transform.GetChild(i).GetComponentInChildren<SpriteRenderer>();
            bulletSprites[i].transform.parent.GetComponent<MeshRenderer>().enabled = false;
        }

        _startRot = transform.eulerAngles;
        DOTween.Init();
    }

    public void Initialize()
    {
        StartReload();
    }

    public void RotateBarrel(RotateBarrelEvent ctx)
    {
        StartCoroutine(Rotate(ctx.direction, 30, 0.05f));
    }

    IEnumerator Rotate(int direction, int angle, float speed)
    {
        currentBullet += direction;
        Debug.Log(currentBullet);

        if (currentBullet < 0) { currentBullet = bulletSprites.Length - 1; }
        if (currentBullet > bulletSprites.Length - 1) { currentBullet = 0; }

        if (_isRotating) yield return animationSequence.WaitForCompletion();

        _isRotating = true;

        // Rotate barrel in specified direction
        Debug.Log(_newZRot);

        int change;

        if ((_newZRot + 30) / 30 % 2 == 0) {  change = angle * direction; }
        else { change = (angle * 2) * direction; }

        _newZRot += change;

        Vector3 rot = new Vector3(_startRot.x, _startRot.y, _newZRot);
        animationSequence = DOTween.Sequence();
            animationSequence.Append(transform.DORotate(rot, speed));
        
        yield return animationSequence.WaitForCompletion();

        _isRotating = false;
    }
    
    private void OnChangeCamera(CameraChangeEvent ctx)
    {
        parentCanvas.transform.SetParent(ctx.transform);
        parentCanvas.transform.localPosition = Vector3.zero;
        parentCanvas.transform.localEulerAngles = Vector3.zero;
    }

    public void EndReload()
    {
        shootIndex = 0;
        int diff = bulletSprites.Length - currentBullet;

        if (diff != 0) StartCoroutine(Rotate(1, diff * 30, 0.05f));
    }

    public void StartReload()
    {
        int diff = 0;
        diff = currentBullet;

        if (diff != 0) StartCoroutine(Rotate(1, diff * 30, 0.00f));
    }

    public void ShootBullet()
    {
        if (bulletSprites[currentBullet].enabled != false) { 
            bulletSprites[currentBullet].enabled = false;
            bulletSprites[currentBullet].transform.parent.GetComponent<MeshRenderer>().enabled = false;
        }

        shootIndex++;
        // Now Reorder it
        // bulletSprites = ReorderArray(bulletSprites);

        StartCoroutine(Rotate(1, 30, 0.05f));
    }

    public void AddBullet(AddBulletEvent ctx)
    {
        int trapdoorChamber = currentBullet - 1;

        if (trapdoorChamber < 0) trapdoorChamber = bulletSprites.Length - 1;
        if (bulletSprites[trapdoorChamber].enabled) return;

        bulletSprites[trapdoorChamber].sprite = ctx.bulletType.bulletReticle;
        bulletSprites[trapdoorChamber].enabled = true;
        //bulletSprites[trapdoorChamber].color = ctx.bulletType.bulletColor;
        bulletSprites[trapdoorChamber].transform.parent.GetComponent<MeshRenderer>().enabled = true;

        if (TEMP_ReloadTesting.Instance.manualRotate)
        {
            return;
        }

        StartCoroutine(Rotate(-1, 30, 0.05f));
    }

    public void RemoveBullet()
    {
        if (!bulletSprites[currentBullet].enabled) return;

        bulletSprites[currentBullet].enabled = false;
        bulletSprites[currentBullet].transform.parent.GetComponent<MeshRenderer>().enabled = false;

        if (TEMP_ReloadTesting.Instance.manualRotate)
        {
            return;
        }

        StartCoroutine(Rotate(1, 30, 0.05f));
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

    private void OnSetChamber(SetChamberEvent ctx)
    {
        foreach (var img in bulletSprites) img.enabled = false;

        for (int i = 0; i < ctx.bulletOrder.Length; i++)
        {
            var tempSlot = currentBullet + i;

            if (tempSlot > 5) tempSlot = 0;

            if (ctx.bulletOrder[i] == null) continue;
            
            bulletSprites[tempSlot].enabled = true;
            bulletSprites[tempSlot].transform.parent.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
