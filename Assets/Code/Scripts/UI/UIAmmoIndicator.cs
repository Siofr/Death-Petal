using UnityEngine;
using UnityEngine.UI;

public class UIAmmoIndicator : MonoBehaviour
{
    private RadialLayoutGroup _group;
    private Transform barrel;
    private Transform[] _cylinders;
    private int _currentCylinder;

    public EventBindings<AddBulletEvent> addBulletListener;
    public EventBindings<RemoveBulletEvent> removeBulletListener;
    public EventBindings<RotateBarrelEvent> rotateBarrelListener;

    private void Awake()
    {
        addBulletListener = new EventBindings<AddBulletEvent>(AddBullet);
        removeBulletListener = new EventBindings<RemoveBulletEvent>(RemoveBullet);
        rotateBarrelListener = new EventBindings<RotateBarrelEvent>(OnRotateBarrel);
    }

    private void OnEnable()
    {
        EventBus<AddBulletEvent>.Register(addBulletListener);
        EventBus<RemoveBulletEvent>.Register(removeBulletListener);
        EventBus<RotateBarrelEvent>.Register(rotateBarrelListener);
    }

    void Start()
    {
        _group = GetComponentInChildren<RadialLayoutGroup>();
        barrel = transform.GetChild(0).transform;
        _cylinders = new Transform[transform.GetChild(0).childCount];

        for (int i = 0; i <= _cylinders.Length - 1; i++)
        {
            _cylinders[i] = transform.GetChild(0).GetChild(i);
        }
    }

    public void AddBullet(AddBulletEvent ctx) 
    {
        if (_currentCylinder >= _cylinders.Length) return;

        Image cylinderImage = _cylinders[_currentCylinder].GetComponent<Image>();
        cylinderImage.enabled = true;
        cylinderImage.color = ctx.bulletType.bulletColor;
        _currentCylinder += 1;
        RotateBarrel(1);
    }

    public void RemoveBullet()
    {
        if (_currentCylinder - 1 < 0) return;

        Image cylinderImage = _cylinders[_currentCylinder - 1].GetComponent<Image>();
        cylinderImage.enabled = false;
        _currentCylinder -= 1;
        RotateBarrel(-1);
    }

    public void OnRotateBarrel(RotateBarrelEvent ctx)
    {
        RotateBarrel(ctx.direction);
    }

    public void RotateBarrel(int direction)
    {
        float zRot = barrel.transform.rotation.z;
        if (((_group.rotationOffset + 30) / 30) % 2 == 0) { Mathf.FloorToInt(zRot = zRot + 30.0f * direction); }
        else { zRot = Mathf.FloorToInt(zRot + 60.0f * direction); }

        Vector3 newRot = new Vector3(0, 0, zRot);
        barrel.transform.Rotate(newRot);
    }
}
