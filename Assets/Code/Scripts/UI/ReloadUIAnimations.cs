using UnityEngine;

public class ReloadUIAnimations : MonoBehaviour
{
    private EventBindings<EndLongReload> _endLongReloadEventListener;
    private EventBindings<StartLongReload> _startLongReloadListener;
    private EventBindings<AddBulletEvent> _addBulletEventListener;
    
    private Animator _animator;
    public Animator _indicatorAnimator;
    
    private void Awake()
    {
        _endLongReloadEventListener = new EventBindings<EndLongReload>(OnReloadEnd);
        _startLongReloadListener = new EventBindings<StartLongReload>(OnReloadStart);
        _addBulletEventListener = new EventBindings<AddBulletEvent>(AddBullet);
        
        
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        
        EventBus<EndLongReload>.Register(_endLongReloadEventListener);
        EventBus<StartLongReload>.Register(_startLongReloadListener);
        EventBus<AddBulletEvent>.Register(_addBulletEventListener);
    }

    private void OnDisable()
    {
        EventBus<EndLongReload>.Unregister(_endLongReloadEventListener);
        EventBus<StartLongReload>.Unregister(_startLongReloadListener);
        EventBus<AddBulletEvent>.Unregister(_addBulletEventListener);
    }

    void OnReloadStart()
    {
        _animator.SetBool("Reloading", true);
    }

    void OnReloadEnd()
    {
        _animator.SetBool("Reloading", false);
    }

    void AddBullet(AddBulletEvent ctx)
    {
        switch (ctx.bulletType.weakness)
        {
            case WeakTypes.BLUE:
                _indicatorAnimator.SetTrigger("Left");
                break;
            case WeakTypes.RED:
                _indicatorAnimator.SetTrigger("Right");
                break;
            case WeakTypes.GREEN:
                _indicatorAnimator.SetTrigger("Top");
                break;
            default:
                Debug.LogWarning("Unknown bullet type: " + ctx.bulletType.weakness);
                break;
        }
    }
}


