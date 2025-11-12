using UnityEngine;

public struct CameraChangeEvent : IEvent
{
    public Transform transform;

    public CameraChangeEvent (Transform newTransform)
    {
        transform = newTransform;
    }
}

public class CameraManager : Singleton<CameraManager>
{
    public Transform activeCam;
    public Transform lastCamTransform;
    public EventBindings<CameraChangeEvent> _cameraChangeEventListener;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        lastCamTransform = activeCam;
    }

    private void OnEnable()
    {
        EventBus<CameraChangeEvent>.Register(_cameraChangeEventListener);
    }

    protected override void Awake()
    {
        base.Awake();
        _cameraChangeEventListener = new EventBindings<CameraChangeEvent>(OnChangeCamera);
    }

    private void OnChangeCamera(CameraChangeEvent ctx)
    {
        Debug.Log("Camera Changed");
        lastCamTransform = activeCam;
        activeCam.gameObject.SetActive(false);
        activeCam = ctx.transform;
        activeCam.gameObject.SetActive(true);
    }
}
