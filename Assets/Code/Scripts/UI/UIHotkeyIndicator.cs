using UnityEngine;

public class UIHotkeyIndicator : MonoBehaviour
{
    private EventBindings<StartLongReload> _onLongReloadStartListener;
    private EventBindings<EndLongReload> _onLongReloadCancelListener;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _onLongReloadStartListener = new EventBindings<StartLongReload>(ShowUI);
        _onLongReloadCancelListener = new EventBindings<EndLongReload>(HideUI);
    }

    private void OnEnable()
    {
        EventBus<StartLongReload>.Register(_onLongReloadStartListener);
        EventBus<EndLongReload>.Register(_onLongReloadCancelListener);
    }

    private void Start()
    {
        HideUI();
    }

    private void ShowUI()
    {
        transform.gameObject.SetActive(true);
    }

    private void HideUI()
    {
        transform.gameObject.SetActive(false);
    }
}
