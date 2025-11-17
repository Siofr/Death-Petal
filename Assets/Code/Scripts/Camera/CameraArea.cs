using Unity.Cinemachine;
using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public class CameraArea : MonoBehaviour
{
    public Transform cameraPosition;
    private CinemachineCamera cam;

    private void Awake()
    {
        cam = GetComponentInChildren<CinemachineCamera>();
        cam.Priority = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            // Trigger Event
            EventBus<CameraChangeEvent>.Raise(new CameraChangeEvent(cameraPosition, cam));
        }
    }

    private void OnDrawGizmos()
    {
        // Draw wire box around area collider
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f); // Green

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        Vector3 boxSize = new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z);
        Vector3 boxPosition = new Vector3(boxCollider.bounds.center.x, boxCollider.bounds.center.y, boxCollider.bounds.center.z);

        Gizmos.DrawWireCube(boxPosition, boxSize);

        // Draw Line from collider position to camera position

        Gizmos.color = new Color(0f, 0f, 1f, 0.5f); // Blue
        Gizmos.DrawLine(boxPosition, cameraPosition.position);
    }
}
