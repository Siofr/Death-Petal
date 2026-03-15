using Unity.Cinemachine;
using UnityEngine;

public class Area : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] _targetCameras;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ClearshotCameraManager.Instance.UseCameras(_targetCameras);
        }
    }
}