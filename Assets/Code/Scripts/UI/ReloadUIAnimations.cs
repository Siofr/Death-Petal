using UnityEngine;

public class ReloadUIAnimations : MonoBehaviour
{
    private EventBindings<EndLongReload> _endLongReloadEventListener;
    private EventBindings<StartLongReload> _startLongReloadListener;
    
    private Animator _animator;
    
    private void Awake()
    {
        _endLongReloadEventListener = new EventBindings<EndLongReload>(OnReloadEnd);
        _startLongReloadListener = new EventBindings<StartLongReload>(OnReloadStart);
        
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        
        EventBus<EndLongReload>.Register(_endLongReloadEventListener);
        EventBus<StartLongReload>.Register(_startLongReloadListener);
    }

    private void OnDisable()
    {
        EventBus<EndLongReload>.Unregister(_endLongReloadEventListener);
        EventBus<StartLongReload>.Unregister(_startLongReloadListener);
    }

    void OnReloadStart()
    {
        _animator.SetBool("Reloading", true);
    }

    void OnReloadEnd()
    {
        _animator.SetBool("Reloading", false);
    }
}


