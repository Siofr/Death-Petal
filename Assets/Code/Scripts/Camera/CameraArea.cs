using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public class CameraArea : MonoBehaviour
{
    public Transform cameraPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            // Trigger Event
            EventBus<CameraChangeEvent>.Raise(new CameraChangeEvent(cameraPosition));
        }
    }
}
