using System.Linq;
using Unity.Cinemachine;

public class ClearshotCameraManager: Singleton<ClearshotCameraManager>
{
    private CinemachineCamera[] _cameras;
    
    private void Awake()
    {
        _cameras = GetComponentsInChildren<CinemachineCamera>();
    }

    public void UseCameras(CinemachineCamera[] targetCameras)
    {
        var targetCamList = targetCameras.ToList();

        foreach (var cam in _cameras)
        {
            if (!targetCameras.Contains(cam)) cam.enabled = false;
            else cam.enabled = true;
        }
    }
}