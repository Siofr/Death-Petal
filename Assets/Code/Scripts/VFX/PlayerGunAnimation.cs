using UnityEngine;

public class PlayerGunAnimation : MonoBehaviour
{
    // EventBindings<ShootEvent> _shootEventListener;
    private EventBindings<AddBulletEvent> _addBulletEventListener;
    private EventBindings<RemoveBulletEvent> _removeEventListener;
    //private EventBindings<StartLongReload> _startLongReloadListener;
    //private EventBindings<EndLongReload> _endLongReloadListener;
    //private EventBindings<QuickReload> _quickReloadListener;
    
    private Animator _animator;

    public void Awake()
    {
        //_shootEventListener = new EventBindings<ShootEvent>(ShootBullet);
        _addBulletEventListener = new EventBindings<AddBulletEvent>(OnBulletAction);
        _removeEventListener = new EventBindings<RemoveBulletEvent>(OnBulletAction);
        //_startLongReloadListener = new EventBindings<StartLongReload>(Initialize);
        //_endLongReloadListener = new EventBindings<EndLongReload>(SaveArray);
        //_quickReloadListener = new EventBindings<QuickReload>(QuickReload);
        
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        //EventBus<ShootEvent>.Register(_shootEventListener);
        EventBus<AddBulletEvent>.Register(_addBulletEventListener);
        EventBus<RemoveBulletEvent>.Register(_removeEventListener);
        //EventBus<StartLongReload>.Register(_startLongReloadListener);
        //EventBus<EndLongReload>.Register(_endLongReloadListener);
        //<QuickReload>.Register(_quickReloadListener);
    }

    private void OnDisable()
    {
        //EventBus<ShootEvent>.Unregister(_shootEventListener);
        EventBus<AddBulletEvent>.Unregister(_addBulletEventListener);
        EventBus<RemoveBulletEvent>.Unregister(_removeEventListener);
        //<StartLongReload>.Unregister(_startLongReloadListener);
        //EventBus<EndLongReload>.Unregister(_endLongReloadListener);
        //EventBus<QuickReload>.Unregister(_quickReloadListener);
    }

    private void OnBulletAction()
    {
        _animator.SetTrigger("Reload");
    }
}
