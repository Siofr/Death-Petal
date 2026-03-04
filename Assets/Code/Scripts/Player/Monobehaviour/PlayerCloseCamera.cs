using Unity.Cinemachine;
using UnityEngine;

public struct ChangeCameraState : IEvent
{
    public bool state;

    public ChangeCameraState(bool state)
    {
        this.state = state;
    }
}

public class PlayerCloseCamera : MonoBehaviour
{
    private CinemachineCamera cam;
    private EventBindings<ChangeCameraState> _changeCameraStateListener;

    private void Awake()
    {
        _changeCameraStateListener = new EventBindings<ChangeCameraState>(OnStateChange);
    }

    private void OnEnable()
    {
        EventBus<ChangeCameraState>.Register(_changeCameraStateListener);
    }

    private void OnDisable()
    {
        EventBus<ChangeCameraState>.Unregister(_changeCameraStateListener);
    }

    void Start()
    {
        cam = GetComponent<CinemachineCamera>();
    }

    private void OnStateChange(ChangeCameraState ctx)
    {
        cam.enabled = ctx.state;
    }
}
