using UnityEngine;
using UnityEngine.UI;

public struct UIAddBullet : IEvent
{
    public BulletSO bulletType;

    public UIAddBullet(BulletSO newBulletType)
    {
        this.bulletType = newBulletType;
    }
}

public struct  UIRemoveBullet : IEvent { }

public struct UIRotateBarrel : IEvent
{
    public int direction;

    public UIRotateBarrel(int newDirection)
    {
        this.direction = newDirection;
    }
}

public class UIAmmoIndicator : MonoBehaviour
{
    private RadialLayoutGroup _group;
    private Transform[] _cylinders;
    private int _currentCylinder;

    public EventBindings<UIAddBullet> addBulletListener;
    public EventBindings<UIRemoveBullet> removeBulletListener;
    public EventBindings<UIRotateBarrel> rotateBarrelListener;

    private void Awake()
    {
        addBulletListener = new EventBindings<UIAddBullet>(AddBullet);
        removeBulletListener = new EventBindings<UIRemoveBullet>(RemoveBullet);
        rotateBarrelListener = new EventBindings<UIRotateBarrel>(RotateBarrelEvent);
    }

    private void OnEnable()
    {
        EventBus<UIAddBullet>.Register(addBulletListener);
        EventBus<UIRemoveBullet>.Register(removeBulletListener);
        EventBus<UIRotateBarrel>.Register(rotateBarrelListener);
    }

    void Start()
    {
        _group = GetComponentInChildren<RadialLayoutGroup>();
        _cylinders = transform.GetChild(0).GetComponentsInChildren<Transform>();
    }

    public void AddBullet(UIAddBullet ctx) 
    {
        Image cylinderImage = _cylinders[_currentCylinder].GetComponent<Image>();
        cylinderImage.enabled = true;
        cylinderImage.color = ctx.bulletType.bulletColor;
        _currentCylinder += 1;
        RotateBarrel(1);
    }

    public void RemoveBullet()
    {
        Image cylinderImage = _cylinders[_currentCylinder].GetComponent<Image>();
        cylinderImage.enabled = false;
        _currentCylinder -= 1;
        RotateBarrel(-1);
    }

    public void RotateBarrelEvent(UIRotateBarrel ctx)
    {
        RotateBarrel(ctx.direction);
    }

    public void RotateBarrel(int direction)
    {
        _group.rotationOffset += 30 * direction;
    }
}
