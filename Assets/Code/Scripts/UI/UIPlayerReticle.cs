using State_Machine;
using UnityEngine;

public class UIPlayerReticle : MonoBehaviour
{
    private Transform _reticle;
    private Transform _activeTarget;
    private Camera _cam;

    public EventBindings<ActiveTargetEvent> activeTargetEventListener;

    public void Awake()
    {
        activeTargetEventListener = new EventBindings<ActiveTargetEvent>(ActivateTarget);
    }

    public void OnEnable()
    {
        EventBus<ActiveTargetEvent>.Register(activeTargetEventListener);
    }

    void Start()
    {
        _cam = Camera.main;
        _reticle = transform.GetChild(0);
    }

    void Update()
    {
        if (_activeTarget)
        {
            Vector3 screenPos = _cam.WorldToScreenPoint(_activeTarget.position);
            _reticle.position = screenPos;
        }
    }

    private void ActivateTarget(ActiveTargetEvent ctx)
    {
        _activeTarget = ctx.activeTarget;

        if (_activeTarget != null)
        {
            _reticle.gameObject.SetActive(true);
            return;
        }

        _reticle.gameObject.SetActive(false);
    }
}
