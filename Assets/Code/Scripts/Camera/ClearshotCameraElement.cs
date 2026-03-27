using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineCamera))]
public class ClearshotCameraElement: CinemachineCameraEvents
{
    [SerializeField] private CinemachineCamera[] _prioritisedCameras;
    
    //Non-Serialized Fields
    private CinemachineCamera _cam;
    
    private void Awake()
    {
        _cam = GetComponent<CinemachineCamera>();
        CameraActivatedEvent.AddListener(ChangeCamera);
    }

    private void ChangeCamera(ICinemachineMixer mixer, ICinemachineCamera cam)
    {
        if ((ICinemachineCamera)_cam != cam) return;
        
        EventBus<CameraChangeEvent>.Raise(new CameraChangeEvent(transform, _cam));
    }
}