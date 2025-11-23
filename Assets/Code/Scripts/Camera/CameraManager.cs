using Unity.Cinemachine;
using UnityEngine;

public struct CameraChangeEvent : IEvent
{
    public Transform transform;
    public CinemachineCamera cam;

    public CameraChangeEvent (Transform newTransform, CinemachineCamera newCam)
    {
        this.transform = newTransform;
        this.cam = newCam;
    }
}

public class CameraManager : Singleton<CameraManager>
{
    public Transform activeCam;
    public Transform lastCamTransform;
    private CinemachineCamera _activeCineCam;
    public EventBindings<CameraChangeEvent> _cameraChangeEventListener;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        lastCamTransform = activeCam;
        _activeCineCam = activeCam.GetComponent<CinemachineCamera>();
        _activeCineCam.Priority = 1;
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
        // Change Priority instead
        lastCamTransform = activeCam;
        _activeCineCam.Priority = 0;
        _activeCineCam = ctx.cam;
        activeCam.gameObject.SetActive(false);
        _activeCineCam.Priority = 1;
        activeCam = ctx.transform;
        activeCam.gameObject.SetActive(true);
    }
}
