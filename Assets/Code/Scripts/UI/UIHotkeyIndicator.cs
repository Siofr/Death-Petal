using UnityEngine;

public class UIHotkeyIndicator : MonoBehaviour
{
    private EventBindings<StartLongReload> _onLongReloadStartListener;
    private EventBindings<EndLongReload> _onLongReloadCancelListener;

    private Transform childTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _onLongReloadStartListener = new EventBindings<StartLongReload>(ShowUI);
        _onLongReloadCancelListener = new EventBindings<EndLongReload>(HideUI);

        childTransform = transform.GetChild(0);
    }

    private void OnEnable()
    {
        EventBus<StartLongReload>.Register(_onLongReloadStartListener);
        EventBus<EndLongReload>.Register(_onLongReloadCancelListener);
    }

    private void OnDisable()
    {
        EventBus<StartLongReload>.Unregister(_onLongReloadStartListener);
        EventBus<EndLongReload>.Unregister(_onLongReloadCancelListener);
    }

    private void Start()
    {
        HideUI();
    }

    private void ShowUI()
    {
        childTransform.gameObject.SetActive(true);
    }

    private void HideUI()
    {
        childTransform.gameObject.SetActive(false);
    }
}
